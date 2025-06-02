using InputDisplay.Inputs;

namespace InputDisplay.Themes;

public static class ThemeConfig
{
    public const string DefaultDirection = "default";
    public const string StreetFighter = "Street Fighter 6";
    public const string PlayStation = "PlayStation";
    public const string Xbox = "XBOX";
    public const string Nintendo = "Switch";

    public static Dictionary<string, Theme.Direction> DirectionMap { get; } =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [DefaultDirection] = new("arrows"),
            ["Arrows"] = new("arrows2", hasNeutral: true),
            ["Small"] = new("arrows_sm"),
            ["Numpad"] = new("numpad", hasNeutral: true),
            ["Dpad"] = new("dpad", hasNeutral: true),
            ["Dpad2"] = new("dpad2", hasNeutral: true),
            ["Dpad3"] = new("dpad3", hasNeutral: true),
            ["KOF"] = new("kof", hasNeutral: true),
            ["Stick"] = new("stick", hasNeutral: true),
            ["Stick2"] = new("stick2", hasNeutral: true),
            ["Tekken"] = new("arrows_tekken", hasNeutral: true),
        };

    public static Dictionary<string, Theme.FaceButtons> ButtonMap { get; } =
        new(StringComparer.InvariantCultureIgnoreCase)
        {
            [StreetFighter] = new()
            {
                Name = "sf6",
                Textures = ForStreetFighter(),
                MacrosTemplate = new()
                {
                    [ButtonName.PP] = [ButtonName.HP, ButtonName.HK],
                    [ButtonName.KK] = [ButtonName.MP, ButtonName.MK],
                },
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
                    [ButtonName.PP] = "a1",
                    [ButtonName.HP] = "a2",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.KK] = [ButtonName.PP, ButtonName.HP],
                    [ButtonName.HK] = [ButtonName.LP, ButtonName.MP, ButtonName.MK],
                },
            },
            ["MVCI"] = new()
            {
                Name = "mvci",
                Textures = new()
                {
                    [ButtonName.LP] = "lp",
                    [ButtonName.MP] = "hp",
                    [ButtonName.LK] = "lk",
                    [ButtonName.MK] = "hk",
                    [ButtonName.HP] = "switch",
                    [ButtonName.PP] = "surge",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.KK] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.HK] = [ButtonName.LK, ButtonName.MK],
                },
            },
            ["2XKO"] = new()
            {
                Name = "2xko",
                Textures = new()
                {
                    [ButtonName.LP] = "l",
                    [ButtonName.MP] = "m",
                    [ButtonName.HP] = "h",
                    [ButtonName.PP] = "dash",
                    [ButtonName.LK] = "s1",
                    [ButtonName.MK] = "s2",
                    [ButtonName.HK] = "tag",
                    [ButtonName.KK] = "parry",
                },
                MacrosTemplate = [],
            },
            ["DBFZ"] = new()
            {
                Name = "dbfz",
                Textures = new()
                {
                    [ButtonName.LP] = "l",
                    [ButtonName.MP] = "m",
                    [ButtonName.MK] = "h",
                    [ButtonName.LK] = "s",
                    [ButtonName.PP] = "a1",
                    [ButtonName.KK] = "a2",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.HP] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.HK] = [ButtonName.LK, ButtonName.MK],
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
                MacrosTemplate = new()
                {
                    [ButtonName.PP] = [ButtonName.LP, ButtonName.LK],
                    [ButtonName.HP] = [ButtonName.MP, ButtonName.LK],
                    [ButtonName.HK] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.KK] = [ButtonName.LK, ButtonName.MK],
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
            ["SF6 Modern"] = new()
            {
                Name = "sf_modern",
                Textures = new()
                {
                    [ButtonName.LP] = "l",
                    [ButtonName.MP] = "sp",
                    [ButtonName.HP] = "dp",
                    [ButtonName.PP] = "di",
                    [ButtonName.LK] = "m",
                    [ButtonName.MK] = "h",
                    [ButtonName.HK] = "auto",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.KK] = [ButtonName.LP, ButtonName.LK],
                },
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
            ["SFxT"] = new()
            {
                Name = "sfxt",
                MacrosTemplate = DefaultTripleMacro(),
                Textures = new()
                {
                    [ButtonName.LP] = "lp",
                    [ButtonName.MP] = "mp",
                    [ButtonName.HP] = "hp",
                    [ButtonName.LK] = "lk",
                    [ButtonName.MK] = "mk",
                    [ButtonName.HK] = "hk",
                    [ButtonName.LP | ButtonName.MP] = "2/lp_mp",
                    [ButtonName.LP | ButtonName.HP] = "2/lp_hp",
                    [ButtonName.LP | ButtonName.LK] = "2/lp_lk",
                    [ButtonName.LP | ButtonName.MK] = "2/lp_mk",
                    [ButtonName.LP | ButtonName.HK] = "2/lp_hk",
                    [ButtonName.MP | ButtonName.HP] = "2/mp_hp",
                    [ButtonName.MP | ButtonName.LK] = "2/mp_lk",
                    [ButtonName.MP | ButtonName.MK] = "2/mp_mk",
                    [ButtonName.MP | ButtonName.HK] = "2/mp_hk",
                    [ButtonName.HP | ButtonName.LK] = "2/hp_lk",
                    [ButtonName.HP | ButtonName.MK] = "2/hp_mk",
                    [ButtonName.HP | ButtonName.HK] = "2/hp_hk",
                    [ButtonName.LK | ButtonName.MK] = "2/lk_mk",
                    [ButtonName.LK | ButtonName.HK] = "2/lk_hk",
                    [ButtonName.MK | ButtonName.HK] = "2/mk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP] = "3/lp_mp_hp",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK] = "3/lp_mp_lk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.MK] = "3/lp_mp_mk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HK] = "3/lp_mp_hk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.LK] = "3/lp_hp_lk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.MK] = "3/lp_hp_mk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.HK] = "3/lp_hp_hk",
                    [ButtonName.LP | ButtonName.LK | ButtonName.MK] = "3/lp_lk_mk",
                    [ButtonName.LP | ButtonName.LK | ButtonName.HK] = "3/lp_lk_hk",
                    [ButtonName.LP | ButtonName.MK | ButtonName.HK] = "3/lp_mk_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.HK] = "3/mp_hp_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.LK] = "3/mp_hp_lk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.MK] = "3/mp_hp_mk",
                    [ButtonName.MP | ButtonName.LK | ButtonName.HK] = "3/mp_lk_hk",
                    [ButtonName.MP | ButtonName.LK | ButtonName.MK] = "3/mp_lk_mk",
                    [ButtonName.MP | ButtonName.MK | ButtonName.HK] = "3/mp_mk_hk",
                    [ButtonName.HP | ButtonName.LK | ButtonName.MK] = "3/hp_lk_mk",
                    [ButtonName.HP | ButtonName.LK | ButtonName.HK] = "3/hp_lk_hk",
                    [ButtonName.HP | ButtonName.MK | ButtonName.HK] = "3/hp_mk_hk",
                    [ButtonName.LK | ButtonName.MK | ButtonName.HK] = "3/lk_mk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.LK] = "4/lp_mp_hp_lk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.MK] = "4/lp_mp_hp_mk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.HK] = "4/lp_mp_hp_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.MK] = "4/lp_mp_lk_mk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.HK] = "4/lp_mp_lk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.MK | ButtonName.HK] = "4/lp_mp_mk_hk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.LK | ButtonName.HK] = "4/lp_hp_lk_hk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.LK | ButtonName.MK] = "4/lp_hp_lk_mk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.MK | ButtonName.HK] = "4/lp_hp_mk_hk",
                    [ButtonName.LP | ButtonName.LK | ButtonName.MK | ButtonName.HK] = "4/lp_lk_mk_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.HK] = "4/mp_hp_lk_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.MK | ButtonName.HK] = "4/mp_hp_mk_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.MK] = "4/mp_hp_lk_mk",
                    [ButtonName.MP | ButtonName.LK | ButtonName.MK | ButtonName.HK] = "4/mp_lk_mk_hk",
                    [ButtonName.HP | ButtonName.LK | ButtonName.MK | ButtonName.HK] = "4/hp_lk_mk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.MK] =
                        "5/lp_mp_hp_lk_mk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.HK] =
                        "5/lp_mp_hp_lk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.MK | ButtonName.HK] =
                        "5/lp_mp_hp_mk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.MK | ButtonName.HK] =
                        "5/lp_mp_lk_mk_hk",
                    [ButtonName.LP | ButtonName.HP | ButtonName.LK | ButtonName.MK | ButtonName.HK] =
                        "5/lp_hp_lk_mk_hk",
                    [ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.MK | ButtonName.HK] =
                        "5/mp_hp_lk_mk_hk",
                    [ButtonName.LP | ButtonName.MP | ButtonName.HP | ButtonName.LK | ButtonName.MK | ButtonName.HK] =
                        "lp_mp_hp_lk_mk_hk",
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
                    [ButtonName.KK] = "dash",
                    [ButtonName.PP] = "rc",
                    [ButtonName.HK] = "fd",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.HK] = [],
                },
            },
            ["BlazBlue"] = new()
            {
                Name = "bbcf",
                Textures = new()
                {
                    [ButtonName.LP] = "a",
                    [ButtonName.MP] = "b",
                    [ButtonName.MK] = "c",
                    [ButtonName.LK] = "d",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.HP] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.PP] = [ButtonName.LP, ButtonName.MP, ButtonName.MK],
                },
            },
            ["DNF Duel"] = new()
            {
                Name = "dnfd",
                Textures = new()
                {
                    [ButtonName.LP] = "a",
                    [ButtonName.MP] = "b",
                    [ButtonName.MK] = "s",
                    [ButtonName.LK] = "ms",
                    [ButtonName.HK] = "g",
                    [ButtonName.KK] = "as",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.PP] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.HP] = [ButtonName.MP, ButtonName.MK],
                },
            },
            ["GranBlue"] = new()
            {
                Name = "gbfv",
                Textures = new()
                {
                    [ButtonName.LP] = "l",
                    [ButtonName.MP] = "m",
                    [ButtonName.MK] = "h",
                    [ButtonName.LK] = "u",
                    [ButtonName.HP] = "ab",
                    [ButtonName.HK] = "g",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.PP] = [ButtonName.MP, ButtonName.MK],
                    [ButtonName.KK] = [ButtonName.LP, ButtonName.LK],
                },
            },
            ["Melty Blood"] = new()
            {
                Name = "melty_blood",
                Textures = new()
                {
                    [ButtonName.None] = "none",
                    [ButtonName.LP] = "a",
                    [ButtonName.MP] = "b",
                    [ButtonName.MK] = "c",
                    [ButtonName.LK] = "d",
                    [ButtonName.LP | ButtonName.MP] = "ab",
                    [ButtonName.LP | ButtonName.LK] = "ac",
                    [ButtonName.LP | ButtonName.MK] = "ad",
                    [ButtonName.MP | ButtonName.LK] = "bc",
                    [ButtonName.MP | ButtonName.MK] = "bd",
                    [ButtonName.LK | ButtonName.MK] = "cd",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK] = "abc",
                    [ButtonName.LP | ButtonName.MP | ButtonName.MK] = "abd",
                    [ButtonName.LP | ButtonName.LK | ButtonName.MK] = "acd",
                    [ButtonName.MP | ButtonName.LK | ButtonName.MK] = "bcd",
                    [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.MK] = "abcd",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.HP] = [ButtonName.MP, ButtonName.MK],
                    [ButtonName.HK] = [ButtonName.LP, ButtonName.MP],
                    [ButtonName.PP] = [ButtonName.LP, ButtonName.LK],
                    [ButtonName.KK] = [ButtonName.LP, ButtonName.MP, ButtonName.MK],
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
                },
            },
            ["Virtua Fighter"] = new()
            {
                Name = "vf",
                Textures = new()
                {
                    [ButtonName.LP] = "g",
                    [ButtonName.MP] = "p",
                    [ButtonName.MK] = "k",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.LK] = [ButtonName.MP],
                    [ButtonName.PP] = [ButtonName.MP, ButtonName.LP],
                    [ButtonName.KK] = [ButtonName.MP, ButtonName.MK, ButtonName.LP],
                    [ButtonName.HP] = [ButtonName.MP, ButtonName.MK],
                    [ButtonName.HK] = [ButtonName.MK, ButtonName.LP],
                },
            },
            ["DOA6"] = new()
            {
                Name = "doa",
                Textures = new()
                {
                    [ButtonName.LP] = "h",
                    [ButtonName.MP] = "p",
                    [ButtonName.LK] = "t",
                    [ButtonName.MK] = "k",
                    [ButtonName.HP] = "s",
                    [ButtonName.KK] = "ap",
                },
                MacrosTemplate = new()
                {
                    [ButtonName.PP] = [ButtonName.MP, ButtonName.MK],
                    [ButtonName.HK] = [ButtonName.LP, ButtonName.MK],
                },
            },
            [PlayStation] = new()
            {
                Name = "ps",
                IsPad = true,
                Textures = ForPlayStation(),
            },
            ["PS3"] = new()
            {
                Name = "ps3",
                IsPad = true,
                Textures = ForPlayStation(),
            },
            ["PS5"] = new()
            {
                Name = "ps5",
                IsPad = true,
                Textures = ForPlayStation(),
            },
            [Xbox] = new()
            {
                Name = "xbox",
                IsPad = true,
                Textures = ForXBox(),
            },
            ["Xbox Series"] = new()
            {
                Name = "xbox_s",
                IsPad = true,
                Textures = ForXBox(),
            },
            ["Xbox 360"] = new()
            {
                Name = "xbox360",
                IsPad = true,
                Textures = ForXBox(),
            },
            [Nintendo] = new()
            {
                Name = "switch",
                IsPad = true,
                Textures = new()
                {
                    [ButtonName.LP] = "y",
                    [ButtonName.MP] = "x",
                    [ButtonName.HP] = "r",
                    [ButtonName.HK] = "zr",
                    [ButtonName.LK] = "b",
                    [ButtonName.MK] = "a",
                    [ButtonName.PP] = "l",
                    [ButtonName.KK] = "zl",
                    [ButtonName.LS] = "ls",
                    [ButtonName.RS] = "rs",
                },
            },
        };

    public static readonly IReadOnlyDictionary<PlayerInputDevice.Kind, string> ControllerTypes =
        new Dictionary<PlayerInputDevice.Kind, string>
        {
            [PlayerInputDevice.Kind.PlayStation] = PlayStation,
            [PlayerInputDevice.Kind.Xbox] = Xbox,
            [PlayerInputDevice.Kind.Nintendo] = Nintendo,
            [PlayerInputDevice.Kind.Keyboard] = Xbox,
        };

    static InputMacro DefaultTripleMacro() => new()
    {
        [ButtonName.PP] = [ButtonName.LP, ButtonName.MP, ButtonName.HP],
        [ButtonName.KK] = [ButtonName.LK, ButtonName.MK, ButtonName.HK],
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
        [ButtonName.LP | ButtonName.LK | ButtonName.MK] = "1_3_4",
        [ButtonName.MP | ButtonName.LK | ButtonName.MK] = "2_3_4",
        [ButtonName.LP | ButtonName.MP | ButtonName.LK | ButtonName.MK] = "1_2_3_4",
        [ButtonName.HP] = "hp",
        [ButtonName.HK] = "hk",
        [ButtonName.PP] = "pp",
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
        [ButtonName.LK] = "cross",
        [ButtonName.MK] = "circle",
        [ButtonName.HK] = "r2",
        [ButtonName.KK] = "l2",
        [ButtonName.LS] = "l3",
        [ButtonName.RS] = "r3",
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
        [ButtonName.LS] = "ls",
        [ButtonName.RS] = "rs",
    };
}
