using Microsoft.Xna.Framework.Input;

namespace InputDisplay.Inputs;

public class InputBuffer(GameConfig config, SpriteFont font)
{
    readonly GameInput gameInput = new();
    readonly List<InputEntry> entries = new(config.MaxEntries);
    public InputEntry? Last => entries.LastOrDefault();

    public void Update(GamePadState state)
    {
        gameInput.Update(state);
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

    public void Draw(SpriteBatch batch, in Rectangle window)
    {
        var dir = config.InvertHistory ? -1 : 1;
        var step = config.Horizontal
            ? new Vector2(config.SpaceBetweenCommands + config.IconSize, 0)
            : new Vector2(0, config.SpaceBetweenCommands + config.IconSize);

        var padding = new Vector2(config.SpaceBetweenInputs, config.SpaceBetweenCommands) * 2;

        Vector2 pos = config switch
        {
            { Horizontal: true, InvertHistory: true } => new(
                window.Size.X - step.X - padding.X, padding.Y
            ),
            { Horizontal: false, InvertHistory: true } => new(
                padding.X, window.Size.Y - step.Y - padding.Y
            ),
            _ => padding,
        };

        for (var i = entries.Count - 1; i >= 0; i--)
            DrawButton(i);

        void DrawButton(int i)
        {
            var entry = entries[i];
            if (config.HideButtonRelease && (entry.State.IsNeutralOnly || entry.State.HasNoPressed))
                return;

            entry.Draw(config, font, batch, pos);
            pos += step * dir;
        }
    }
}
