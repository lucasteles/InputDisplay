using System.Diagnostics;
using InputDisplay.Config;
using InputDisplay.Inputs;
using InputDisplay.Inputs.Entities;
using InputDisplay.Themes;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class GameMain : Game
{
    readonly GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch = default!;
    GameResources resources = default!;

    readonly SettingsManager configManager = new();
    readonly InputBuffer buffer;
    readonly ThemeManager themeManager;

    PlayerPad? player;

    Settings Config => configManager.CurrentConfig;

    readonly Process configProcess = new();

    public GameMain()
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

        if (themeManager.CurrentTheme.ButtonsName != Config.CurrentTheme.Buttons
            || themeManager.CurrentTheme.StickName != Config.CurrentTheme.Direction)
            themeManager.CurrentTheme = ThemeManager.Get(Config.CurrentTheme);

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
        if (!IsActive) return;

        if (KeyboardManager.IsKeyPressed(Keys.Escape))
            if (player is null || PlayerPad.GetConnected().Count() <= 1)
                Exit();
            else
                player = null;

        if (KeyboardManager.IsKeyPressed(Keys.I))
        {
            Config.InvertHistory = !Config.InvertHistory;
            Config.Dirty = true;
            return;
        }

        if (KeyboardManager.IsKeyPressed(Keys.Delete))
        {
            player?.Disconnect();
            return;
        }

        if (KeyboardManager.IsKeyPressed(Keys.B))
        {
            Config.Borderless = !Config.Borderless;
            Config.Dirty = true;
        }

        if (KeyboardManager.IsKeyPressed(Keys.Back))
            buffer.Clear();

        if (KeyboardManager.IsKeyPressed(Keys.F1) ||
            (MouseManager.WasDoubleLeftClick && MouseManager.IsOnWindow(Window)))
            StartConfig();
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
    bool configStarted;
    bool startingConfig;

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

    void StartConfig()
    {
        if (startingConfig) return;
        startingConfig = true;

        configProcess.EnableRaisingEvents = true;
        var si = configProcess.StartInfo;
        si.UseShellExecute = false;
        si.FileName = Process.GetCurrentProcess().MainModule?.FileName;
        si.Arguments = $"config {player?.Index.ToString() ?? string.Empty}".Trim();

        if (configStarted && configProcess.Responding)
        {
            configProcess.CloseMainWindow();
            configProcess.WaitForExit();
        }

        configStarted = configProcess.Start();
        configProcess.WaitForInputIdle();

        startingConfig = false;
    }

    void DetectController()
    {
        if (PlayerPad.DetectPress() is not { } playerPad) return;

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
        if (configStarted && configProcess.Responding)
        {
            configProcess.CloseMainWindow();
            configProcess.WaitForExit();
        }

        base.OnExiting(sender, args);
    }
}
