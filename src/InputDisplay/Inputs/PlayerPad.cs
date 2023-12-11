using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public record PlayerPad(
    PlayerIndex Index,
    GamePadCapabilities Capabilities
)
{
    GamePadState? currentState;

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

    public enum Kind
    {
        Xbox,
        PlayStation,
    }

    public Kind GetPadKind()
    {
        var name = Name.ToLowerInvariant();
        return !name.Contains("xbox") && name.Contains("ps")
            ? Kind.PlayStation
            : Kind.Xbox;
    }

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
        {
            var state = GamePad.GetState(pad.Index);
            if (!state.IsConnected) continue;
            foreach (var button in Enum.GetValues<Buttons>())
            {
                if (!state.IsButtonDown(button)) continue;
                return pad;
            }
        }

        return null;
    }

    public static IEnumerable<GamePadCapabilities> GetControllers() =>
        Enum.GetValues<PlayerIndex>()
            .Select(GamePad.GetCapabilities)
            .Where(caps => caps.IsConnected);
}
