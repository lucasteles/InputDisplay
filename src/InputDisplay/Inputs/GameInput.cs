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

    public record struct Stick(Direction Direction, bool Holding);

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

        public bool EquivalentTo(State other) =>
            Stick.Direction == other.Stick.Direction
            && LP.Active == other.LP.Active
            && MP.Active == other.MP.Active
            && HP.Active == other.HP.Active
            && PP.Active == other.PP.Active
            && LK.Active == other.LK.Active
            && MK.Active == other.MK.Active
            && HK.Active == other.HK.Active
            && KK.Active == other.KK.Active;

        public bool IsNeutralOnly =>
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
            };

        public bool HasPressed => !HasNoPressed;

        public bool HasNoPressed =>
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
        }

        public ButtonName GetActiveButtons()
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
            return result;
        }

        public ButtonName GetSinglePressed()
        {
            if (LP.Pressed) return ButtonName.LP;
            if (MP.Pressed) return ButtonName.MP;
            if (HP.Pressed) return ButtonName.HP;
            if (PP.Pressed) return ButtonName.PP;
            if (LK.Pressed) return ButtonName.LK;
            if (MK.Pressed) return ButtonName.MK;
            if (HK.Pressed) return ButtonName.HK;
            if (KK.Pressed) return ButtonName.KK;

            return ButtonName.None;
        }
    }

    State currentState;
    public State CurrentState => currentState;

    void UpdateButton(GamePadState state, Buttons padButton, ref Button button)
    {
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

    public void UpdateStick(GamePadState state)
    {
        var direction = Direction.Neutral;

        if (state.IsButtonDown(Buttons.DPadUp) || state.IsButtonDown(Buttons.LeftThumbstickUp))
            direction |= Direction.Up;

        if (state.IsButtonDown(Buttons.DPadDown) || state.IsButtonDown(Buttons.LeftThumbstickDown))
            direction |= Direction.Down;

        if (state.IsButtonDown(Buttons.DPadLeft) || state.IsButtonDown(Buttons.LeftThumbstickLeft))
            direction |= Direction.Backward;

        if (state.IsButtonDown(Buttons.DPadRight) ||
            state.IsButtonDown(Buttons.LeftThumbstickRight))
            direction |= Direction.Forward;

        currentState.Stick.Holding = currentState.Stick.Direction == direction
                                     && direction != Direction.Neutral;
        currentState.Stick.Direction = direction;
    }

    public void Update(PlayerPad pad, InputMap mapper)
    {
        var state = pad.State;
        UpdateStick(state);
        Buttons M(Buttons bts) => mapper.GetButton(pad.Identifier, bts);

        UpdateButton(state, M(Buttons.X), ref currentState.LP);
        UpdateButton(state, M(Buttons.Y), ref currentState.MP);
        UpdateButton(state, M(Buttons.RightShoulder), ref currentState.HP);
        UpdateButton(state, M(Buttons.LeftShoulder), ref currentState.PP);
        UpdateButton(state, M(Buttons.A), ref currentState.LK);
        UpdateButton(state, M(Buttons.B), ref currentState.MK);
        UpdateButton(state, M(Buttons.RightTrigger), ref currentState.HK);
        UpdateButton(state, M(Buttons.LeftTrigger), ref currentState.KK);
    }
}
