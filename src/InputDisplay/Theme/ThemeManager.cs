using InputDisplay.Inputs;

namespace InputDisplay.Theme;

public static class ThemeManager
{
    public class Direction(string name, bool hasNeutral = false)
    {
        const string BasePath = @"themes\dir";
        public const string DefaultNeutral = @"themes\n";
        public bool Neutral { get; } = hasNeutral;
        public string Path { get; } = $"{BasePath}/{name}";

        public IEnumerable<Texture2D> LoadTextures(ContentManager content)
        {
            if (Neutral)
                yield return content.LoadTexture(Path, NumpadNotation.Neutral);

            yield return content.LoadTexture(Path, NumpadNotation.Up);
            yield return content.LoadTexture(Path, NumpadNotation.Down);
            yield return content.LoadTexture(Path, NumpadNotation.Backward);
            yield return content.LoadTexture(Path, NumpadNotation.Forward);
            yield return content.LoadTexture(Path, NumpadNotation.UpForward);
            yield return content.LoadTexture(Path, NumpadNotation.UpBackward);
            yield return content.LoadTexture(Path, NumpadNotation.DownForward);
            yield return content.LoadTexture(Path, NumpadNotation.DownBackward);
        }

        public string GetTexturePath(GameInput.Direction dir)
        {
            var dirName = dir switch
            {
                GameInput.Direction.Neutral
                    or (GameInput.Direction.Up | GameInput.Direction.Down)
                    or (GameInput.Direction.Forward | GameInput.Direction.Backward) =>
                    Neutral ? NumpadNotation.Neutral : null,
                GameInput.Direction.Up | GameInput.Direction.Forward => NumpadNotation.UpForward,
                GameInput.Direction.Up | GameInput.Direction.Backward => NumpadNotation.UpBackward,
                GameInput.Direction.Down | GameInput.Direction.Forward => NumpadNotation.DownForward,
                GameInput.Direction.Down | GameInput.Direction.Backward => NumpadNotation.DownBackward,
                GameInput.Direction.Down => NumpadNotation.Down,
                GameInput.Direction.Up => NumpadNotation.Up,
                GameInput.Direction.Forward => NumpadNotation.Forward,
                GameInput.Direction.Backward => NumpadNotation.Backward,
                _ => Neutral ? NumpadNotation.Neutral : null,
            };

            return string.IsNullOrWhiteSpace(dirName)
                ? DefaultNeutral
                : ContentPath.Combine(Path, dirName);
        }
    }

    public class Buttons
    {
        const string BasePath = @"themes\btn";
        public const string Unknown = @"themes\unknown";

        public string Name { get; init; } = "";
        public string Path => ContentPath.Combine(BasePath, Name);
        public InputMacro Macros { get; init; } = new();
        public required ButtonImage Textures { get; init; } = new();

        public IEnumerable<Texture2D> LoadTextures(ContentManager content)
        {
            foreach (var (_, name) in Textures)
                yield return content.LoadTexture(Path, name);
        }

        public string? GetTexturePath(ButtonName btn) =>
            Textures.TryGetValue(btn, out var name)
                ? ContentPath.Combine(Path, name)
                : null;
    }


    public const string DefaultButtons = "Street Fighter";
    public const string DefaultDirection = "default";

    public static Theme Get(string buttons, string direction = DefaultDirection) =>
        new()
        {
            Buttons = Themes.ButtonMap[buttons],
            Stick = Themes.DirectionMap[direction],
            ButtonsName = buttons,
            StickName = direction,
        };

    public static Theme Get(GameConfig.SelectedTheme selected) =>
        Get(selected.Buttons, selected.Direction);

    static readonly Dictionary<string, Texture2D> textures = [];

    public static void LoadContent(ContentManager content)
    {
        textures.Clear();
        var neutral = content.LoadTexture(Direction.DefaultNeutral);
        textures.Add(neutral.Name, neutral);
        var unknown = content.LoadTexture(Buttons.Unknown);
        textures.Add(unknown.Name, unknown);
        foreach (var dir in Themes.DirectionMap.Values)
            foreach (var texture in dir.LoadTextures(content))
                textures.Add(texture.Name, texture);

        foreach (var btn in Themes.ButtonMap.Values)
            foreach (var texture in btn.LoadTextures(content))
                textures.Add(texture.Name, texture);
    }

    public static Texture2D? GetTexture(string name) => textures.GetValueOrDefault(name);
    public static Texture2D UnknownButton => textures[Buttons.Unknown];
}
