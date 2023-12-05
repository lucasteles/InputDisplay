namespace InputDisplay;

public static class Extensions
{
    public static string CombinePath(this IReadOnlyList<string> path) =>
        path is [var oneSegment]
            ? oneSegment
            : path
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(Path.Combine)
                .Replace('/', '\\');

    public static Texture2D LoadTexture(this ContentManager content, params string[] path) =>
        content.Load<Texture2D>(path.CombinePath());
}
