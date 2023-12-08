using InputDisplay.Config;
using InputDisplay.Inputs;
using InputDisplay.Inputs.Entities;
using InputDisplay.Theme;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class Game1 : Game
{
    readonly GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch = default!;

    InputBuffer buffer = default!;
    GameResources resources = default!;

    readonly ThemeCycle themeCycle = new();
    readonly GameConfigManager configManager = new();
    PlayerPad? player;

    GameConfig Config => configManager.CurrentConfig;

    public Game1()
    {
        graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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
        themeCycle.StartWith(Config.Theme);
        buffer = new(Config);
    }

    protected override void Update(GameTime gameTime)
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

        HandleKeyboard();
        HandleMouse();
        UpdateTheme();
        UpdateConfig();
        base.Update(gameTime);
    }

    void UpdateConfig()
    {
        if (Window.Position.X != Config.Left || Window.Position.Y != Config.Top)
            Config.UpdateWindowSize(Window);
    }

    void HandleKeyboard()
    {
        KeyboardManager.Update();

        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.Space))
        {
            Config.InvertHistory = !Config.InvertHistory;
            configManager.Save();
        }

        if (KeyboardManager.IsKeyDown(Keys.Delete))
        {
            player?.Disconnect();
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

        Config.FallbackTheme = ThemeManager.Get(
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

    void UpdateTheme()
    {
        var theme = Config.Theme;
        if (KeyboardManager.IsKeyPressed(Keys.Up))
            Config.Theme = themeCycle.NextStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Down))
            Config.Theme = themeCycle.PrevStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Left))
            Config.Theme = themeCycle.NextButtons();

        else if (KeyboardManager.IsKeyPressed(Keys.Right))
            Config.Theme = themeCycle.PrevButtons();

        if (Config.Theme != theme) configManager.Save();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Config.ClearColor);

        spriteBatch.Begin();

        if (player is null)
            spriteBatch.DrawString(resources.NumbersFont, "Press any button...", new(20), Color.White);
        else
            buffer.Draw(spriteBatch, resources.NumbersFont, Window.ClientBounds);

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
