using InputDisplay.Config;
using InputDisplay.Inputs;

namespace InputDisplay.Themes;

public record Theme
{
    public required string ButtonsName { get; set; }
    public required string StickName { get; set; }
    public required FaceButtons Buttons { get; set; }
    public required Direction Stick { get; set; }
    public Theme? Fallback { get; set; }

    public Texture2D? GetTexture(StickDirection dir) =>
        ThemeManager.GetTexture(Stick.GetTexturePath(dir));

    public Texture2D? GetTexture(ButtonName btn) =>
        Buttons.GetTexturePath(btn) is { } path ? ThemeManager.GetTexture(path) : null;

    public ButtonName[] GetMacro(ButtonName name) =>
        Buttons.MacrosTemplate.TryGetValue(name, out var names) ? names : [name];

    public ButtonName[] GetMacro(ButtonName name, InputMacro macros) =>
        macros.TryGetValue(name, out var customMacro) ? customMacro : GetMacro(name);

    public static implicit operator GameConfig.SelectedTheme(Theme theme) =>
        new(theme.ButtonsName, theme.StickName);

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

        public string GetTexturePath(StickDirection dir)
        {
            var dirName = dir switch
            {
                StickDirection.Neutral
                    or (StickDirection.Up | StickDirection.Down)
                    or (StickDirection.Forward | StickDirection.Backward) =>
                    Neutral ? NumpadNotation.Neutral : null,
                StickDirection.Up | StickDirection.Forward => NumpadNotation.UpForward,
                StickDirection.Up | StickDirection.Backward => NumpadNotation.UpBackward,
                StickDirection.Down | StickDirection.Forward => NumpadNotation.DownForward,
                StickDirection.Down | StickDirection.Backward => NumpadNotation.DownBackward,
                StickDirection.Down => NumpadNotation.Down,
                StickDirection.Up => NumpadNotation.Up,
                StickDirection.Forward => NumpadNotation.Forward,
                StickDirection.Backward => NumpadNotation.Backward,
                _ => Neutral ? NumpadNotation.Neutral : null,
            };

            return string.IsNullOrWhiteSpace(dirName)
                ? DefaultNeutral
                : ContentPath.Combine(Path, dirName);
        }
    }

    public class FaceButtons
    {
        const string BasePath = @"themes\btn";
        public const string Unknown = @"themes\unknown";

        public string Name { get; init; } = "";
        public string Path => ContentPath.Combine(BasePath, Name);
        public InputMacro MacrosTemplate { get; init; } = new();
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
}
