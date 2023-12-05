namespace InputDisplay.Theme;

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
