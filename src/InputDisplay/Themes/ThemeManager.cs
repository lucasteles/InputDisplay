using InputDisplay.Config;
using InputDisplay.Inputs;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Themes;

public class ThemeManager(Settings.SelectedTheme theme)
{
    public const string DefaultButtons = ThemeConfig.StreetFighter;
    public const string DefaultDirection = ThemeConfig.DefaultDirection;

    static readonly Dictionary<string, Lazy<Texture2D>> textures = [];
    readonly ThemeCycle cycle = new();
    Theme currentTheme = Get(theme);
    Theme? fallbackTheme;

    public static Texture2D? GetTexture(string name) => textures.GetValueOrDefault(name)?.Value;
    public static Texture2D UnknownButton => textures[Theme.FaceButtons.Unknown].Value;

    public Theme CurrentTheme
    {
        get => currentTheme;
        set
        {
            currentTheme = value;
            currentTheme.Fallback = fallbackTheme;
            cycle.StartAt(CurrentTheme);
        }
    }

    public Theme? FallbackTheme
    {
        get => fallbackTheme;
        set
        {
            fallbackTheme = value;
            currentTheme.Fallback = fallbackTheme;
        }
    }

    public void SetFallback(string themeName) => FallbackTheme = Get(themeName);

    public void SetFallback(PlayerPad.Kind kind)
    {
        var themeName =
            ThemeConfig.ControllerTypes.GetValueOrDefault(kind, ThemeConfig.Xbox);

        if (FallbackTheme?.ButtonsName == themeName)
            return;

        SetFallback(themeName);
    }

    public bool Update()
    {
        var previousTheme = CurrentTheme;
        if (KeyboardManager.IsKeyPressed(Keys.Up))
            CurrentTheme = cycle.NextStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Down))
            CurrentTheme = cycle.PrevStick();

        else if (KeyboardManager.IsKeyPressed(Keys.Left))
            CurrentTheme = cycle.NextButtons();

        else if (KeyboardManager.IsKeyPressed(Keys.Right))
            CurrentTheme = cycle.PrevButtons();


        if (previousTheme == CurrentTheme)
            return false;

        CurrentTheme.Fallback = FallbackTheme;
        return true;
    }

    public static void LoadContent(ContentManager content)
    {
        if (textures.Count > 0) return;

        var neutral = content.LoadTexture(Theme.Direction.DefaultNeutral);
        textures.Add(neutral.Name, new(() => neutral));
        var unknown = content.LoadTexture(Theme.FaceButtons.Unknown);
        textures.Add(unknown.Name, new(() => unknown));

        foreach (var dir in ThemeConfig.DirectionMap.Values)
            foreach (var (name, texture) in dir.GetTextures(content))
                textures.Add(name, texture);

        foreach (var btn in ThemeConfig.ButtonMap.Values)
            foreach (var (name, texture) in btn.GetTextures(content))
                textures.Add(name, texture);
    }

    public static Theme Get(string buttons = DefaultButtons, string direction = DefaultDirection) =>
        new()
        {
            Buttons = ThemeConfig.ButtonMap.GetValueOrDefault(buttons, ThemeConfig.ButtonMap[DefaultButtons]),
            Stick = ThemeConfig.DirectionMap.GetValueOrDefault(direction, ThemeConfig.DirectionMap[DefaultDirection]),
            ButtonsName = buttons,
            StickName = direction,
        };

    public static Theme Get(Settings.SelectedTheme selected) =>
        Get(selected.Buttons, selected.Direction);
}
