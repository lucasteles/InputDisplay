using InputDisplay.Inputs;

namespace InputDisplay.Theme;

public class Theme
{
    public required ThemeManager.Buttons Buttons { get; set; }
    public required ThemeManager.Direction Stick { get; set; }
    public required Dictionary<ButtonName, ButtonName[]> InputMap { get; set; }

    public bool CustomMap { get; set; }

    public Texture2D? GetTexture(GameInput.Direction dir) =>
        ThemeManager.GetTexture(Stick.GetTexturePath(dir));

    public Texture2D? GetTexture(ButtonName btn) =>
        Buttons.GetTexturePath(btn) is { } path ? ThemeManager.GetTexture(path) : null;

    public ButtonName[] GetMapped(ButtonName name) =>
        InputMap.TryGetValue(name, out var names) ? names : [name];
}
