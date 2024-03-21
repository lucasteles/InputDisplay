using System.Diagnostics;
using InputDisplay.Config;
using InputDisplay.Themes;

namespace InputDisplay.Inputs.Drawable;

public class InputEntry
{
    const int MaxHoldingFrames = 99;

    public int HoldingFrames { get; private set; } = 1;

    public GameInput.State State { get; init; }

    public void IncrementFrame() => HoldingFrames = Math.Min(HoldingFrames + 1, MaxHoldingFrames);

    readonly HashSet<ButtonName> holding = [];
    readonly HashSet<ButtonName> pressed = [];
    readonly HashSet<ButtonName> fallback = [];
    readonly SortedSet<ButtonName> currentButtons = [];


    public void Draw(
        Settings config,
        Theme theme,
        OutlineBitmapFont font,
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
            var stringSize = font.MeasureString(frameString);
            var len = stringSize.Length();
            var scale = config.IconSize / len;

            var marginX = config.SpaceBetweenInputs * 3 * scale;
            Vector2 padding = new(
                config.FramesAfter ? marginX : 0,
                (config.IconSize - 6 - (stringSize.Y * scale)) / 2f
            );

            font.Draw(batch, frameString, Color.White, Color.Black, offset + padding, scale);

            var space = (stringSize * scale) + new Vector2(marginX);
            offset += commandDir * space;
        }

        void DrawDirection()
        {
            if (config.EnabledDirections is Settings.DirectionSources.None)
            {
                offset += commandDir * (config.SpaceBetweenInputs + config.DirectionSpace);
                return;
            }

            var step = commandDir * (config.IconSize + config.SpaceBetweenInputs);

            if (
                (config.ShowNeutralIcon && State.Stick.Direction is Direction.Neutral)
                || State.Stick.Direction is not Direction.Neutral
            )
            {
                if (SOCD.IsSingle(config.SOCD, State.Stick.Direction))
                    DrawDirectionComponent(State.Stick.Direction);
                else
                {
                    foreach (var stickDir in SOCD.GetComponents(config.SOCD, State.Stick))
                        DrawDirectionComponent(stickDir);
                }

                offset += commandDir * config.DirectionSpace;
            }

            void DrawDirectionComponent(Direction stickDirection)
            {
                if (theme.GetTexture(stickDirection) is not { } dirTexture)
                    return;

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
            {
#pragma warning restore S2583
                if (!theme.Buttons.HasEmptyImage || theme.GetTexture(ButtonName.None) is not { } emptyTexture)
                    return;

                var scale = emptyTexture.GetRatioScale(config.IconSize);
                batch.Draw(emptyTexture, offset, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                return;
            }

            var combined = ButtonName.None;
            foreach (var b in currentButtons)
                combined |= b;

            if (combined.IsMultiple() && theme.GetTexture(combined) is { } multiTexture)
            {
                var scale = multiTexture.GetRatioScale(config.IconSize);
                var color = State.HasNoPressed && config.ShadowHolding ? Color.Gray : Color.White;
                batch.Draw(multiTexture, offset, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                return;
            }

            foreach (var btn in currentButtons)
            {
                if (fallback.Contains(btn) || theme.GetTexture(btn) is not { } texture)
                    if (theme.Fallback?.GetTexture(btn) is { } fallbackTexture)
                        texture = fallbackTexture;
                    else
                        texture = ThemeManager.UnknownButton;

                var scale = texture.GetRatioScale(config.IconSize);
                var color = holding.Contains(btn) && !pressed.Contains(btn) ? Color.Gray : Color.White;

                batch.Draw(texture, offset, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                offset += commandDir * ((texture.Width * scale) + config.SpaceBetweenInputs);
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
                        case GameInput.ButtonStatus.Unpressed:
                        case GameInput.ButtonStatus.Released:
                        default:
                            break;
                    }

                currentButtons.Add(btn);
            }
        }
    }
}

sealed class TimeEntry<T>(T value) : IComparable<TimeEntry<T>>
{
    public T Value { get; } = value;
    public long Time { get; set; }

    public void Update() => Time = Stopwatch.GetTimestamp();

    public int CompareTo(TimeEntry<T>? other) => Time.CompareTo(other?.Time);
}
