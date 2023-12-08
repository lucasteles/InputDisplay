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
}

public static class NumpadNotation
{
    public const string DownBackward = "1";
    public const string Down = "2";
    public const string DownForward = "3";
    public const string Backward = "4";
    public const string Neutral = "5";
    public const string Forward = "6";
    public const string UpBackward = "7";
    public const string Up = "8";
    public const string UpForward = "9";
}
