using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record PlayerInputDevice(
    PlayerIndex Index,
    GamePadCapabilities Capabilities
)
{
    public const string KeyboardIdentifier = "Keyboard";

    GamePadState? currentState;
    readonly KeyboardToPadMap keyboardMapping = new();

    public PlayerInputDevice(PlayerIndex index) : this(index, GamePad.GetCapabilities(index)) { }

    public PlayerInputDevice(KeyboardToPadMap keyboardMap) : this(PlayerIndex.One, new())
    {
        IsKeyboard = true;
        keyboardMapping = keyboardMap;
    }

    public bool IsKeyboard { get; }
    public string Identifier => IsKeyboard ? KeyboardIdentifier : Capabilities.Identifier;
    public string Name => IsKeyboard ? KeyboardIdentifier : Capabilities.DisplayName;
    public bool IsConnected => IsKeyboard || currentState?.IsConnected is true;

    [MemberNotNull(nameof(currentState))]
    public void Update() =>
        currentState = IsKeyboard
            ? keyboardMapping.GetPadState()
            : GamePad.GetState(Index);

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
        Keyboard,
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

    public bool TryRemapKeyboardFor(Buttons button) => IsKeyboard && keyboardMapping.DetectUpdate(button);

    public Kind GetKind() => IsKeyboard ? Kind.Keyboard : GetPadKind(Capabilities);

    public static bool HasMultiplePads()
    {
        var count = 0;
        foreach (var index in Enum.GetValues<PlayerIndex>())
        {
            var caps = GamePad.GetCapabilities(index);
            if (!caps.IsConnected) continue;
            count++;

            if (count > 1)
                return true;
        }

        return false;
    }

    public static IEnumerable<PlayerInputDevice> GetConnectedPads()
    {
        foreach (var index in Enum.GetValues<PlayerIndex>())
        {
            var caps = GamePad.GetCapabilities(index);
            if (!caps.IsConnected) continue;
            yield return new(index, caps);
        }
    }

    public static PlayerInputDevice? DetectPress(bool autoSelectSingle, KeyboardToPadMap? keyboardToPadMap = null)
    {
        var connected = GetConnectedPads().ToList();
        if (autoSelectSingle && connected is [var justOne])
            return justOne;

        foreach (var pad in connected)
            if (pad.GetAnyButton() is not null)
                return pad;

        if (keyboardToPadMap?.AnyPress() is true)
            return new(keyboardToPadMap);

        return null;
    }

    public static implicit operator PlayerIndex(PlayerInputDevice pad) => pad.Index;
}
