using InputDisplay.Inputs;
using InputDisplay.Themes;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

#pragma warning disable S4487

namespace InputDisplay.Config.Window;

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

    static readonly Point windowSize = new(1080, 720);

    public SettingsGame(string? playerIndex = null)
    {
        graphics = new(this);
        graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
        graphics.ApplyChanges();

        if (playerIndex.IsNonEmpty() && Enum.TryParse(playerIndex, out PlayerIndex index))
            playerIndexArg = index;
    }

    void OnPreparingDeviceSettings(object? sender, PreparingDeviceSettingsEventArgs e)
    {
        graphics.PreferMultiSampling = true;
        e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 16;
    }

    protected override void Initialize()
    {
        Window.Title = "Input Display - Config";
        graphics.PreferredBackBufferWidth = windowSize.X;
        graphics.PreferredBackBufferHeight = windowSize.Y;
        graphics.ApplyChanges();
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
            if (player is null || PlayerPad.GetConnected().Count() <= 1 || playerIndexArg is not null)
                Exit();
            else
                player = null;

        if (player is null)
        {
            DetectController();
            return;
        }

        player.Update();
        gameInput.Update(player, Config.InputMap, Config.EnabledDirections);

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
        if (playerIndexArg is not null && player is null)
        {
            PlayerPad argPlayer = new(playerIndexArg.Value);
            if (argPlayer.Capabilities.IsConnected)
            {
                SetPlayerPad(argPlayer);
                return;
            }

            player = null;
            playerIndexArg = null;
        }

        if (PlayerPad.DetectPress() is not { } playerPad) return;
        SetPlayerPad(playerPad);
    }

    public void SetPlayerPad(PlayerPad playerPad)
    {
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
            resources.Font.Draw(spriteBatch, "Press any button...", Color.White, Color.Black, new(20), 0.8f);
            spriteBatch.End();
        }
        else
            desktop.Render();

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        controls.Dispose();
        graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;
        controls.ResetMapButton.Click -= OnResetMap;
        base.UnloadContent();
    }
}
