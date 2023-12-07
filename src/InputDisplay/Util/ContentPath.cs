namespace InputDisplay.Util;

public static class ContentPath
{
    public static string Combine(params string[] path) =>
        path is [var oneSegment]
            ? oneSegment
            : path
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(Path.Combine)
                .Replace('/', '\\');
}
