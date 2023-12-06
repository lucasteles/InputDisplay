namespace InputDisplay.Inputs;

public class InputConfig
{
    public required int ButtonIconSize { get; set; }
    public required int SpaceBetweenInputs { get; set; }
    public required int MaxEntries { get; set; }
    public required Theme.Theme Theme { get; set; }
    public Theme.Theme? FallbackTheme { get; set; }
    public required int SpaceBetweenCommands { get; set; }
    public int DirectionIconSize { get; set; }
    public bool ShadowHolding { get; set; }
    public bool ShowFrames { get; set; } = true;

    public bool AutoCorrectMultiple { get; set; }
    public int ValidDirectionSize => DirectionIconSize is 0 ? ButtonIconSize : DirectionIconSize;
    public int MaxIconSize => Math.Max(DirectionIconSize, ButtonIconSize);
    public bool InvertHistory { get; set; }
    public bool HideButtonRelease { get; set; }
    public bool Horizontal { get; set; }
    public Color BackgroundColor { get; set; } = Color.DarkGray;

    public void ConfigureForWindow(GameWindow window)
    {
        Horizontal = window.ClientBounds.Width > window.ClientBounds.Height;

        var windowSize = Horizontal ? window.ClientBounds.Size.X : window.ClientBounds.Size.Y;
        var estimateCount = (windowSize - (SpaceBetweenCommands + SpaceBetweenInputs))
                            / (float)(SpaceBetweenCommands + MaxIconSize);

        MaxEntries = (int)Math.Ceiling(estimateCount * 2f);
    }
}
