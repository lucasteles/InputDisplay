using InputDisplay.Config;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Themes;

public class ThemeManager
{
    public const string DefaultButtons = "Street Fighter";
    public const string DefaultDirection = "default";

    public static Theme Get(string buttons = DefaultButtons, string direction = DefaultDirection) =>
        new()
        {
            Buttons = ThemeConfig.ButtonMap.GetValueOrDefault(buttons, ThemeConfig.ButtonMap[DefaultButtons]),
            Stick = ThemeConfig.DirectionMap.GetValueOrDefault(direction, ThemeConfig.DirectionMap[DefaultDirection]),
            ButtonsName = buttons,
            StickName = direction,
        };

    public static Theme Get(GameConfig.SelectedTheme selected) =>
        Get(selected.Buttons, selected.Direction);

    static readonly Dictionary<string, Texture2D> textures = [];

    public static void LoadContent(ContentManager content)
    {
        textures.Clear();
        var neutral = content.LoadTexture(Theme.Direction.DefaultNeutral);
        textures.Add(neutral.Name, neutral);
        var unknown = content.LoadTexture(Theme.FaceButtons.Unknown);
        textures.Add(unknown.Name, unknown);
        foreach (var dir in ThemeConfig.DirectionMap.Values)
            foreach (var texture in dir.LoadTextures(content))
                textures.Add(texture.Name, texture);

        foreach (var btn in ThemeConfig.ButtonMap.Values)
            foreach (var texture in btn.LoadTextures(content))
                textures.Add(texture.Name, texture);
    }

    public static Texture2D? GetTexture(string name) => textures.GetValueOrDefault(name);
    public static Texture2D UnknownButton => textures[Theme.FaceButtons.Unknown];

    readonly ThemeCycle cycle = new();
    Theme currentTheme;
    Theme? fallbackTheme;

    public Theme CurrentTheme
    {
        get => currentTheme;
        set
        {
            currentTheme = value;
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

    public ThemeManager(GameConfig.SelectedTheme theme) => currentTheme = Get(theme);

    public void SetFallback(string themeName) => FallbackTheme = Get(themeName);

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
}
