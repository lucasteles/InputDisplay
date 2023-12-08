namespace InputDisplay.Util;

public static class Log
{
    public static bool Enabled { get; set; } = true;

    static string Format(string message, Exception? exception = null) =>
        $"[{DateTime.UtcNow:o}] {message} {exception?.Message}";

    public static void Info(string message)
    {
        if (!Enabled) return;
        Console.WriteLine(Format(message));
    }

    public static void Error(string message, Exception? exception = null)
    {
        if (!Enabled) return;
        Console.Error.WriteLine(Format(message, exception));
    }
}
