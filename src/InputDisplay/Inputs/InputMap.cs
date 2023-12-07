using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record InputMap
{
    [Serializable]
    public record ButtonMap
    {
        public required string Name { get; init; }

        public Buttons? X { get; set; } = Buttons.X;
        public Buttons? Y { get; set; } = Buttons.Y;
        public Buttons? A { get; set; } = Buttons.A;
        public Buttons? B { get; set; } = Buttons.B;
        public Buttons? LB { get; set; } = Buttons.LeftShoulder;
        public Buttons? RB { get; set; } = Buttons.RightShoulder;
        public Buttons? LT { get; set; } = Buttons.LeftTrigger;
        public Buttons? RT { get; set; } = Buttons.RightTrigger;

        public Buttons GetButton(Buttons button) => (button, this) switch
        {
            (Buttons.X, { X: { } btn }) => btn,
            (Buttons.Y, { Y: { } btn }) => btn,
            (Buttons.A, { A: { } btn }) => btn,
            (Buttons.B, { B: { } btn }) => btn,
            (Buttons.LeftShoulder, { LB: { } btn }) => btn,
            (Buttons.RightShoulder, { RB: { } btn }) => btn,
            (Buttons.LeftTrigger, { LT: { } btn }) => btn,
            (Buttons.RightTrigger, { RT: { } btn }) => btn,
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
