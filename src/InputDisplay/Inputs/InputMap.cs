using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record InputMap
{
    [Serializable]
    public record ButtonMap
    {
        public required string Name { get; init; }

        public Buttons LP { get; set; } = Buttons.X;
        public Buttons MP { get; set; } = Buttons.Y;
        public Buttons HP { get; set; } = Buttons.RightShoulder;
        public Buttons PP { get; set; } = Buttons.LeftShoulder;
        public Buttons LK { get; set; } = Buttons.A;
        public Buttons MK { get; set; } = Buttons.B;
        public Buttons HK { get; set; } = Buttons.RightTrigger;
        public Buttons KK { get; set; } = Buttons.LeftTrigger;

        public Buttons GetButton(Buttons button) => (button, this) switch
        {
            (Buttons.X, { LP: { } btn }) => btn,
            (Buttons.Y, { MP: { } btn }) => btn,
            (Buttons.RightShoulder, { HP: { } btn }) => btn,
            (Buttons.LeftShoulder, { PP: { } btn }) => btn,
            (Buttons.A, { LK: { } btn }) => btn,
            (Buttons.B, { MK: { } btn }) => btn,
            (Buttons.RightTrigger, { HK: { } btn }) => btn,
            (Buttons.LeftTrigger, { KK: { } btn }) => btn,
            _ => button,
        };
    }

    public Dictionary<string, ButtonMap> Maps { get; set; } = new();

    public bool Contains(string id) => Maps.ContainsKey(id);

    public void AddGamePad(GamePadCapabilities caps) =>
        Maps.Add(caps.Identifier, new()
        {
            Name = caps.DisplayName,
        });

    public Buttons GetButton(string identifier, Buttons button) =>
        string.IsNullOrWhiteSpace(identifier) || !Maps.TryGetValue(identifier, out var buttonMap)
            ? button
            : buttonMap.GetButton(button);
}
