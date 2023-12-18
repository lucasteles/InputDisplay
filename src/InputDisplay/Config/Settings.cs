using System.Text.Json.Serialization;
using InputDisplay.Inputs;
using InputDisplay.Themes;

namespace InputDisplay.Config;

[Serializable]
public class Settings
{
    public record SelectedTheme(
        string Buttons = ThemeManager.DefaultButtons,
        string Direction = ThemeManager.DefaultDirection
    );

    public SelectedTheme CurrentTheme { get; set; } = new();

    public bool Borderless { get; set; }
    public bool ShadowHolding { get; set; } = true;
    public bool ShowFrames { get; set; } = true;
    public bool ShowNeutralIcon { get; set; } = true;
    public bool AutoCorrectMultiple { get; set; } = true;
    public bool InvertHistory { get; set; } = false;
    public bool FramesAfter { get; set; } = false;
    public bool HideButtonRelease { get; set; }
    public bool ShortcutsEnabled { get; set; } = true;
    public int IconSize { get; set; } = 40;
    public int SpaceBetweenInputs { get; set; } = 2;
    public int SpaceBetweenCommands { get; set; } = 4;
    public int DirectionSpace { get; set; } = 10;

    public int Width { get; set; } = 480;
    public int Height { get; set; } = 1024;
    public int Top { get; set; }
    public int Left { get; set; }

    public InputMap InputMap { get; set; } = new();

    public InputMacro Macros { get; set; } = new();

    public string BackgroundColor
    {
        get => ClearColor.ToHex();
        set => ClearColor = ColorUtil.FromHex(value);
    }

    [JsonIgnore]
    public bool Dirty { get; set; }

    [JsonIgnore]
    public Color ClearColor { get; set; } = Color.DarkOliveGreen;

    [JsonIgnore]
    public bool IsHorizontal => Width > Height;

    [JsonIgnore]
    public int MaxEntries
    {
        get
        {
            var windowSize = IsHorizontal ? Width : Height;
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

    public void CopyFrom(Settings config)
    {
        IconSize = config.IconSize;
        SpaceBetweenInputs = config.SpaceBetweenInputs;
        CurrentTheme = config.CurrentTheme;
        SpaceBetweenCommands = config.SpaceBetweenCommands;
        ShadowHolding = config.ShadowHolding;
        ShowFrames = config.ShowFrames;
        ShortcutsEnabled = config.ShortcutsEnabled;
        ClearColor = config.ClearColor;
        ShowNeutralIcon = config.ShowNeutralIcon;
        Macros = config.Macros;
        InputMap = config.InputMap;
        DirectionSpace = config.DirectionSpace;
        AutoCorrectMultiple = config.AutoCorrectMultiple;
        InvertHistory = config.InvertHistory;
        FramesAfter = config.FramesAfter;
        Borderless = config.Borderless;
        HideButtonRelease = config.HideButtonRelease;
        Width = config.Width;
        Height = config.Height;
        Top = config.Top;
        Left = config.Left;
    }
}
