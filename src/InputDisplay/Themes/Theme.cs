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
                yield return content.LazyLoadTexture(Path, NumpadNotationString.Neutral);

            yield return content.LazyLoadTexture(Path, NumpadNotationString.Up);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.Down);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.Backward);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.Forward);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.UpForward);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.UpBackward);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.DownForward);
            yield return content.LazyLoadTexture(Path, NumpadNotationString.DownBackward);
        }

        public string GetTexturePath(StickDirection dir)
        {
            var dirName = dir switch
            {
                StickDirection.Neutral => Neutral ? NumpadNotationString.Neutral : null,
                StickDirection.UpForward => NumpadNotationString.UpForward,
                StickDirection.UpBackward => NumpadNotationString.UpBackward,
                StickDirection.DownForward => NumpadNotationString.DownForward,
                StickDirection.DownBackward => NumpadNotationString.DownBackward,
                StickDirection.Down => NumpadNotationString.Down,
                StickDirection.Up => NumpadNotationString.Up,
                StickDirection.Forward => NumpadNotationString.Forward,
                StickDirection.Backward => NumpadNotationString.Backward,
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
        public InputMacro MacrosTemplate { get; init; } = [];
        public required ButtonImage Textures { get; init; } = [];

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
