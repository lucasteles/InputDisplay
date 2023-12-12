using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Util;

public static class MouseManager
{
    static MouseState previousState;
    static MouseState currentState;

    static readonly TimeSpan doubleClickDelay = TimeSpan.FromSeconds(0.3f);
    static DateTime previousClick;

    public static void BeginUpdate()
    {
        previousState = currentState;
        currentState = Mouse.GetState();
    }

    public static void EndUpdate()
    {
        if (WasLeftClick)
            previousClick = DateTime.UtcNow;
    }

    public static bool IsDragging =>
        previousState.LeftButton is ButtonState.Pressed && currentState.LeftButton is ButtonState.Pressed;

    public static bool IsOnWindow(GameWindow window) =>
        window.ClientBounds.Contains(window.Position + previousState.Position);

    public static bool IsDraggingOnWindow(GameWindow window) =>
        IsDragging && IsOnWindow(window);

    public static Point DeltaDragging => currentState.Position - previousState.Position;

    public static int ScrollWheelValue => currentState.ScrollWheelValue;

    public static int DeltaWheelValue => ScrollWheelValue - previousState.ScrollWheelValue;
    public static Point Position => currentState.Position;

    public static bool WasDoubleLeftClick =>
        WasLeftClick && DateTime.UtcNow - previousClick < doubleClickDelay;

    public static bool WasLeftClick => previousState.LeftButton is ButtonState.Pressed
                                       && currentState.LeftButton is ButtonState.Released;
}
