
namespace InputDisplay.Themes;

public class ThemeCycle
{
    readonly Cycle<KeyValuePair<string, Theme.Direction>> stick = new(ThemeConfig.DirectionMap);
    readonly Cycle<KeyValuePair<string, Theme.FaceButtons>> buttons = new(ThemeConfig.ButtonMap);

    public void StartAt(Theme theme)
    {
        if (ThemeConfig.DirectionMap.Values.ToList().IndexOf(theme.Stick) is not -1 and var s)
            stick.SetIndex(s);

        if (ThemeConfig.ButtonMap.Values.ToList().IndexOf(theme.Buttons) is not -1 and var b)
            buttons.SetIndex(b);
    }

    public Theme NextStick() => new()
    {
        Stick = stick.Next().Value,
        StickName = stick.Current.Key,
        Buttons = buttons.Current.Value,
        ButtonsName = buttons.Current.Key,
    };

    public Theme PrevStick() => new()
    {
        Stick = stick.Previous().Value,
        StickName = stick.Current.Key,
        Buttons = buttons.Current.Value,
        ButtonsName = buttons.Current.Key,
    };

    public Theme NextButtons()
    {
        var next = buttons.Next();
        return new()
        {
            Stick = stick.Current.Value,
            StickName = stick.Current.Key,
            Buttons = next.Value,
            ButtonsName = next.Key,
        };
    }

    public Theme PrevButtons()
    {
        var next = buttons.Previous();
        return new()
        {
            Stick = stick.Current.Value,
            StickName = stick.Current.Key,
            Buttons = next.Value,
            ButtonsName = next.Key,
        };
    }
}
