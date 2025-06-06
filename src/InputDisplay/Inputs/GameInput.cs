using InputDisplay.Config;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public class GameInput
{
    public enum ButtonStatus
    {
        Unpressed,
        Pressed,
        Holding,
        Released,
    }

    public readonly record struct Button(ButtonStatus Status)
    {
        public bool Pressed => Status is ButtonStatus.Pressed;
        public bool Active => Status is ButtonStatus.Pressed or ButtonStatus.Holding;

        public static implicit operator Button(ButtonStatus status) => new()
        {
            Status = status,
        };

        public static implicit operator ButtonStatus(Button btn) => btn.Status;

        public Button Combine(Button other) => new(
            (Status, other.Status) is (_, ButtonStatus.Pressed) or (ButtonStatus.Pressed, _)
                ? ButtonStatus.Pressed
                : Status
        );
    }

    public record struct Stick(
        Direction Direction,
        Direction Raw,
        bool Holding
    );

    public record struct State
    {
        public Stick Stick;
        public Button LP;
        public Button MP;
        public Button HP;
        public Button PP;
        public Button LK;
        public Button MK;
        public Button HK;
        public Button KK;
        public Button LS;
        public Button RS;

        public readonly bool EquivalentTo(State other) =>
            Stick.Direction == other.Stick.Direction
            && LP.Active == other.LP.Active
            && MP.Active == other.MP.Active
            && HP.Active == other.HP.Active
            && PP.Active == other.PP.Active
            && LK.Active == other.LK.Active
            && MK.Active == other.MK.Active
            && HK.Active == other.HK.Active
            && KK.Active == other.KK.Active
            && LS.Active == other.LS.Active
            && RS.Active == other.RS.Active;

        public readonly bool IsNeutralOnly =>
            this is
            {
                Stick.Direction: Direction.Neutral,
                LP.Active: false,
                MP.Active: false,
                HP.Active: false,
                PP.Active: false,
                LK.Active: false,
                MK.Active: false,
                HK.Active: false,
                KK.Active: false,
                LS.Active: false,
                RS.Active: false,
            };

        public readonly bool IsDirectionOnly =>
            this is
            {
                Stick.Direction: not Direction.Neutral,
                LP.Active: false,
                MP.Active: false,
                HP.Active: false,
                PP.Active: false,
                LK.Active: false,
                MK.Active: false,
                HK.Active: false,
                KK.Active: false,
                LS.Active: false,
                RS.Active: false,
            };

        public readonly bool HasPressed => !HasNoPressed;

        public readonly bool HasNoPressed =>
            this is
            {
                Stick: { Direction: Direction.Neutral } or { Holding: true },
                LP.Pressed: false,
                MP.Pressed: false,
                HP.Pressed: false,
                PP.Pressed: false,
                LK.Pressed: false,
                MK.Pressed: false,
                HK.Pressed: false,
                KK.Pressed: false,
                LS.Active: false,
                RS.Active: false,
            };

        public void Combine(State other)
        {
            LP = LP.Combine(other.LP);
            MP = MP.Combine(other.MP);
            HP = HP.Combine(other.HP);
            PP = PP.Combine(other.PP);

            LK = LK.Combine(other.LK);
            MK = MK.Combine(other.MK);
            HK = HK.Combine(other.HK);
            KK = KK.Combine(other.KK);

            LS = LS.Combine(other.LS);
            RS = RS.Combine(other.RS);
        }

        public readonly ButtonName GetActiveButtons()
        {
            var result = ButtonName.None;
            if (LP.Active) result |= ButtonName.LP;
            if (MP.Active) result |= ButtonName.MP;
            if (HP.Active) result |= ButtonName.HP;
            if (PP.Active) result |= ButtonName.PP;
            if (LK.Active) result |= ButtonName.LK;
            if (MK.Active) result |= ButtonName.MK;
            if (HK.Active) result |= ButtonName.HK;
            if (KK.Active) result |= ButtonName.KK;
            if (LS.Active) result |= ButtonName.LS;
            if (RS.Active) result |= ButtonName.RS;
            return result;
        }
    }

    State currentState;
    public State CurrentState => currentState;

    static void UpdateButton(GamePadState state, Buttons padButton, ref Button button)
    {
        if (padButton is Buttons.None)
        {
            button = ButtonStatus.Unpressed;
            return;
        }

        var pressed = state.IsButtonDown(padButton);

        switch (button.Status, pressed)
        {
            case (ButtonStatus.Unpressed, true):
                button = ButtonStatus.Pressed;
                break;
            case (ButtonStatus.Unpressed, false):
                break;
            case (ButtonStatus.Pressed, true):
                button = ButtonStatus.Holding;
                break;
            case (ButtonStatus.Pressed, false):
                button = ButtonStatus.Released;
                break;
            case (ButtonStatus.Released, true):
                button = ButtonStatus.Pressed;
                break;
            case (ButtonStatus.Released, false):
                button = ButtonStatus.Unpressed;
                break;
            case (ButtonStatus.Holding, true):
                break;
            case (ButtonStatus.Holding, false):
                button = ButtonStatus.Released;
                break;
        }
    }

    void UpdateDirections(GamePadState state, SOCDMode socd, Settings.DirectionSources sources)
    {
        if (sources is Settings.DirectionSources.None)
        {
            currentState.Stick.Direction = Direction.Neutral;
            currentState.Stick.Raw = Direction.Neutral;
            currentState.Stick.Holding = false;
            return;
        }

        var direction = Direction.Neutral;
        if (
            (sources.HasFlag(Settings.DirectionSources.DPad) && state.IsButtonDown(Buttons.DPadUp))
            || (sources.HasFlag(Settings.DirectionSources.LeftAnalog) && state.IsButtonDown(Buttons.LeftThumbstickUp))
            || (sources.HasFlag(Settings.DirectionSources.RightAnalog) && state.IsButtonDown(Buttons.RightThumbstickUp))
        )
            direction |= Direction.Up;

        if (
            (sources.HasFlag(Settings.DirectionSources.DPad) && state.IsButtonDown(Buttons.DPadDown))
            || (sources.HasFlag(Settings.DirectionSources.LeftAnalog) && state.IsButtonDown(Buttons.LeftThumbstickDown))
            || (sources.HasFlag(Settings.DirectionSources.RightAnalog) &&
                state.IsButtonDown(Buttons.RightThumbstickDown))
        )
            direction |= Direction.Down;

        if (
            (sources.HasFlag(Settings.DirectionSources.DPad) && state.IsButtonDown(Buttons.DPadLeft))
            || (sources.HasFlag(Settings.DirectionSources.LeftAnalog) && state.IsButtonDown(Buttons.LeftThumbstickLeft))
            || (sources.HasFlag(Settings.DirectionSources.RightAnalog) &&
                state.IsButtonDown(Buttons.RightThumbstickLeft))
        )
            direction |= Direction.Backward;

        if (
            (sources.HasFlag(Settings.DirectionSources.DPad) && state.IsButtonDown(Buttons.DPadRight)) ||
            (sources.HasFlag(Settings.DirectionSources.LeftAnalog) && state.IsButtonDown(Buttons.LeftThumbstickRight))
            || (sources.HasFlag(Settings.DirectionSources.RightAnalog) &&
                state.IsButtonDown(Buttons.RightThumbstickRight)))
            direction |= Direction.Forward;

        currentState.Stick.Holding = currentState.Stick.Raw == direction && direction != Direction.Neutral;
        currentState.Stick.Direction = SOCD.Clean(socd, direction, currentState.Stick);
        currentState.Stick.Raw = direction;
    }

    public void Update(
        PlayerInputDevice pad,
        Settings config
    )
    {
        var state = pad.State;
        UpdateDirections(state, config.SOCD, config.EnabledDirections);

        var map = config.InputMap.GetMappingOrDefault(pad.Identifier);
        UpdateButton(state, map.LP, ref currentState.LP);
        UpdateButton(state, map.MP, ref currentState.MP);
        UpdateButton(state, map.HP, ref currentState.HP);
        UpdateButton(state, map.PP, ref currentState.PP);
        UpdateButton(state, map.LK, ref currentState.LK);
        UpdateButton(state, map.MK, ref currentState.MK);
        UpdateButton(state, map.HK, ref currentState.HK);
        UpdateButton(state, map.KK, ref currentState.KK);
        UpdateButton(state, map.LS, ref currentState.LS);
        UpdateButton(state, map.RS, ref currentState.RS);
    }
}
