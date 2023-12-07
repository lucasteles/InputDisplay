using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Util;

public static class MouseManager
{
    static MouseState previousState;
    static MouseState currentState;

    public static void Update()
    {
        previousState = currentState;
        currentState = Mouse.GetState();
    }

    public static int ScrollWheelValue => currentState.ScrollWheelValue;

    public static int DeltaWheelValue => ScrollWheelValue - previousState.ScrollWheelValue;
}
