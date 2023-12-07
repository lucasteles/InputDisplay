namespace InputDisplay;

public static class Extensions
{
    public static Texture2D LoadTexture(this ContentManager content, params string[] path) =>
        content.Load<Texture2D>(ContentPath.Combine(path));

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
}
