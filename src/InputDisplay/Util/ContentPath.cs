namespace InputDisplay.Util;

public static class ContentPath
{
    public static string CombineRaw(params string[] path) =>
        path is [var oneSegment]
            ? oneSegment
            : path
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(Path.Combine);

    public static string Combine(params string[] path) =>
        CombineRaw(path).Replace('/', '\\');
}
