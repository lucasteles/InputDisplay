using InputDisplay.Config;
using InputDisplay.Inputs;

namespace InputDisplay.Themes;

using StickDirection = Direction;

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

    public ButtonName[] GetMacro(ButtonName name, InputMacro? macros) =>
        macros?.TryGetValue(name, out var customMacro) is true
            ? customMacro
            : GetMacro(name);

    public static implicit operator Settings.SelectedTheme(Theme theme) =>
        new(theme.ButtonsName, theme.StickName);

    public class Direction(string name, bool hasNeutral = false)
    {
        public const string BasePath = @"themes\dir";
        public const string DefaultNeutral = @"themes\n";
        public bool Neutral { get; } = hasNeutral;
        public string Path { get; } = $"{BasePath}/{name}";

        public IEnumerable<(string, Lazy<Texture2D>)> GetTextures(ContentManager content)
        {
            if (Neutral)
                yield return content.LazyLoadTexture(Path, NumpadNotationStr.Neutral);

            yield return content.LazyLoadTexture(Path, NumpadNotationStr.Up);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.Down);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.Backward);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.Forward);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.UpForward);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.UpBackward);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.DownForward);
            yield return content.LazyLoadTexture(Path, NumpadNotationStr.DownBackward);
        }

        public string GetTexturePath(StickDirection dir)
        {
            var dirName = dir switch
            {
                StickDirection.Neutral => Neutral ? NumpadNotationStr.Neutral : null,
                StickDirection.UpForward => NumpadNotationStr.UpForward,
                StickDirection.UpBackward => NumpadNotationStr.UpBackward,
                StickDirection.DownForward => NumpadNotationStr.DownForward,
                StickDirection.DownBackward => NumpadNotationStr.DownBackward,
                StickDirection.Down => NumpadNotationStr.Down,
                StickDirection.Up => NumpadNotationStr.Up,
                StickDirection.Forward => NumpadNotationStr.Forward,
                StickDirection.Backward => NumpadNotationStr.Backward,
                _ => null,
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
        public bool IsPad { get; init; } = false;
        public string Path => ContentPath.Combine(BasePath, Name);
        public InputMacro MacrosTemplate { get; init; } = new();
        public required ButtonImage Textures { get; init; } = new();

        public bool HasEmptyImage => Textures.ContainsKey(ButtonName.None);

        public IEnumerable<(string, Lazy<Texture2D>)> GetTextures(ContentManager content)
        {
            foreach (var (_, name) in Textures)
                yield return content.LazyLoadTexture(Path, name);
        }

        public string? GetTexturePath(ButtonName btn) =>
            Textures.TryGetValue(btn, out var name)
                ? ContentPath.Combine(Path, name)
                : null;
    }
}
