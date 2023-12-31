using InputDisplay.Config;
using InputDisplay.Inputs;

namespace InputDisplay;

public static class Extensions
{
    public static Texture2D LoadTexture(this ContentManager content, params string[] path) =>
        content.Load<Texture2D>(ContentPath.Combine(path));

    public static (string, Lazy<Texture2D>) LazyLoadTexture(this ContentManager content, params string[] path)
    {
        var fullPath = ContentPath.Combine(path);
        return (fullPath, new(() => content.LoadTexture(fullPath)));
    }

    public static Texture2D LoadTexturePng(this ContentManager content, params string[] path)
    {
        if (content.ServiceProvider.GetService(typeof(IGraphicsDeviceManager)) is not GraphicsDeviceManager
            {
                GraphicsDevice: var graphicsDevice,
            })
            throw new InvalidOperationException("Invalid graphics device");

        var fullPath = ContentPath.Combine([content.RootDirectory, .. path]);
        fullPath = Path.ChangeExtension(fullPath, "png");

        using FileStream stream = new(fullPath, FileMode.Open);
        var texture = Texture2D.FromStream(graphicsDevice, stream);
        texture.Name = ContentPath.Combine(path);
        return texture;
    }

    public static string ToFieldLabel(this string text) => $"{text}: ";

    public static Settings.DirectionSources ChangeFlag(this Settings.DirectionSources flags,
        Settings.DirectionSources flag, bool value = true) =>
        value ? flags | flag : flags & ~flag;

    public static bool IsMultiple(this ButtonName name) =>
        name is not (
            ButtonName.None
            or ButtonName.LP or ButtonName.MP or ButtonName.HP or ButtonName.PP
            or ButtonName.LK or ButtonName.MK or ButtonName.HK or ButtonName.KK
            );

    public static bool Exists<T>(this T[] array, Predicate<T> match) => Array.Exists(array, match);
    public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);
    public static bool IsEmpty(this string? value) => string.IsNullOrWhiteSpace(value);
    public static bool IsNonEmpty(this string? value) => value?.IsEmpty() == false;
    public static bool IsWide(this Texture2D tex) => tex.Width > tex.Height;

    public static float GetRatioScale(this Texture2D tex, float size) =>
        size / (tex.IsWide() ? tex.Height : tex.Width);
}
