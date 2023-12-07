using InputDisplay.Inputs;
using InputDisplay.Theme;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class Game1 : Game
{
    readonly GraphicsDeviceManager graphics;

    SpriteBatch spriteBatch = null!;
    InputBuffer buffer = default!;

    readonly GameConfig config;

    PlayerIndex? player;
    string currentPad = string.Empty;

    SpriteFont font = null!;

    readonly ThemeCycle themeCycle = new();

    public Game1()
    {
        config = GameConfig.Load();
        graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Input Display";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
    }

    protected override void LoadContent()
    {
        spriteBatch = new(GraphicsDevice);
        font = Content.Load<SpriteFont>("fonts/numbers");

        ThemeManager.LoadContent(Content);
        themeCycle.StartWith(config.Theme);
        buffer = new(config, font);
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = config.Width;
        graphics.PreferredBackBufferHeight = config.Height;

        if (config.Top + config.Left > 0)
            Window.Position = new(config.Left, config.Top);

        graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (player is null)
        {
            DetectController();
            return;
        }

        var state = GamePad.GetState(player.Value);
        if (!state.IsConnected)
        {
            player = null;
            return;
        }

        buffer.Update(state, currentPad);

        HandleKeyboard();
        HandleMouse();
        UpdateTheme();
        UpdateConfig();
        base.Update(gameTime);
    }

    void UpdateConfig()
    {
        if (Window.Position.X != config.Left || Window.Position.Y != config.Top)
            config.UpdateWindowSize(Window);
    }

    void HandleKeyboard()
    {
        KeyboardManager.Update();

        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.Space))
        {
            config.InvertHistory = !config.InvertHistory;
            config.Save();
        }

        if (KeyboardManager.IsKeyDown(Keys.Delete))
        {
            currentPad = string.Empty;
            player = null;
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
        var newSize = MathHelper.Clamp(config.IconSize + step, 20, 100);
        config.IconSize = newSize;
    }

    void DetectController()
    {
        foreach (var i in Enum.GetValues<PlayerIndex>())
        {
            var state = GamePad.GetState(i);
            if (!state.IsConnected) continue;
            foreach (var button in Enum.GetValues<Buttons>())
            {
                if (!state.IsButtonDown(button)) continue;
                var caps = GamePad.GetCapabilities(i);
                player = i;
                currentPad = caps.Identifier;
                Console.WriteLine($"Selected: {caps.DisplayName}");

                if (!config.InputMap.Contains(caps.Identifier))
                {
                    config.InputMap.AddGamePad(caps);
                    config.Save();
                }

                var name = caps.DisplayName.ToLower();
                config.FallbackTheme =
                    !name.Contains("xbox") && name.Contains("ps")
                        ? ThemeManager.Get("PlayStation")
                        : ThemeManager.Get("XBOX");

                return;
            }
        }
    }

    void UpdateTheme()
    {
        var theme = config.Theme;
        if (KeyboardManager.IsKeyPressed(Keys.Up))
            config.Theme = themeCycle.NextStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Down))
            config.Theme = themeCycle.PrevStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Left))
            config.Theme = themeCycle.NextButtons();

        else if (KeyboardManager.IsKeyPressed(Keys.Right))
            config.Theme = themeCycle.PrevButtons();

        if (config.Theme != theme) config.Save();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(config.ClearColor);

        spriteBatch.Begin();

        if (player is null)
            spriteBatch.DrawString(font, "Press any button...", new(20), Color.White);
        else
            buffer.Draw(spriteBatch, Window.ClientBounds);

        spriteBatch.End();
        base.Draw(gameTime);
    }

    void OnResize(object? sender, EventArgs e) => config.UpdateWindowSize(Window);

    protected override void OnExiting(object sender, EventArgs args)
    {
        Window.ClientSizeChanged -= OnResize;
        base.OnExiting(sender, args);
    }
}
