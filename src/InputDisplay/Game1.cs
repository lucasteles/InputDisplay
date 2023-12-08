using InputDisplay.Config;
using InputDisplay.Inputs;
using InputDisplay.Inputs.Entities;
using InputDisplay.Themes;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class Game1 : Game
{
    readonly GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch = default!;
    GameResources resources = default!;

    readonly GameConfigManager configManager = new();
    readonly InputBuffer buffer;
    readonly ThemeManager themeManager;

    PlayerPad? player;

    GameConfig Config => configManager.CurrentConfig;

    public Game1()
    {
        graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        themeManager = new(Config.CurrentTheme);
        buffer = new(Config);
    }

    protected override void Initialize()
    {
        Window.Title = "Input Display";
        Window.AllowUserResizing = true;
        Window.IsBorderless = Config.Borderless;
        Window.ClientSizeChanged += OnResize;
        SetWindowPosition();

        graphics.PreferredBackBufferWidth = Config.Width;
        graphics.PreferredBackBufferHeight = Config.Height;
        graphics.ApplyChanges();

        configManager.StartWatch();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new(GraphicsDevice);
        resources = new(Content);

        ThemeManager.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardManager.Update();
        MouseManager.BeginUpdate();

        HandleShortcuts();
        HandleDragging();
        HandlePlayerConnected();

        UpdatePlayer();
        UpdateConfig();
        configManager.Update();
        base.Update(gameTime);

        MouseManager.EndUpdate();
    }

    void UpdatePlayer()
    {
        if (player is null)
        {
            DetectController();
            return;
        }

        player.Update();

        if (!player.IsConnected)
        {
            player = null;
            return;
        }

        buffer.Update(player);

        if (!themeManager.Update()) return;
        Config.Dirty = true;
        Config.CurrentTheme = themeManager.CurrentTheme;
    }

    public void SetWindowPosition()
    {
        if (Config.Top + Config.Left != 0)
            Window.Position = new(Config.Left, Config.Top);
    }


    void UpdateConfig()
    {
        if (MouseManager.IsDragging)
            return;

        if (Window.Position.X != Config.Left || Window.Position.Y != Config.Top)
        {
            Config.UpdateWindowSize(Window);
            Config.Dirty = true;
        }

        if (Window.IsBorderless != Config.Borderless)
        {
            Config.Dirty = true;
            Window.IsBorderless = Config.Borderless;
        }


        if (Config.Dirty)
        {
            configManager.Save();
            Config.Dirty = false;
        }
    }

    void HandleShortcuts()
    {
        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.I))
        {
            Config.InvertHistory = !Config.InvertHistory;
            Config.Dirty = true;
            return;
        }

        if (KeyboardManager.IsKeyDown(Keys.Delete))
        {
            player?.Disconnect();
            return;
        }

        if (MouseManager.WasDoubleLeftClick)
        {
            Config.Borderless = !Config.Borderless;
            Config.Dirty = true;
        }

        if (KeyboardManager.IsKeyDown(Keys.Back))
            buffer.Clear();
    }

    void HandlePlayerConnected()
    {
        if (player?.IsConnected != true) return;
        var wheel = MouseManager.DeltaWheelValue;
        if (wheel is 0) return;
        var step = Math.Sign(wheel) * 5;
        var newSize = MathHelper.Clamp(Config.IconSize + step, 20, 100);
        Config.IconSize = newSize;
    }

    Point? startDragging;

    void HandleDragging()
    {
        if (IsActive && MouseManager.IsDraggingOnWindow(Window))
        {
            startDragging ??= MouseManager.Position;

            var delta = MouseManager.DeltaDragging;
            if (delta == Point.Zero) return;

            var topFix = MouseManager.Position.Y - startDragging.Value.Y;
            var leftFix = MouseManager.Position.X - startDragging.Value.X;

            Config.Top += delta.Y + topFix;
            Config.Left += delta.X + leftFix;
            Config.Dirty = true;
            SetWindowPosition();
            MouseManager.BeginUpdate();
        }

        if (!MouseManager.IsDragging && startDragging is not null)
        {
            startDragging = null;
        }
    }

    void DetectController()
    {
        if (PlayerPad.Detect() is not { } playerPad) return;

        player = playerPad;

        themeManager.SetFallback(
            player.GetPadKind() switch
            {
                PlayerPad.Kind.PlayStation => "PlayStation",
                _ => "XBOX",
            }
        );

        if (Config.InputMap.Contains(player.Identifier)) return;
        Config.InputMap.AddGamePad(player.Capabilities);
        configManager.Save();
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Config.ClearColor);

        spriteBatch.Begin();

        if (player is null)
            spriteBatch.DrawString(resources.NumbersFont, "Press any button...", new(20), Color.White);
        else
            buffer.Draw(
                spriteBatch,
                themeManager.CurrentTheme,
                resources.NumbersFont,
                Window.ClientBounds
            );

        spriteBatch.End();
        base.Draw(gameTime);
    }

    void OnResize(object? sender, EventArgs e)
    {
        Config.UpdateWindowSize(Window);
        configManager.Save();
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        Window.ClientSizeChanged -= OnResize;
        configManager.Dispose();
        base.OnExiting(sender, args);
    }
}
