using InputDisplay.Inputs;
using InputDisplay.Themes;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

#pragma warning disable S4487

namespace InputDisplay.Config.Screen;

public class SettingsGame : Game
{
    SpriteBatch spriteBatch = default!;
    Desktop desktop = default!;
    SettingsControls controls = default!;
    GameResources resources = default!;

    readonly GameInput gameInput = new();

#pragma warning disable S1450
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    readonly GraphicsDeviceManager graphics;
#pragma warning restore S1450

    readonly SettingsManager configManager = new();
    PlayerPad? player;

    Settings Config => configManager.CurrentConfig;

    PlayerIndex? playerIndexArg;

    public SettingsGame(string? playerIndex = null)
    {
        graphics = new(this)
        {
            PreferredBackBufferWidth = 1080,
            PreferredBackBufferHeight = 720,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
        graphics.ApplyChanges();

        if (playerIndex.IsNonEmpty() && Enum.TryParse(playerIndex, out PlayerIndex index))
            playerIndexArg = index;
    }

    protected override void Initialize()
    {
        Window.Title = "Input Display - Config";
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new(GraphicsDevice);
        ThemeManager.LoadContent(Content);

        MyraEnvironment.Game = this;
        desktop = new();
        controls = new(desktop, configManager);
        desktop.Root = controls.BuildUI();

        controls.ResetMapButton.Click += OnResetMap;
        resources = new(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardManager.Update();

        if (controls.MappingButton is null && controls.MappingMacro is null &&
            KeyboardManager.IsKeyPressed(Keys.Escape))
            if (player is null || PlayerPad.GetConnected().Count() <= 1)
                Exit();
            else
                player = null;

        if (player is null)
        {
            DetectController();
            return;
        }

        player.Update();
        gameInput.Update(player, Config.InputMap);

        if (controls.MappingButton is null)
        {
            var input = gameInput.CurrentState;
            controls.HighLightDirection(input.Stick.Direction);
            controls.HighLightButtons(input.GetActiveButtons());
        }
        else if (player.GetAnyButton() is { } padButton && Config.InputMap.GetMapping(player.Identifier) is { } map)
        {
            map.Set(controls.MappingButton.Value, padButton);
            controls.ButtonMapped();
            configManager.SaveFile();
        }

        base.Update(gameTime);
    }

    void DetectController()
    {
        if (playerIndexArg is not null)
        {
            player = new(playerIndexArg.Value);

            if (!player.IsConnected)
            {
                player = null;
                playerIndexArg = null;
            }
            else
                return;
        }

        if (PlayerPad.DetectPress() is not { } playerPad) return;
        player = playerPad;
        controls.SetPlayer(playerPad);

        if (Config.InputMap.TryAddGamePad(player.Capabilities))
            configManager.SaveFile();
    }

    void OnResetMap(object? sender, EventArgs e)
    {
        if (player is null || Config.InputMap.GetMapping(player.Identifier) is not { } map) return;
        map.Reset();
        configManager.SaveFile();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        if (player is null)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(resources.NumbersFont, "Press any button...", new(20), Color.White);
            spriteBatch.End();
        }
        else
            desktop.Render();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        controls.Dispose();
        controls.ResetMapButton.Click -= OnResetMap;
        base.UnloadContent();
    }
}
