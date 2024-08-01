using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

[Serializable]
public class KeyboardToPadMap
{
    public Keys Up { get; set; } = Keys.Up;
    public Keys Down { get; set; } = Keys.Down;
    public Keys Left { get; set; } = Keys.Left;
    public Keys Right { get; set; } = Keys.Right;
    public Keys X { get; set; } = Keys.A;
    public Keys Y { get; set; } = Keys.S;
    public Keys RightShoulder { get; set; } = Keys.D;
    public Keys LeftShoulder { get; set; } = Keys.F;
    public Keys A { get; set; } = Keys.Z;
    public Keys B { get; set; } = Keys.X;
    public Keys RightTrigger { get; set; } = Keys.C;
    public Keys LeftTrigger { get; set; } = Keys.V;

    public Buttons GetButtons(KeyboardState state)
    {
        var buttons = Buttons.None;

        if (state.IsKeyDown(Up))
        {
            buttons |= Buttons.DPadUp;
            buttons |= Buttons.LeftThumbstickUp;
        }

        if (state.IsKeyDown(Down))
        {
            buttons |= Buttons.DPadDown;
            buttons |= Buttons.LeftThumbstickDown;
        }

        if (state.IsKeyDown(Left))
        {
            buttons |= Buttons.DPadLeft;
            buttons |= Buttons.LeftThumbstickLeft;
        }

        if (state.IsKeyDown(Right))
        {
            buttons |= Buttons.DPadRight;
            buttons |= Buttons.LeftThumbstickRight;
        }

        if (state.IsKeyDown(X)) buttons |= Buttons.X;
        if (state.IsKeyDown(Y)) buttons |= Buttons.Y;
        if (state.IsKeyDown(RightShoulder)) buttons |= Buttons.RightShoulder;
        if (state.IsKeyDown(LeftShoulder)) buttons |= Buttons.LeftShoulder;
        if (state.IsKeyDown(A)) buttons |= Buttons.A;
        if (state.IsKeyDown(B)) buttons |= Buttons.B;
        if (state.IsKeyDown(RightTrigger)) buttons |= Buttons.RightTrigger;
        if (state.IsKeyDown(LeftTrigger)) buttons |= Buttons.LeftTrigger;

        return buttons;
    }

    public bool AnyPress() => AnyPress(Keyboard.GetState());

    public bool AnyPress(KeyboardState state) =>
        state.IsKeyDown(Up) ||
        state.IsKeyDown(Down) ||
        state.IsKeyDown(Left) ||
        state.IsKeyDown(Right) ||
        state.IsKeyDown(X) ||
        state.IsKeyDown(Y) ||
        state.IsKeyDown(RightShoulder) ||
        state.IsKeyDown(LeftShoulder) ||
        state.IsKeyDown(A) ||
        state.IsKeyDown(B) ||
        state.IsKeyDown(RightTrigger) ||
        state.IsKeyDown(LeftTrigger);

    public bool DetectUpdate(Buttons button) => DetectUpdate(button, Keyboard.GetState());

    public bool DetectUpdate(Buttons button, KeyboardState state)
    {
        if (state.GetPressedKeys() is not [var pressed])
            return false;

        if (button.HasFlag(Buttons.DPadUp) || button.HasFlag(Buttons.LeftThumbstickUp))
            Up = pressed;

        if (button.HasFlag(Buttons.DPadDown) || button.HasFlag(Buttons.LeftThumbstickDown))
            Down = pressed;

        if (button.HasFlag(Buttons.DPadLeft) || button.HasFlag(Buttons.LeftThumbstickLeft))
            Left = pressed;

        if (button.HasFlag(Buttons.DPadRight) || button.HasFlag(Buttons.RightThumbstickRight))
            Right = pressed;

        if (button.HasFlag(Buttons.X)) X = pressed;
        if (button.HasFlag(Buttons.Y)) Y = pressed;
        if (button.HasFlag(Buttons.RightShoulder)) RightShoulder = pressed;
        if (button.HasFlag(Buttons.LeftShoulder)) LeftShoulder = pressed;
        if (button.HasFlag(Buttons.A)) A = pressed;
        if (button.HasFlag(Buttons.B)) B = pressed;
        if (button.HasFlag(Buttons.RightTrigger)) RightTrigger = pressed;
        if (button.HasFlag(Buttons.LeftTrigger)) LeftTrigger = pressed;

        return true;
    }

    public GamePadState GetPadState()
    {
        var state = Keyboard.GetState();
        var buttons = GetButtons(state);

        GamePadDPad dpad = new(
            upValue: GetState(Buttons.DPadUp),
            downValue: GetState(Buttons.DPadDown),
            leftValue: GetState(Buttons.DPadLeft),
            rightValue: GetState(Buttons.DPadRight)
        );

        GamePadTriggers triggers = new(
            GetState(Buttons.LeftTrigger) is ButtonState.Pressed ? 1f : 0,
            GetState(Buttons.RightTrigger) is ButtonState.Pressed ? 1f : 0
        );

        return new(
            new(),
            triggers,
            new(buttons),
            dpad
        );

        ButtonState GetState(Buttons button) =>
            buttons.HasFlag(button) ? ButtonState.Pressed : ButtonState.Released;
    }

    public void Reset()
    {
        Up = Keys.Up;
        Down = Keys.Down;
        Left = Keys.Left;
        Right = Keys.Right;
        X = Keys.A;
        Y = Keys.S;
        RightShoulder = Keys.D;
        LeftShoulder = Keys.F;
        A = Keys.Z;
        B = Keys.X;
        RightTrigger = Keys.C;
        LeftTrigger = Keys.V;
    }
}
