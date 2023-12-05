using InputDisplay.Data;

namespace InputDisplay.Theme;

public class ThemeCycle
{
    readonly Cycle<ThemeManager.Direction> stick = new(Themes.DirectionMap.Values);
    readonly Cycle<ThemeManager.Buttons> buttons = new(Themes.ButtonMap.Values);

    public void StartWith(Theme theme)
    {
        if (Themes.DirectionMap.Values.ToList().IndexOf(theme.Stick) is not -1 and var s)
            stick.SetIndex(s);

        if (Themes.ButtonMap.Values.ToList().IndexOf(theme.Buttons) is not -1 and var b)
            buttons.SetIndex(b);
    }

    public Theme NextStick() => new()
    {
        Stick = stick.Next(),
        Buttons = buttons.Current,
        InputMap = buttons.Current.InputTemplate,
    };

    public Theme PrevStick() => new()
    {
        Stick = stick.Previous(),
        Buttons = buttons.Current,
        InputMap = buttons.Current.InputTemplate,
    };

    public Theme NextButtons()
    {
        var next = buttons.Next();
        return new()
        {
            Stick = stick.Current,
            Buttons = next,
            InputMap = next.InputTemplate,
        };
    }

    public Theme PrevButtons()
    {
        var next = buttons.Previous();
        return new()
        {
            Stick = stick.Current,
            Buttons = next,
            InputMap = next.InputTemplate,
        };
    }
}
