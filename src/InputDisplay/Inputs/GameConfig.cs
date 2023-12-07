using System.Text.Json;
using System.Text.Json.Serialization;
using InputDisplay.Theme;

namespace InputDisplay.Inputs;

public class GameConfig
{
    public record SelectedTheme(string Buttons, string Direction = ThemeManager.DefaultDirection);

    Theme.Theme? currentTheme;

    public int IconSize { get; set; } = 40;
    public int SpaceBetweenInputs { get; set; } = 2;

    public SelectedTheme CurrentTheme { get; set; } = new(ThemeManager.DefaultButtons);

    [JsonIgnore]
    public Theme.Theme Theme
    {
        get
        {
            currentTheme ??= ThemeManager.Get(CurrentTheme);
            return currentTheme;
        }
        set
        {
            CurrentTheme = new(value.ButtonsName, value.StickName);
            currentTheme = value;
        }
    }

    [JsonIgnore]
    public Theme.Theme? FallbackTheme { get; set; }

    public int SpaceBetweenCommands { get; set; } = 4;
    public bool ShadowHolding { get; set; } = true;
    public bool ShowFrames { get; set; } = true;
    public int DirectionSpace { get; set; } = 10;

    public bool AutoCorrectMultiple { get; set; } = true;
    public bool InvertHistory { get; set; }
    public bool HideButtonRelease { get; set; }

    public int Width { get; set; } = 480;
    public int Height { get; set; } = 1024;

    public int Top { get; set; }
    public int Left { get; set; }

    public Color BackgroundColor { get; set; } = Color.DarkOliveGreen;

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
        Save();
    }

    static readonly JsonSerializerOptions jsonOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(),
        },
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    const string fileName = "config.json";

    public static GameConfig CreateFile()
    {
        GameConfig config = new();
        config.Save();
        return config;
    }

    public static GameConfig Load()
    {
        if (!File.Exists(fileName))
            return CreateFile();

        try
        {
            var content = File.ReadAllBytes(fileName);
            if (JsonSerializer.Deserialize<GameConfig>(content, jsonOptions) is { } config)
                return config;
        }
        catch
        {
            File.Delete(fileName);
        }

        return CreateFile();
    }

    public void Save()
    {
        Console.WriteLine("Saving config...");
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(this, jsonOptions);
        File.WriteAllBytes(fileName, jsonBytes);
    }
}
