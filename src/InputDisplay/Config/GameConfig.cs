using System.Text.Json.Serialization;
using InputDisplay.Inputs;
using InputDisplay.Themes;

namespace InputDisplay.Config;

[Serializable]
public class GameConfig
{
    public record SelectedTheme(
        string Buttons = ThemeManager.DefaultButtons,
        string Direction = ThemeManager.DefaultDirection
    );

    public SelectedTheme CurrentTheme { get; set; } = new();
    public int IconSize { get; set; } = 40;
    public int SpaceBetweenInputs { get; set; } = 2;

    public bool Borderless { get; set; }
    public int SpaceBetweenCommands { get; set; } = 4;
    public bool ShadowHolding { get; set; } = true;
    public bool ShowFrames { get; set; } = true;
    public bool ShowNeutralIcon { get; set; } = true;
    public int DirectionSpace { get; set; } = 10;

    public bool AutoCorrectMultiple { get; set; } = true;
    public bool InvertHistory { get; set; }
    public bool HideButtonRelease { get; set; }

    public int Width { get; set; } = 480;
    public int Height { get; set; } = 1024;

    public int Top { get; set; }
    public int Left { get; set; }

    public InputMap InputMap { get; set; } = new();

    public string BackgroundColor
    {
        get => ClearColor.PackedValue.ToString("X");
        set => ClearColor = new(uint.Parse(value, System.Globalization.NumberStyles.HexNumber));
    }

    public InputMacro Macros { get; init; } = new();

    [JsonIgnore]
    public bool Dirty { get; set; }

    [JsonIgnore]
    public Color ClearColor { get; set; } = Color.DarkOliveGreen;

    [JsonIgnore]
    public bool Horizontal => Width > Height;

    [JsonIgnore]
    public int MaxEntries
    {
        get
        {
            var windowSize = Horizontal ? Width : Height;
            var estimateCount = (windowSize - (SpaceBetweenCommands + SpaceBetweenInputs))
                                / (float)(SpaceBetweenCommands + IconSize);

            return (int)Math.Ceiling(estimateCount * 2f);
        }
    }

    public void UpdateWindowSize(GameWindow window)
    {
        Top = window.Position.Y;
        Left = window.Position.X;
        Width = window.ClientBounds.Size.X;
        Height = window.ClientBounds.Size.Y;
    }

    public void CopyFrom(GameConfig config)
    {
        IconSize = config.IconSize;
        SpaceBetweenInputs = config.SpaceBetweenInputs;
        CurrentTheme = config.CurrentTheme;
        SpaceBetweenCommands = config.SpaceBetweenCommands;
        ShadowHolding = config.ShadowHolding;
        ShowFrames = config.ShowFrames;
        ShowNeutralIcon = config.ShowNeutralIcon;
        DirectionSpace = config.DirectionSpace;
        AutoCorrectMultiple = config.AutoCorrectMultiple;
        InvertHistory = config.InvertHistory;
        Borderless = config.Borderless;
        HideButtonRelease = config.HideButtonRelease;
        Width = config.Width;
        Height = config.Height;
        Top = config.Top;
        Left = config.Left;
    }
}
