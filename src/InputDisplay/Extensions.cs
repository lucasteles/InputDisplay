namespace InputDisplay;

public static class Extensions
{
    public static Texture2D LoadTexture(this ContentManager content, params string[] path) =>
        content.Load<Texture2D>(ContentPath.Combine(path));
}
