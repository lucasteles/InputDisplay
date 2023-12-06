namespace InputDisplay.Theme;

public static class Themes
{
    public static Dictionary<string, ThemeManager.Direction> DirectionMap { get; } = new()
    {
        [ThemeManager.DefaultName] = new("stick/arrows"),
        ["small"] = new("stick/arrows_sm"),
        ["dpad"] = new("stick/dpad", neutral: true),
        ["dpad2"] = new("stick/dpad2", neutral: true),
        ["dpad3"] = new("stick/dpad3", neutral: true),
        ["stick"] = new("stick/stick", neutral: true),
        ["stick2"] = new("stick/stick2", neutral: true),
        ["tekken"] = new("stick/arrows_tekken", neutral: true),
    };

    public static Dictionary<string, ThemeManager.Buttons> ButtonMap { get; } = new()
    {
        ["Street Fighter"] = new()
        {
            Path = "sf",
            Textures = ForStreetFighter(),
            InputTemplate = DefaultTripleMacro(),
        },
        ["PlayStation"] = new()
        {
            Path = "ps",
            Textures = ForPlayStation(),
        },
        ["XBOX"] = new()
        {
            Path = "xbox",
            Textures = ForXBox(),
        },
        ["MVC3"] = new()
        {
            Path = "mvc3",
            Textures = new()
            {
                [ButtonName.LP] = "l",
                [ButtonName.MP] = "m",
                [ButtonName.MK] = "h",
                [ButtonName.LK] = "s",
                [ButtonName.HP] = "a1",
                [ButtonName.HK] = "a2",
            },
        },
        ["Guilty Gear"] = new()
        {
            Path = "gg",
            Textures = new()
            {
                [ButtonName.LP] = "p",
                [ButtonName.MP] = "s",
                [ButtonName.HP] = "d",
                [ButtonName.LK] = "k",
                [ButtonName.MK] = "hs",
                [ButtonName.HK] = "dash",
            },
        },
        ["KOF"] = new()
        {
            Path = "kof",
            Textures = new()
            {
                [ButtonName.LP] = "lp",
                [ButtonName.MP] = "hp",
                [ButtonName.LK] = "lk",
                [ButtonName.MK] = "hk",
            },
        },
        ["NeoGeo"] = new()
        {
            Path = "neogeo",
            Textures = new()
            {
                [ButtonName.LP] = "a",
                [ButtonName.MP] = "b",
                [ButtonName.LK] = "c",
                [ButtonName.MK] = "d",
            },
        },
        ["NeoGeo Clear"] = new()
        {
            Path = "neogeo2",
            Textures = new()
            {
                [ButtonName.LP] = "a",
                [ButtonName.MP] = "b",
                [ButtonName.LK] = "c",
                [ButtonName.MK] = "d",
            },
        },
        ["Numbers"] = new()
        {
            Path = "numbers",
            Textures = new()
            {
                [ButtonName.LP] = "1",
                [ButtonName.MP] = "2",
                [ButtonName.LK] = "3",
                [ButtonName.MK] = "4",
            },
        },
        ["Tekken - XBox"] = new()
        {
            Path = "tekken_xb",
            Textures = ForTekken(),
        },
        ["Tekken - PlayStation"] = new()
        {
            Path = "tekken_ps",
            Textures = ForTekken(),
        },
        ["Tekken - Steam"] = new()
        {
            Path = "tekken_steam",
            Textures = ForTekken(),
        },
        ["SF5"] = new()
        {
            Path = "sf5",
            Textures = ForStreetFighter(),
            InputTemplate = DefaultTripleMacro(),
        },
        ["SF4"] = new()
        {
            Path = "sf4",
            Textures = ForStreetFighter(),
            InputTemplate = DefaultTripleMacro(),
        },
    };

    static ButtonImage ForTekken() => new()
    {
        [ButtonName.LP] = "1",
        [ButtonName.MP] = "2",
        [ButtonName.LK] = "3",
        [ButtonName.MK] = "4",
        [ButtonName.LP | ButtonName.MP] = "1_2",
        [ButtonName.LP | ButtonName.LK] = "1_3",
        [ButtonName.LP | ButtonName.MK] = "1_4",
        [ButtonName.MP | ButtonName.LK] = "2_3",
        [ButtonName.MP | ButtonName.MK] = "2_4",
        [ButtonName.LK | ButtonName.MK] = "3_4",
        [ButtonName.LP | ButtonName.MP | ButtonName.LK] = "1_2_3",
        [ButtonName.LP | ButtonName.MP | ButtonName.MK] = "1_2_4",
        [ButtonName.MP | ButtonName.LK | ButtonName.MK] = "2_3_4",
        [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.MK] = "1_2_3_4",
    };

    static ButtonImage ForStreetFighter() => new()
    {
        [ButtonName.LP] = "lp",
        [ButtonName.MP] = "mp",
        [ButtonName.HP] = "hp",
        [ButtonName.LK] = "lk",
        [ButtonName.MK] = "mk",
        [ButtonName.HK] = "hk",
    };

    static ButtonImage ForPlayStation() => new()
    {
        [ButtonName.LP] = "square",
        [ButtonName.MP] = "triangle",
        [ButtonName.HP] = "r1",
        [ButtonName.PP] = "l1",
        [ButtonName.LK] = "x",
        [ButtonName.MK] = "circle",
        [ButtonName.HK] = "r2",
        [ButtonName.KK] = "l2",
    };

    static ButtonImage ForXBox() => new()
    {
        [ButtonName.LP] = "x",
        [ButtonName.MP] = "y",
        [ButtonName.HP] = "rb",
        [ButtonName.PP] = "lb",
        [ButtonName.LK] = "a",
        [ButtonName.MK] = "b",
        [ButtonName.HK] = "rt",
        [ButtonName.KK] = "lt",
    };

    static InputMap DefaultTripleMacro() => new()
    {
        [ButtonName.PP] = [ButtonName.LP, ButtonName.MP, ButtonName.HP],
        [ButtonName.KK] = [ButtonName.LK, ButtonName.MK, ButtonName.HK],
    };
}
