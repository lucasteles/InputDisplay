namespace InputDisplay.Util;

public class GameResources(ContentManager content)
{
    public OutlineBitmapFont Font = new(content,
        "fonts/monofonto.fnt", "fonts/monofonto_0",
        "fonts/monofonto_outline.fnt", "fonts/monofonto_outline_0"
    );
}
