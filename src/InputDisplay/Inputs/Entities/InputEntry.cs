using InputDisplay.Config;
using InputDisplay.Themes;

namespace InputDisplay.Inputs.Entities;

public class InputEntry
{
    const int MaxHoldingFrames = 99;
    const string MaxFramesString = "99";

    public int HoldingFrames { get; private set; } = 1;

    public GameInput.State State { get; init; }

    public void IncrementFrame() => HoldingFrames = Math.Min(HoldingFrames + 1, MaxHoldingFrames);

    readonly HashSet<ButtonName> holding = new();
    readonly HashSet<ButtonName> pressed = new();
    readonly HashSet<ButtonName> fallback = new();
    readonly HashSet<ButtonName> currentButtons = new();

    public void Draw(
        Settings config,
        Theme theme,
        SpriteFont font,
        SpriteBatch batch,
        Vector2 position
    )
    {
        var paddingLeft = Vector2.UnitX * config.SpaceBetweenInputs * 3;
        Vector2 offset = position + paddingLeft;

        var commandDir = config.IsHorizontal ? Vector2.UnitY : Vector2.UnitX;

        if (!config.FramesAfter) DrawFrames();

        DrawDirection();
        DrawButtons();

        if (config.FramesAfter) DrawFrames();

        void DrawFrames()
        {
            if (!config.ShowFrames) return;
            var frameString = HoldingFrames.ToString().PadLeft(2);
            var stringSize = font.MeasureString(MaxFramesString);
            var scale = config.IconSize / stringSize.Length();

            var marginX = config.SpaceBetweenInputs * 3;
            Vector2 padding = new(
                config.FramesAfter ? marginX : 0,
                config.IconSize * 0.10f
            );

            batch.DrawString(
                font, frameString, offset + padding, Color.White, 0,
                Vector2.Zero, scale, SpriteEffects.None, 0.5f
            );

            var space = (stringSize * scale) + new Vector2(marginX);
            offset += commandDir * space;
        }

        void DrawDirection()
        {
            var step = commandDir * (config.IconSize + config.SpaceBetweenInputs + config.DirectionSpace);
            if (
                (
                    (config.ShowNeutralIcon && State.Stick.Direction is Direction.Neutral)
                    || State.Stick.Direction is not Direction.Neutral
                )
                && theme.GetTexture(State.Stick.Direction) is { } dirTexture
            )
            {
                var dirColor = State.Stick.Holding && config.ShadowHolding ? Color.LightGray : Color.White;
                Rectangle dirRect = new(
                    (int)offset.X, (int)offset.Y,
                    config.IconSize, config.IconSize
                );
                batch.Draw(dirTexture, dirRect, dirColor);
                offset += step;
            }
        }

        void DrawButtons()
        {
            holding.Clear();
            pressed.Clear();
            fallback.Clear();
            currentButtons.Clear();

            CheckButton(State.LP, ButtonName.LP);
            CheckButton(State.MP, ButtonName.MP);
            CheckButton(State.LP, ButtonName.LP);
            CheckButton(State.MP, ButtonName.MP);
            CheckButton(State.HP, ButtonName.HP);
            CheckButton(State.PP, ButtonName.PP);
            CheckButton(State.LK, ButtonName.LK);
            CheckButton(State.MK, ButtonName.MK);
            CheckButton(State.HK, ButtonName.HK);
            CheckButton(State.KK, ButtonName.KK);

#pragma warning disable S2583
            if (currentButtons.Count is 0)
#pragma warning restore S2583
                return;

            var combined = ButtonName.None;
            foreach (var b in currentButtons)
                combined |= b;

            if (combined.IsMultiple() && theme.GetTexture(combined) is { } multiTexture)
            {
                Rectangle btnRect = new(
                    (int)offset.X, (int)offset.Y,
                    config.IconSize, config.IconSize
                );
                batch.Draw(multiTexture, btnRect,
                    State.HasNoPressed && config.ShadowHolding ? Color.Gray : Color.White);
                return;
            }

            foreach (var btn in currentButtons.Order().Distinct())
            {
                if (fallback.Contains(btn) || theme.GetTexture(btn) is not { } texture)
                    if (theme.Fallback?.GetTexture(btn) is { } fallbackTexture)
                        texture = fallbackTexture;
                    else
                        texture = ThemeManager.UnknownButton;

                Rectangle btnRect = new(
                    (int)offset.X, (int)offset.Y,
                    config.IconSize, config.IconSize
                );

                var color = holding.Contains(btn) && !pressed.Contains(btn) ? Color.Gray : Color.White;
                batch.Draw(texture, btnRect, color);
                offset += commandDir * (config.IconSize + config.SpaceBetweenInputs);
            }
        }

        void CheckButton(GameInput.Button button, ButtonName name)
        {
            if (!button.Active) return;

            var buttons = theme.GetMacro(name, config.Macros);

            if (buttons.Length is 0)
            {
                fallback.Add(name);
                buttons = [name];
            }

            for (var index = 0; index < buttons.Length; index++)
            {
                var btn = buttons[index];

                if (config.ShadowHolding)
                    switch (button.Status)
                    {
                        case GameInput.ButtonStatus.Holding:
                            holding.Add(btn);
                            break;
                        case GameInput.ButtonStatus.Pressed:
                            pressed.Add(btn);
                            break;
                    }

                currentButtons.Add(btn);
            }
        }
    }
}
