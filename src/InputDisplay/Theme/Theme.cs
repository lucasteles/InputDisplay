using InputDisplay.Inputs;

namespace InputDisplay.Theme;

public record Theme
{
    public required string ButtonsName { get; set; }
    public required string StickName { get; set; }
    public required ThemeManager.Buttons Buttons { get; set; }
    public required ThemeManager.Direction Stick { get; set; }

    public Texture2D? GetTexture(GameInput.Direction dir) =>
        ThemeManager.GetTexture(Stick.GetTexturePath(dir));

    public Texture2D? GetTexture(ButtonName btn) =>
        Buttons.GetTexturePath(btn) is { } path ? ThemeManager.GetTexture(path) : null;

    public ButtonName[] GetMacro(ButtonName name) =>
        Buttons.MacrosTemplate.TryGetValue(name, out var names) ? names : [name];
}
