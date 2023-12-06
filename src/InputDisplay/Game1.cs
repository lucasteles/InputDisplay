using InputDisplay.Inputs;
using InputDisplay.Theme;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class Game1 : Game
{
    readonly GraphicsDeviceManager graphics;

    SpriteBatch spriteBatch = null!;
    InputBuffer buffer = default!;

    readonly ThemeCycle themeCycle = new();

    readonly InputConfig config = new()
    {
        ButtonIconSize = 40,
        SpaceBetweenInputs = 2,
        SpaceBetweenCommands = 4,
        Theme = null!,
        AutoCorrectMultiple = true,
        MaxEntries = 100,
        ShadowHolding = true,
        HideButtonRelease = false,
        InvertHistory = false,
        Horizontal = false,
    };

    public Game1()
    {
        graphics = new(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
        Window.Title = "Input Display";
    }

    protected override void LoadContent()
    {
        spriteBatch = new(GraphicsDevice);
        ThemeManager.LoadContent(Content);

#pragma warning disable S125
        config.Theme = ThemeManager.Get("Street Fighter");
        // config.Theme = ThemeManager.Get("Tekken - XBox", "dpad");
        // config.Theme = ThemeManager.Get("Guilty Gear", "small");
#pragma warning restore S125

        themeCycle.StartWith(config.Theme);
        buffer = new(config, Content.Load<SpriteFont>("numbers"));
        config.ConfigureForWindow(Window);
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = 480;
        graphics.PreferredBackBufferHeight = 1024;
        graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        var state = GamePad.GetState(PlayerIndex.One);

        buffer.Update(state);
        KeyboardManager.Update();

        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.Space))
            config.InvertHistory = !config.InvertHistory;

        UpdateTheme();

        base.Update(gameTime);
    }

    void UpdateTheme()
    {
        if (KeyboardManager.IsKeyPressed(Keys.Up))
            config.Theme = themeCycle.NextStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Down))
            config.Theme = themeCycle.PrevStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Left))
            config.Theme = themeCycle.NextButtons();

        else if (KeyboardManager.IsKeyPressed(Keys.Right))
            config.Theme = themeCycle.PrevButtons();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(config.BackgroundColor);

        spriteBatch.Begin();

        buffer.Draw(spriteBatch, Window.ClientBounds);

        spriteBatch.End();
        base.Draw(gameTime);
    }

    void OnResize(object? sender, EventArgs e) => config.ConfigureForWindow(Window);

    protected override void OnExiting(object sender, EventArgs args)
    {
        Window.ClientSizeChanged -= OnResize;
        base.OnExiting(sender, args);
    }
}
