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
    bool configUpdated;

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
        Window.ClientSizeChanged += OnResize;

        graphics.PreferredBackBufferWidth = Config.Width;
        graphics.PreferredBackBufferHeight = Config.Height;

        if (Config.Top + Config.Left > 0)
            Window.Position = new(Config.Left, Config.Top);

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
        configUpdated = false;
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

        HandleKeyboard();
        HandleMouse();
        UpdateTheme();
        UpdateConfig();
        configManager.Update();
        base.Update(gameTime);
    }

    public void UpdateTheme()
    {
        if (!themeManager.Update()) return;
        configUpdated = true;
        Config.CurrentTheme = themeManager.CurrentTheme;
    }

    void UpdateConfig()
    {
        if (Window.Position.X != Config.Left || Window.Position.Y != Config.Top)
        {
            Config.UpdateWindowSize(Window);
            configUpdated = true;
        }

        if (configUpdated) configManager.Save();
    }

    void HandleKeyboard()
    {
        KeyboardManager.Update();

        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.Space))
        {
            Config.InvertHistory = !Config.InvertHistory;
            configUpdated = true;
            return;
        }

        if (KeyboardManager.IsKeyDown(Keys.Delete))
        {
            player?.Disconnect();
            return;
        }

        if (KeyboardManager.IsKeyDown(Keys.Back))
            buffer.Clear();
    }

    void HandleMouse()
    {
        MouseManager.Update();
        var wheel = MouseManager.DeltaWheelValue;
        if (wheel is 0) return;

        var step = Math.Sign(wheel) * 5;
        var newSize = MathHelper.Clamp(Config.IconSize + step, 20, 100);
        Config.IconSize = newSize;
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
