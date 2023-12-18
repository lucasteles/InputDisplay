using System.Xml.Serialization;

#nullable disable
namespace InputDisplay.Util;

public class OutlineBitmapFont
{
    readonly BitmapFont face;
    readonly BitmapFont outline;

    public OutlineBitmapFont(ContentManager c,
        string fontDescription, string textureFile,
        string outlineDescription, string outlineTextureFile)
    {
        face = new(c, fontDescription, textureFile);
        outline = new(c, outlineDescription, outlineTextureFile);
    }

    public void Draw(SpriteBatch spriteBatch, string message,
        Color foreColor, Color outColor, Vector2 pos, float scale = 1
    )
    {
        outline.Draw(spriteBatch, message, outColor, pos, scale, 1);
        face.Draw(spriteBatch, message, foreColor, pos, scale, 0);
    }

    public Vector2 MeasureString(string message) => outline.MeasureString(message);
}

public class BitmapFont
{
    readonly FontRenderer fontRenderer;

    public BitmapFont(ContentManager c, string fontDescription, string textureFile)
    {
        var fontFilePath = Path.Combine(c.RootDirectory, fontDescription);
        var fontFile = Load(fontFilePath);
        var texture = c.Load<Texture2D>(textureFile);
        fontRenderer = new(fontFile, texture);
    }

    public void Draw(SpriteBatch spriteBatch, string message, Color color, Vector2 pos, float scale = 1,
        float layer = 0) =>
        fontRenderer.DrawText(spriteBatch, message, color, pos, scale, layer);

    public Vector2 MeasureString(string message) =>
        new(fontRenderer.LetterSize.X * message.Length, fontRenderer.LetterSize.Y);

    static FontFile Load(string filename)
    {
        XmlSerializer deserializer = new(typeof(FontFile));
        TextReader textReader = new StreamReader(filename);
        var file = (FontFile)deserializer.Deserialize(textReader);
        textReader.Close();
        return file;
    }
}

public class FontRenderer
{
    public Dictionary<char, FontChar> Chars { get; private set; }
    readonly Texture2D texture;

    public Vector2 LetterSize { get; }

    public FontRenderer(FontFile fontFile, Texture2D fontTexture)
    {
        texture = fontTexture;
        Chars = [];

        foreach (var fontCharacter in fontFile.Chars)
        {
            var c = (char)fontCharacter.Id;
            Chars.Add(c, fontCharacter);
        }

        LetterSize = new(
            fontFile.Chars.Max(x => x.Width),
            fontFile.Chars.Max(x => x.Height)
        );
    }

    public void DrawText(SpriteBatch spriteBatch, string text, Color color, Vector2 position, float scale, float layer)
    {
        var dx = position.X;
        foreach (var c in text)
        {
            if (!Chars.TryGetValue(c, out var fc)) continue;
            Rectangle sourceRectangle = new(fc.X, fc.Y, fc.Width, fc.Height);
            var offset = fc.Offset * scale;
            Vector2 pos = new(dx + offset.X, position.Y + offset.Y);

            spriteBatch.Draw(texture,
                pos, sourceRectangle, color, 0, Vector2.Zero,
                scale, SpriteEffects.None, layer
            );

            dx += fc.XAdvance * scale;
        }
    }
}

[Serializable]
[XmlRoot("font")]
public class FontFile
{
    [XmlElement("info")]
    public FontInfo Info { get; set; }

    [XmlElement("common")]
    public FontCommon Common { get; set; }

    [XmlArray("pages")]
    [XmlArrayItem("page")]
    public List<FontPage> Pages { get; set; }

    [XmlArray("chars")]
    [XmlArrayItem("char")]
    public List<FontChar> Chars { get; set; }

    [XmlArray("kernings")]
    [XmlArrayItem("kerning")]
    public List<FontKerning> Kernings { get; set; }
}

[Serializable]
public class FontInfo
{
    [XmlAttribute("face")]
    public string Face { get; set; }

    [XmlAttribute("size")]
    public int Size { get; set; }

    [XmlAttribute("bold")]
    public int Bold { get; set; }

    [XmlAttribute("italic")]
    public int Italic { get; set; }

    [XmlAttribute("charset")]
    public string CharSet { get; set; }

    [XmlAttribute("unicode")]
    public int Unicode { get; set; }

    [XmlAttribute("stretchH")]
    public int StretchHeight { get; set; }

    [XmlAttribute("smooth")]
    public int Smooth { get; set; }

    [XmlAttribute("aa")]
    public int SuperSampling { get; set; }

    Rectangle padding;

    [XmlAttribute("padding")]
    public string Padding
    {
        get => $"{padding.X},{padding.Y},{padding.Width},{padding.Height}";
        set
        {
            var padded = value.Split(',');
            padding = new Rectangle(Convert.ToInt32(padded[0]), Convert.ToInt32(padded[1]),
                Convert.ToInt32(padded[2]), Convert.ToInt32(padded[3]));
        }
    }

    Point spacing;

    [XmlAttribute("spacing")]
    public string Spacing
    {
        get => $"{spacing.X},{spacing.Y}";
        set
        {
            var spaced = value.Split(',');
            spacing = new Point(Convert.ToInt32(spaced[0]), Convert.ToInt32(spaced[1]));
        }
    }

    [XmlAttribute("outline")]
    public int OutLine { get; set; }
}

[Serializable]
public class FontCommon
{
    [XmlAttribute("lineHeight")]
    public int LineHeight { get; set; }

    [XmlAttribute("base")]
    public int Base { get; set; }

    [XmlAttribute("scaleW")]
    public int ScaleW { get; set; }

    [XmlAttribute("scaleH")]
    public int ScaleH { get; set; }

    [XmlAttribute("pages")]
    public int Pages { get; set; }

    [XmlAttribute("packed")]
    public int Packed { get; set; }

    [XmlAttribute("alphaChnl")]
    public int AlphaChannel { get; set; }

    [XmlAttribute("redChnl")]
    public int RedChannel { get; set; }

    [XmlAttribute("greenChnl")]
    public int GreenChannel { get; set; }

    [XmlAttribute("blueChnl")]
    public int BlueChannel { get; set; }
}

[Serializable]
public class FontPage
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("file")]
    public string File { get; set; }
}

[Serializable]
public class FontChar
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("x")]
    public int X { get; set; }

    [XmlAttribute("y")]
    public int Y { get; set; }

    [XmlAttribute("width")]
    public int Width { get; set; }

    [XmlAttribute("height")]
    public int Height { get; set; }

    [XmlAttribute("xoffset")]
    public int XOffset { get; set; }

    [XmlAttribute("yoffset")]
    public int YOffset { get; set; }

    [XmlAttribute("xadvance")]
    public int XAdvance { get; set; }

    [XmlAttribute("page")]
    public int Page { get; set; }

    [XmlAttribute("chnl")]
    public int Channel { get; set; }


    [XmlIgnore]
    public Vector2 Offset => new(XOffset, YOffset);
}

[Serializable]
public class FontKerning
{
    [XmlAttribute("first")]
    public int First { get; set; }

    [XmlAttribute("second")]
    public int Second { get; set; }

    [XmlAttribute("amount")]
    public int Amount { get; set; }
}
