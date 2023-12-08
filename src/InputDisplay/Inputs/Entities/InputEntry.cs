using InputDisplay.Config;
using InputDisplay.Theme;

namespace InputDisplay.Inputs.Entities;

public class InputEntry
{
    const int MaxHoldingFrames = 99;
    const string MaxFramesString = "99";

    public int HoldingFrames { get; private set; } = 1;

    public GameInput.State State { get; init; }

    public void IncrementFrame() => HoldingFrames = Math.Min(HoldingFrames + 1, MaxHoldingFrames);

    public void Draw(GameConfig config, SpriteFont font, SpriteBatch batch, Vector2 position)
    {
        var paddingLeft = Vector2.UnitX * config.SpaceBetweenInputs * 3;
        Vector2 offset = position + paddingLeft;
        var theme = config.Theme;

        var commandDir = config.Horizontal ? Vector2.UnitY : Vector2.UnitX;

        if (config.ShowFrames)
        {
            var frameString = HoldingFrames.ToString().PadLeft(2);
            var stringSize = font.MeasureString(MaxFramesString);
            var scale = config.IconSize / stringSize.Length();
            var padTop = config.IconSize * 0.15f * Vector2.UnitY;

            batch.DrawString(
                font, frameString, offset + padTop, Color.Black,
                0, Vector2.Zero, scale + 0.08f, SpriteEffects.None, 0.1f
            );

            batch.DrawString(
                font, frameString, offset + padTop, Color.White, 0,
                Vector2.Zero, scale, SpriteEffects.None, 0.5f
            );

            var space = (stringSize * scale) + new Vector2(config.SpaceBetweenInputs * 3);
            offset += commandDir * space;
        }

        if (theme.GetTexture(State.Stick.Direction) is { } dirTexture)
        {
            var dirColor = State.Stick.Holding && config.ShadowHolding ? Color.LightGray : Color.White;
            Rectangle dirRect = new(
                (int)offset.X, (int)offset.Y,
                config.IconSize, config.IconSize
            );
            batch.Draw(dirTexture, dirRect, dirColor);
        }

        offset += commandDir * (config.IconSize + config.SpaceBetweenInputs + config.DirectionSpace);

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
            Rectangle btnRect = new(
                (int)offset.X, (int)offset.Y,
                config.IconSize, config.IconSize
            );
            batch.Draw(multiTexture, btnRect, State.HasNoPressed ? Color.Gray : Color.White);
            return;
        }

        foreach (var btn in currentButtons.Order().Distinct())
        {
            if (theme.GetTexture(btn) is not { } texture)
                if (config.FallbackTheme?.GetTexture(btn) is { } fallbackTexture)
                    texture = fallbackTexture;
                else
                    texture = ThemeManager.UnknownButton;

            Rectangle btnRect = new(
                (int)offset.X, (int)offset.Y,
                config.IconSize, config.IconSize
            );

            var color = holding.Contains(btn) ? Color.Gray : Color.White;
            batch.Draw(texture, btnRect, color);
            offset += commandDir * (config.IconSize + config.SpaceBetweenInputs);
        }

        void Check(GameInput.Button button, ButtonName name)
        {
            if (!button.Active) return;
            if (!config.ShadowHolding && button.Status is GameInput.ButtonStatus.Holding)
                return;

            var buttons = config.GetMacro(name);
            for (var index = 0; index < buttons.Length; index++)
            {
                var btn = buttons[index];
                if (button.Status is GameInput.ButtonStatus.Holding) holding.Add(btn);
                currentButtons.Add(btn);
            }
        }
    }
}
