using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

#pragma warning disable S4487

namespace InputDisplay.Config;

public partial class SettingsGame : Game
{
    SpriteBatch spriteBatch = default!;
    Desktop desktop = default!;

    readonly GraphicsDeviceManager graphics;

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
            Root = LoadWidgets(),
        };
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        desktop.Render();

        base.Draw(gameTime);
    }
}
