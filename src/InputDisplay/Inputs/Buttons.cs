namespace InputDisplay.Inputs;

[Flags]
public enum ButtonName : short
{
    None = 0,
    LP = 1 << 1,
    MP = 1 << 2,
    HP = 1 << 3,
    PP = 1 << 4,
    LK = 1 << 5,
    MK = 1 << 6,
    HK = 1 << 7,
    KK = 1 << 8,
}

[Flags]
public enum Direction : byte
{
    Neutral = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Forward = 1 << 2,
    Backward = 1 << 3,

    UpForward = Up | Forward,
    UpBackward = Up | Backward,
    DownForward = Down | Forward,
    DownBackward = Down | Backward,
}

public static class NumpadNotation
{
    public const int DownBackward = 1;
    public const int Down = 2;
    public const int DownForward = 3;
    public const int Backward = 4;
    public const int Neutral = 5;
    public const int Forward = 6;
    public const int UpBackward = 7;
    public const int Up = 8;
    public const int UpForward = 9;

    public static int From(Direction numpad) => numpad switch
    {
        Direction.Up => Up,
        Direction.Down => Down,
        Direction.Forward => Forward,
        Direction.Backward => Backward,
        Direction.UpForward => UpForward,
        Direction.UpBackward => UpBackward,
        Direction.DownForward => DownForward,
        Direction.DownBackward => DownBackward,
        Direction.Neutral => Neutral,
        _ => Neutral,
    };

    public static Direction ToDirection(int numpad) => numpad switch
    {
        Up => Direction.Up,
        Down => Direction.Down,
        Forward => Direction.Forward,
        Backward => Direction.Backward,
        UpForward => Direction.UpForward,
        UpBackward => Direction.UpBackward,
        DownBackward => Direction.DownBackward,
        DownForward => Direction.DownForward,
        _ => Direction.Neutral,
    };
}

public static class NumpadNotationString
{
    public static readonly string DownBackward = $"{NumpadNotation.DownBackward}";
    public static readonly string Down = NumpadNotation.Down.ToString();
    public static readonly string DownForward = NumpadNotation.DownForward.ToString();
    public static readonly string Backward = NumpadNotation.Backward.ToString();
    public static readonly string Neutral = NumpadNotation.Neutral.ToString();
    public static readonly string Forward = NumpadNotation.Forward.ToString();
    public static readonly string UpBackward = NumpadNotation.UpBackward.ToString();
    public static readonly string Up = NumpadNotation.Up.ToString();
    public static readonly string UpForward = NumpadNotation.UpForward.ToString();
}
