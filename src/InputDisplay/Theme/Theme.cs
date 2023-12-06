using InputDisplay.Inputs;

namespace InputDisplay.Theme;

public class Theme
{
    public required ThemeManager.Buttons Buttons { get; set; }
    public required ThemeManager.Direction Stick { get; set; }
    public required Dictionary<ButtonName, ButtonName[]> InputMap { get; set; }

    public bool CustomMap { get; set; }

    public string GetTexturePath(GameInput.Direction state)
    {
        var name = state switch
        {
            GameInput.Direction.Neutral
                or (GameInput.Direction.Up | GameInput.Direction.Down)
                or (GameInput.Direction.Forward | GameInput.Direction.Backward) =>
                Stick.Neutral,
            GameInput.Direction.Up | GameInput.Direction.Forward => Stick.UpForward,
            GameInput.Direction.Up | GameInput.Direction.Backward => Stick.UpBackward,
            GameInput.Direction.Down | GameInput.Direction.Forward => Stick.DownForward,
            GameInput.Direction.Down | GameInput.Direction.Backward => Stick
                .DownBackward,
            GameInput.Direction.Down => Stick.Down,
            GameInput.Direction.Up => Stick.Up,
            GameInput.Direction.Forward => Stick.Forward,
            GameInput.Direction.Backward => Stick.Backward,
            _ => Stick.Neutral,
        };

        return string.IsNullOrWhiteSpace(name)
            ? ThemeManager.Direction.DefaultNeutral
            : new[]
            {
                Stick.Path, name,
            }.CombinePath();
    }

    public string? GetTexturePath(ButtonName btn)
    {
        if (Buttons.Textures.TryGetValue(btn, out var name))
            return new[]
            {
                Buttons.Path, name,
            }.CombinePath();

        return null;
    }

    public Texture2D? GetTexture(GameInput.Direction dir) =>
        ThemeManager.GetTexture(GetTexturePath(dir));

    public Texture2D? GetTexture(ButtonName btn) =>
        GetTexturePath(btn) is { } path ? ThemeManager.GetTexture(path) : null;

    public ButtonName[] GetMapped(ButtonName name) =>
        InputMap.TryGetValue(name, out var names) ? names : [name];

    public bool Known(GameInput.State state)
    {
        var bts = state.GetActiveButtons().ToArray();
        return bts.Length is 0 || Array.Exists(bts, b =>
            Buttons.Textures.ContainsKey(b)
            || InputMap.ContainsKey(b));
    }
}
