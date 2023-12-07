using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Util;

public static class KeyboardManager
{
    static KeyboardState previousKeyState;
    static KeyboardState currentKeyState;

    public static void Update()
    {
        previousKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();
    }

    public static bool IsKeyDown(Keys key) => currentKeyState.IsKeyDown(key);
    public static bool WasKeyDown(Keys key) => previousKeyState.IsKeyDown(key);

    public static bool IsKeyPressed(Keys key) => IsKeyDown(key) && !WasKeyDown(key);
}
