namespace InputDisplay.Theme;

public static class Themes
{
    public static Dictionary<string, ThemeManager.Direction> DirectionMap { get; } = new()
    {
        [ThemeManager.DefaultDirection] = new("arrows"),
        ["small"] = new("arrows_sm"),
        ["numpad"] = new("numpad", hasNeutral: true),
        ["dpad"] = new("dpad", hasNeutral: true),
        ["dpad2"] = new("dpad2", hasNeutral: true),
        ["dpad3"] = new("dpad3", hasNeutral: true),
        ["stick"] = new("stick", hasNeutral: true),
        ["stick2"] = new("stick2", hasNeutral: true),
        ["tekken"] = new("arrows_tekken", hasNeutral: true),
    };

    public static Dictionary<string, ThemeManager.Buttons> ButtonMap { get; } = new()
    {
        ["Street Fighter"] = new()
        {
            Name = "sf",
            Textures = ForStreetFighter(),
            MacrosTemplate = DefaultTripleMacro(),
        },
        ["PlayStation"] = new()
        {
            Name = "ps",
            Textures = ForPlayStation(),
        },
        ["XBOX"] = new()
        {
            Name = "xbox",
            Textures = ForXBox(),
        },
        ["MVC3"] = new()
        {
            Name = "mvc3",
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
            Name = "gg",
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
            Name = "kof",
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
            Name = "neogeo",
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
            Name = "neogeo2",
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
            Name = "numbers",
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
            Name = "tekken_xb",
            Textures = ForTekken(),
        },
        ["Tekken - PlayStation"] = new()
        {
            Name = "tekken_ps",
            Textures = ForTekken(),
        },
        ["Tekken - Steam"] = new()
        {
            Name = "tekken_steam",
            Textures = ForTekken(),
        },
        ["SF5"] = new()
        {
            Name = "sf5",
            Textures = ForStreetFighter(),
            MacrosTemplate = DefaultTripleMacro(),
        },
        ["SF4"] = new()
        {
            Name = "sf4",
            Textures = ForStreetFighter(),
            MacrosTemplate = DefaultTripleMacro(),
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

    static InputMacro DefaultTripleMacro() => new()
    {
        [ButtonName.PP] = [ButtonName.LP, ButtonName.MP, ButtonName.HP],
        [ButtonName.KK] = [ButtonName.LK, ButtonName.MK, ButtonName.HK],
    };
}
