namespace InputDisplay.Theme;

public static class ThemeManager
{
    public class Direction(string path, bool neutral = false)
    {
        public const string DefaultNeutral = "n";

        public string Path { get; } = path;
        public string? Neutral { get; } = neutral ? "n" : null;
        public string Up { get; } = "u";
        public string Down { get; } = "d";
        public string Backward { get; } = "b";
        public string Forward { get; } = "f";
        public string UpForward { get; } = "uf";
        public string UpBackward { get; } = "ub";
        public string DownForward { get; } = "df";
        public string DownBackward { get; } = "db";

        public IEnumerable<Texture2D> LoadTextures(ContentManager content)
        {
            if (Neutral is not null)
                yield return content.LoadTexture(Path, Neutral);

            yield return content.LoadTexture(Path, Up);
            yield return content.LoadTexture(Path, Down);
            yield return content.LoadTexture(Path, Backward);
            yield return content.LoadTexture(Path, Forward);
            yield return content.LoadTexture(Path, UpForward);
            yield return content.LoadTexture(Path, UpBackward);
            yield return content.LoadTexture(Path, DownForward);
            yield return content.LoadTexture(Path, DownBackward);
        }
    }

    public class Buttons
    {
        public string Path { get; init; } = "";
        public InputMap InputTemplate { get; init; } = new();
        public required ButtonImage Textures { get; init; } = new();

        public IEnumerable<Texture2D> LoadTextures(ContentManager content)
        {
            foreach (var (_, name) in Textures)
                yield return content.LoadTexture(Path, name);
        }
    }

    public const string DefaultName = "default";

    public static Theme Get(string buttons, string direction = DefaultName) =>
        new()
        {
            Buttons = Themes.ButtonMap[buttons],
            Stick = Themes.DirectionMap[direction],
            InputMap = Themes.ButtonMap[buttons].InputTemplate,
        };

    static readonly Dictionary<string, Texture2D> textures = [];

    public static void LoadContent(ContentManager content)
    {
        textures.Clear();
        var neutral = content.LoadTexture(Direction.DefaultNeutral);
        textures.Add(neutral.Name, neutral);
        foreach (var dir in Themes.DirectionMap.Values)
            foreach (var texture in dir.LoadTextures(content))
                textures.Add(texture.Name, texture);

        foreach (var btn in Themes.ButtonMap.Values)
            foreach (var texture in btn.LoadTextures(content))
                textures.Add(texture.Name, texture);
    }

    public static Texture2D? GetTexture(string name) => textures.GetValueOrDefault(name);
}
