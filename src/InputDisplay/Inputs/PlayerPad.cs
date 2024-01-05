using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record PlayerPad(
    PlayerIndex Index,
    GamePadCapabilities Capabilities
)
{
    GamePadState? currentState;

    public PlayerPad(PlayerIndex index) : this(index, GamePad.GetCapabilities(index)) { }

    public string Identifier => Capabilities.Identifier;
    public string Name => Capabilities.DisplayName;
    public bool IsConnected => currentState?.IsConnected == true;

    [MemberNotNull(nameof(currentState))]
    public void Update() => currentState = GamePad.GetState(Index);

    public void Disconnect() => currentState = null;

    public GamePadState State
    {
        get
        {
            if (currentState is null)
                Update();

            return currentState.Value;
        }
    }

    public Buttons? GetAnyButton()
    {
        if (!State.IsConnected)
            return null;

        foreach (var button in Enum.GetValues<Buttons>())
        {
            if (!State.IsButtonDown(button)) continue;
            return button;
        }

        return null;
    }

    public enum Kind
    {
        Xbox,
        PlayStation,
        Nintendo,
    }

    static readonly string[] playStationAliases =
    [
        "playstation",
        "sony",
        .. Enumerable.Range(1, 6).Select(n => $"ps{n}"),
    ];

    public static Kind GetPadKind(GamePadCapabilities caps)
    {
        var name = caps.DisplayName.ToLowerInvariant();
        if (!name.Contains("xbox"))
        {
            if (playStationAliases.Exists(name.Contains))
                return Kind.PlayStation;

            if (name.Contains("switch") || name.Contains("nintendo"))
                return Kind.Nintendo;
        }

        return Kind.Xbox;
    }

    public Kind GetPadKind() => GetPadKind(Capabilities);

    public static IEnumerable<PlayerPad> GetConnected()
    {
        foreach (var index in Enum.GetValues<PlayerIndex>())
        {
            var state = GamePad.GetCapabilities(index);
            if (!state.IsConnected) continue;
            var caps = GamePad.GetCapabilities(index);
            yield return new(index, caps);
        }
    }

    public static PlayerPad? DetectPress()
    {
        var connected = GetConnected().ToList();
        if (connected is [var justOne])
            return justOne;

        foreach (var pad in connected)
            if (pad.GetAnyButton() is not null)
                return pad;

        return null;
    }

    public static implicit operator GamePadCapabilities(PlayerPad pad) => pad.Capabilities;
    public static implicit operator PlayerIndex(PlayerPad pad) => pad.Index;
}
