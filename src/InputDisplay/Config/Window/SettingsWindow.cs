using System.Diagnostics;
using InputDisplay.Inputs;

namespace InputDisplay.Config.Window;

public sealed class SettingsWindow : IDisposable
{
    public enum WindowState { Closed, Starting, Started }

    readonly Process process = new();
    WindowState state = WindowState.Closed;

    public bool IsOpen() => state is not WindowState.Closed && process is { Responding: true, HasExited: false };

    public void Open(PlayerInputDevice? player = null)
    {
        if (state is WindowState.Starting) return;

        process.EnableRaisingEvents = true;
        var si = process.StartInfo;
        si.UseShellExecute = false;
        si.FileName = Process.GetCurrentProcess().MainModule?.FileName;
        si.Arguments = "config";

        switch (player)
        {
            case { IsKeyboard: true }:
                si.Arguments += " keyboard";
                break;
            case { Index: var index }:
                si.Arguments += $" {index}";
                break;
        }

        if (IsOpen())
        {
            process.CloseMainWindow();
            process.WaitForExit();
        }

        if (process.Start())
        {
            state = WindowState.Started;
            process.WaitForInputIdle();
        }
        else
        {
            state = WindowState.Closed;
            Log.Error(process.StandardError.ReadToEnd());
        }
    }

    public void Close()
    {
        if (!IsOpen()) return;
        process.CloseMainWindow();
        process.WaitForExit();
        state = WindowState.Closed;
    }

    public void Dispose()
    {
        Close();
        process.Dispose();
    }
}
