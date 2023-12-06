using InputDisplay.Theme;

namespace InputDisplay.Inputs;

public class InputEntry
{
    const int MaxHoldingFrames = 99;
    const string MaxFramesString = "99";

    public int HoldingFrames { get; private set; } = 1;

    public GameInput.State State { get; init; }

    public void IncrementFrame() => HoldingFrames = Math.Min(HoldingFrames + 1, MaxHoldingFrames);

    public void Draw(InputConfig config, SpriteFont font, SpriteBatch batch, Vector2 position)
    {
        Rectangle rect = new(
            (int)position.X,
            (int)position.Y,
            config.ButtonIconSize, config.ButtonIconSize
        );

        var theme = config.Theme;

        if (config.ShowFrames)
        {
            var frameString = HoldingFrames.ToString().PadLeft(2);
            var stringSize = font.MeasureString(MaxFramesString);
            var scale = config.MaxIconSize / stringSize.Y * 0.9f;

            Vector2 textMiddlePoint = new(
                config.SpaceBetweenInputs * -2,
                ((float)config.MaxIconSize / 2) - (stringSize.Y / 2)
            );

            batch.DrawString(font, frameString, position, Color.Black, 0,
                textMiddlePoint,
                scale + 0.05f, SpriteEffects.None, 0.5f);

            batch.DrawString(font, frameString, position, Color.White, 0,
                textMiddlePoint, scale, SpriteEffects.None, 1f);

            if (config.Horizontal)
                rect.Y += (int)(stringSize.Y * scale * 1.2) + (config.SpaceBetweenInputs * 3);
            else
                rect.X += (int)(stringSize.X * scale * 1.2) + (config.SpaceBetweenInputs * 3);
        }

        if (theme.GetTexture(State.Stick.Direction) is { } dirTexture)
        {
            var stickColor = State.Stick.Holding && config.ShadowHolding ? Color.Gray : Color.White;
            batch.Draw(dirTexture, rect, stickColor);
        }

        if (config.Horizontal)
            rect.Y += config.ValidDirectionSize + config.SpaceBetweenInputs;
        else
            rect.X += config.ValidDirectionSize + config.SpaceBetweenInputs;

        List<ButtonName> holding = new();
        List<ButtonName> currentButtons = new();

        Check(State.LP, ButtonName.LP);
        Check(State.MP, ButtonName.MP);
        Check(State.LP, ButtonName.LP);
        Check(State.MP, ButtonName.MP);
        Check(State.HP, ButtonName.HP);
        Check(State.PP, ButtonName.PP);
        Check(State.LK, ButtonName.LK);
        Check(State.MK, ButtonName.MK);
        Check(State.HK, ButtonName.HK);
        Check(State.KK, ButtonName.KK);

#pragma warning disable S2583
        if (currentButtons.Count is 0)
#pragma warning restore S2583
            return;

        var combined = ButtonName.None;
        foreach (var b in currentButtons)
            combined |= b;

        if (theme.GetTexture(combined) is { } multiTexture)
        {
            batch.Draw(multiTexture, rect, State.HasNoPressed ? Color.Gray : Color.White);
            return;
        }

        foreach (var btn in currentButtons.Order().Distinct())
        {
            if (theme.GetTexture(btn) is not { } texture)
                if (config.FallbackTheme?.GetTexture(btn) is { } fallbackTexture)
                    texture = fallbackTexture;
                else
                    texture = ThemeManager.UnknownButton;

            var color = holding.Contains(btn) ? Color.Gray : Color.White;
            batch.Draw(texture, rect, color);

            if (config.Horizontal)
                rect.Y += config.ButtonIconSize + config.SpaceBetweenInputs;
            else
                rect.X += config.ButtonIconSize + config.SpaceBetweenInputs;
        }

        void Check(GameInput.Button button, ButtonName name)
        {
            if (!button.Active) return;
            if (!config.ShadowHolding && button.Status is GameInput.ButtonStatus.Holding)
                return;

            var buttons = theme.GetMapped(name);
            for (var index = 0; index < buttons.Length; index++)
            {
                var btn = buttons[index];
                if (button.Status is GameInput.ButtonStatus.Holding) holding.Add(btn);
                currentButtons.Add(btn);
            }
        }
    }
}
