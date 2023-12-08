using InputDisplay.Config;

namespace InputDisplay.Theme;

public static class ThemeManager
{
    public const string DefaultButtons = "Street Fighter";
    public const string DefaultDirection = "default";

    public static Theme Get(string buttons, string direction = DefaultDirection) =>
        new()
        {
            Buttons = Themes.ButtonMap.GetValueOrDefault(buttons, Themes.ButtonMap[DefaultButtons]),
            Stick = Themes.DirectionMap.GetValueOrDefault(direction, Themes.DirectionMap[DefaultDirection]),
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
        foreach (var dir in Themes.DirectionMap.Values)
            foreach (var texture in dir.LoadTextures(content))
                textures.Add(texture.Name, texture);

        foreach (var btn in Themes.ButtonMap.Values)
            foreach (var texture in btn.LoadTextures(content))
                textures.Add(texture.Name, texture);
    }

    public static Texture2D? GetTexture(string name) => textures.GetValueOrDefault(name);
    public static Texture2D UnknownButton => textures[Theme.FaceButtons.Unknown];
}
