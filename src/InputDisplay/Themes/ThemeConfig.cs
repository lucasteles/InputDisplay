using InputDisplay.Inputs;

namespace InputDisplay.Themes;

public static class ThemeConfig
{
    public const string DefaultDirection = "default";
    public const string StreetFighter = "Street Fighter";
    public const string PlayStation = "PlayStation";
    public const string Xbox = "XBOX";

    public static Dictionary<string, Theme.Direction> DirectionMap { get; } =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [DefaultDirection] = new("arrows"),
            ["Small"] = new("arrows_sm"),
            ["Numpad"] = new("numpad", hasNeutral: true),
            ["Dpad"] = new("dpad", hasNeutral: true),
            ["Dpad2"] = new("dpad2", hasNeutral: true),
            ["Dpad3"] = new("dpad3", hasNeutral: true),
            ["Stick"] = new("stick", hasNeutral: true),
            ["Stick2"] = new("stick2", hasNeutral: true),
            ["Tekken"] = new("arrows_tekken", hasNeutral: true),
        };

    public static Dictionary<string, Theme.FaceButtons> ButtonMap { get; } =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [StreetFighter] = new()
            {
                Name = "sf",
                Textures = ForStreetFighter(),
                MacrosTemplate = DefaultTripleMacro(),
            },
            [PlayStation] = new()
            {
                Name = "ps",
                Textures = ForPlayStation(),
            },
            [Xbox] = new()
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
            ["SoulCalibur 6"] = new()
            {
                Name = "soulcalibur6",
                Textures = new()
                {
                    [ButtonName.LP] = "a",
                    [ButtonName.MP] = "b",
                    [ButtonName.LK] = "g",
                    [ButtonName.MK] = "k",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.KK] = [ButtonName.LP, ButtonName.LK],
                    [ButtonName.PP] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.HP] = [ButtonName.MP, ButtonName.LK],
                    [ButtonName.HK] = [ButtonName.LP, ButtonName.MP, ButtonName.MK],
                }
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

    public static readonly IReadOnlyDictionary<PlayerPad.Kind, string> ControllerTypes =
        new Dictionary<PlayerPad.Kind, string>
        {
            [PlayerPad.Kind.PlayStation] = PlayStation,
            [PlayerPad.Kind.Xbox] = Xbox,
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
