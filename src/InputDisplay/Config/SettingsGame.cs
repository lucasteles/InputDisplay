using InputDisplay.Inputs;
using InputDisplay.Themes;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

#pragma warning disable S4487

namespace InputDisplay.Config;

public partial class SettingsGame : Game
{
    SpriteBatch spriteBatch = default!;
    Desktop desktop = default!;
    SettingsWindow Controls = new();

    readonly GraphicsDeviceManager graphics;

    GameResources resources = default!;
    readonly GameConfigManager configManager = new();
    PlayerPad? player;

    Settings Config => configManager.CurrentConfig;

    public SettingsGame()
    {
        graphics = new(this)
        {
            PreferredBackBufferWidth = 1024,
            PreferredBackBufferHeight = 1024,
        };
        Window.AllowUserResizing = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.Title = "Input Display - Config";
        base.Initialize();
    }


    protected override void LoadContent()
    {
        spriteBatch = new(GraphicsDevice);

        MyraEnvironment.Game = this;
        desktop = new();

        desktop = new Desktop
        {
            Root = Controls.LoadWidgets(),
        };

        resources = new(Content);

        ThemeManager.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        if (KeyboardManager.IsKeyPressed(Keys.Escape))
            if (player is null || PlayerPad.GetConnected().Count() <= 1)
                Exit();
            else
                player = null;


        if (player is null)
        {
            DetectController();
            return;
        }

        base.Update(gameTime);
    }

    void DetectController()
    {
        if (PlayerPad.DetectPress() is not { } playerPad) return;
        player = playerPad;
        Controls.SelectedJoystick.Text = playerPad.Name;

        if (Config.InputMap.Contains(player.Identifier)) return;
        Config.InputMap.AddGamePad(player.Capabilities);
        configManager.Save();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin();

        if (player is null)
            spriteBatch.DrawString(resources.NumbersFont, "Press any button...", new(20), Color.White);
        else
            desktop.Render();

        spriteBatch.End();

        base.Draw(gameTime);
    }
}
