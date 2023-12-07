using InputDisplay.Inputs;
using InputDisplay.Theme;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay;

public class Game1 : Game
{
    readonly GraphicsDeviceManager graphics;

    SpriteBatch spriteBatch = null!;
    InputBuffer buffer = default!;

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

    PlayerIndex? player;
    SpriteFont font = null!;
    readonly ThemeCycle themeCycle = new();

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

        config.FallbackTheme = ThemeManager.Get("XBOX");
        config.Theme = ThemeManager.Get("Street Fighter");

        themeCycle.StartWith(config.Theme);
        font = Content.Load<SpriteFont>("fonts/numbers");
        buffer = new(config, font);
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

        buffer.Update(state);
        KeyboardManager.Update();

        if (KeyboardManager.IsKeyDown(Keys.Escape))
            Exit();

        if (KeyboardManager.IsKeyPressed(Keys.Space))
            config.InvertHistory = !config.InvertHistory;

        if (KeyboardManager.IsKeyDown(Keys.Back) || KeyboardManager.IsKeyDown(Keys.Delete))
            buffer.Clear();

        UpdateTheme();

        base.Update(gameTime);
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
                player = i;

                var caps = GamePad.GetCapabilities(i);
                Console.WriteLine($"Selected: {caps.DisplayName}");

                return;
            }
        }
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

        if (player is null)
            spriteBatch.DrawString(font, "Press any button...", new(20), Color.White);
        else
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
