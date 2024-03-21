using System.Text.Json;
using System.Text.Json.Serialization;
using InputDisplay.Config;
using InputDisplay.Inputs;
using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Util;

public static partial class Json
{
    static readonly JsonSerializerOptions options = JsonContext.Default.Options;

    public static byte[] SerializeBytes<T>(T value) =>
        JsonSerializer.SerializeToUtf8Bytes(value, options);

    public static T? DeserializeBytes<T>(ReadOnlySpan<byte> value) =>
        JsonSerializer.Deserialize<T>(value, options);

    public static DateTime SerializeToFile<T>(T value, string filename)
    {
        var jsonBytes = SerializeBytes(value);

        for (int i = 0; i < 3; i++)
        {
            try
            {
                File.WriteAllBytes(filename, jsonBytes);
                break;
            }
            catch (Exception e)
            {
                Log.Error($"Fail to save json (retry: {i})", e);
                Thread.Sleep(100);
            }
        }

        return File.GetLastWriteTimeUtc(filename);
    }

    public static T? DeserializeFromFile<T>(string filename)
    {
        if (!File.Exists(filename))
            return default;

        var content = File.ReadAllBytes(filename);
        return DeserializeBytes<T>(content);
    }

    [JsonSerializable(typeof(Settings))]
    [JsonSerializable(typeof(Settings.SelectedTheme))]
    [JsonSerializable(typeof(InputMacro))]
    [JsonSerializable(typeof(InputMap))]
    [JsonSerializable(typeof(PlayerPad.Kind))]
    [JsonSerializable(typeof(Buttons))]
    [JsonSerializable(typeof(SOCD))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Serialization | JsonSourceGenerationMode.Metadata,
            WriteIndented = true,
            PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
            Converters =
            [
                typeof(JsonStringEnumConverter<Direction>),
                typeof(JsonStringEnumConverter<ButtonName>),
                typeof(JsonStringEnumConverter<Settings.DirectionSources>),
                typeof(JsonStringEnumConverter<PlayerPad.Kind>),
                typeof(JsonStringEnumConverter<Buttons>),
                typeof(JsonStringEnumConverter<SOCD>),
            ]),
    ]
    public partial class JsonContext : JsonSerializerContext;
}
