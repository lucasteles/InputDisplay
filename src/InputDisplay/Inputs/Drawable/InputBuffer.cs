using InputDisplay.Config;
using InputDisplay.Themes;

namespace InputDisplay.Inputs.Drawable;

public class InputBuffer(Settings config)
{
    readonly GameInput gameInput = new();
    readonly List<InputEntry> entries = new(config.MaxEntries);
    public InputEntry? Last => entries.LastOrDefault();

    public void Update(PlayerPad pad)
    {
        gameInput.Update(pad, config.InputMap, config.EnabledDirections);
        var controllerState = gameInput.CurrentState;

        if (Last is { } last)
        {
            if (last.State.EquivalentTo(controllerState))
            {
                last.IncrementFrame();
                return;
            }

            if (config.AutoCorrectMultiple
                && last.HoldingFrames is 1
                && last.State.HasPressed
                && controllerState.HasPressed
               )
                controllerState.Combine(last.State);
        }

        entries.Add(new()
        {
            State = controllerState,
        });

        ShrinkBuffer();
    }

    public void Clear() => entries.Clear();

    void ShrinkBuffer()
    {
        if (entries.Count > config.MaxEntries)
            entries.RemoveAt(0);
    }

    public void Draw(SpriteBatch batch, Theme theme, GameResources resources, in Rectangle window)
    {
        var dir = config.InvertHistory ? -1 : 1;
        var step = config.IsHorizontal
            ? new Vector2(config.SpaceBetweenCommands + config.IconSize, 0)
            : new Vector2(0, config.SpaceBetweenCommands + config.IconSize);

        var padding = new Vector2(config.SpaceBetweenInputs, config.SpaceBetweenCommands) * 2;
        var font = resources.Font;

        Vector2 pos = config switch
        {
            { IsHorizontal: true, InvertHistory: true } => new(
                window.Size.X - step.X - padding.X, padding.Y
            ),
            { IsHorizontal: false, InvertHistory: true } => new(
                padding.X, window.Size.Y - step.Y - padding.Y
            ),
            _ => padding,
        };

        for (var i = entries.Count - 1; i >= 0; i--)
        {
            var entry = entries[i];
            if (config.HideButtonRelease && (
                    entry.State.IsNeutralOnly
                    || entry.State.HasNoPressed
                    || (config.EnabledDirections is Settings.DirectionSources.None && entry.State.IsDirectionOnly)))
                continue;

            entry.Draw(config, theme, font, batch, pos);
            pos += step * dir;
        }
    }
}
