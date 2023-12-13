using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record InputMap
{
    [Serializable]
    public record ButtonMap
    {
        public required string Name { get; init; }
        public PlayerPad.Kind Kind { get; set; }

        public Buttons LP { get; set; } = Buttons.X;
        public Buttons MP { get; set; } = Buttons.Y;
        public Buttons HP { get; set; } = Buttons.RightShoulder;
        public Buttons PP { get; set; } = Buttons.LeftShoulder;
        public Buttons LK { get; set; } = Buttons.A;
        public Buttons MK { get; set; } = Buttons.B;
        public Buttons HK { get; set; } = Buttons.RightTrigger;
        public Buttons KK { get; set; } = Buttons.LeftTrigger;

        public Buttons Get(ButtonName button) => button switch
        {
            ButtonName.LP => LP,
            ButtonName.MP => MP,
            ButtonName.HP => HP,
            ButtonName.PP => PP,
            ButtonName.LK => LK,
            ButtonName.MK => MK,
            ButtonName.HK => HK,
            ButtonName.KK => KK,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null),
        };

        public ButtonName? Get(Buttons button)
        {
            if (LP == button) return ButtonName.LP;
            if (MP == button) return ButtonName.MP;
            if (HP == button) return ButtonName.HP;
            if (PP == button) return ButtonName.PP;
            if (LK == button) return ButtonName.LK;
            if (MK == button) return ButtonName.MK;
            if (HK == button) return ButtonName.HK;
            if (KK == button) return ButtonName.KK;
            return null;
        }

        public void Set(ButtonName name, Buttons btn)
        {
            switch (name)
            {
                case ButtonName.LP:
                    LP = Swap(LP);
                    return;
                case ButtonName.MP:
                    MP = Swap(MP);
                    return;
                case ButtonName.HP:
                    HP = Swap(HP);
                    return;
                case ButtonName.PP:
                    PP = Swap(PP);
                    return;
                case ButtonName.LK:
                    LK = Swap(LK);
                    return;
                case ButtonName.MK:
                    MK = Swap(MK);
                    return;
                case ButtonName.HK:
                    HK = Swap(HK);
                    return;
                case ButtonName.KK:
                    KK = Swap(KK);
                    return;
                default:
                    Log.Info($"Unmappable button {name}");
                    return;
            }

            Buttons Swap(Buttons field)
            {
                if (field == btn) return field;
                if (btn is not Buttons.None && Get(btn) is { } oldField && oldField != name)
                    Set(oldField, Buttons.None);

                return btn;
            }
        }

        public static readonly ButtonMap Default = new()
        {
            Name = "Default",
        };

        public void Reset()
        {
            LP = Default.LP;
            MP = Default.MP;
            HP = Default.HP;
            PP = Default.PP;
            LK = Default.LK;
            MK = Default.MK;
            HK = Default.HK;
            KK = Default.KK;
        }
    }

    public Dictionary<string, ButtonMap> Pads { get; set; } = new();

    public bool Contains(string id) => Pads.ContainsKey(id);

    public void AddGamePad(GamePadCapabilities caps) =>
        Pads.Add(caps.Identifier, new()
        {
            Name = caps.DisplayName,
            Kind = PlayerPad.GetPadKind(caps),
        });

    public bool TryAddGamePad(GamePadCapabilities caps)
    {
        if (Contains(caps.Identifier)) return false;
        AddGamePad(caps);
        return true;
    }

    public ButtonMap? GetMapping(GamePadCapabilities pad) =>
        GetMapping(pad.Identifier);

    public ButtonMap? GetMapping(string identifier) =>
        identifier.IsEmpty() || !Pads.TryGetValue(identifier, out var buttonMap)
            ? null
            : buttonMap;

    public ButtonMap GetMappingOrDefault(string identifier) =>
        GetMapping(identifier) ?? ButtonMap.Default;

    public PlayerPad.Kind GetPadKind(PlayerPad player) =>
        GetMapping(player)?.Kind ?? player.GetPadKind();
}
