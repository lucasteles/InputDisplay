#pragma warning disable S101

namespace InputDisplay.Inputs;

/// <summary>
/// Simultaneous Opposing Cardinal Directions.
/// </summary>
public enum SOCDMode
{
    Neutral,
    UpPriority,
    LastPriority,
    Bypass,
}

static class SOCD
{
    public static bool HasHorizontalOpposingDirections(Direction direction) =>
        direction.HasFlag(Direction.Backward) && direction.HasFlag(Direction.Forward);

    public static bool HasVerticalOpposingDirections(Direction direction) =>
        direction.HasFlag(Direction.Up) && direction.HasFlag(Direction.Down);

    public static bool HasOpposingDirections(Direction direction) =>
        HasHorizontalOpposingDirections(direction) || HasVerticalOpposingDirections(direction);

    public static bool IsSingle(SOCDMode mode, Direction dir) =>
        !(mode is SOCDMode.Bypass && HasOpposingDirections(dir));

    public static Direction Clean(SOCDMode mode, Direction direction, GameInput.Stick last)
    {
        if (mode is SOCDMode.Bypass)
            return direction;

        if (HasHorizontalOpposingDirections(direction))
            switch (mode)
            {
                case SOCDMode.Neutral or SOCDMode.UpPriority:
                    direction &= ~Direction.Forward;
                    direction &= ~Direction.Backward;
                    break;

                case SOCDMode.LastPriority:
                    if (direction == last.Raw)
                        direction = last.Direction;
                    else if (HasHorizontalOpposingDirections(last.Raw))
                    {
                        Direction lastUsed = Direction.Neutral;
                        if (last.Direction.HasFlag(Direction.Backward))
                            lastUsed = Direction.Backward;
                        else if (last.Direction.HasFlag(Direction.Forward))
                            lastUsed = Direction.Forward;

                        direction &= ~(Direction.Backward | Direction.Forward);
                        direction |= lastUsed;
                    }
                    else if (last.Raw.HasFlag(Direction.Backward) && !last.Raw.HasFlag(Direction.Forward))
                        direction &= ~Direction.Backward;
                    else if (last.Raw.HasFlag(Direction.Forward) && !last.Raw.HasFlag(Direction.Backward))
                        direction &= ~Direction.Forward;

                    break;
            }

        if (HasVerticalOpposingDirections(direction))
            switch (mode)
            {
                case SOCDMode.Neutral:
                    direction &= ~Direction.Up;
                    direction &= ~Direction.Down;
                    break;
                case SOCDMode.UpPriority:
                    direction &= ~Direction.Down;
                    break;
                case SOCDMode.LastPriority:
                    if (direction == last.Raw)
                        direction = last.Direction;
                    else if (HasVerticalOpposingDirections(last.Raw))
                    {
                        Direction lastUsed = Direction.Neutral;
                        if (last.Direction.HasFlag(Direction.Up))
                            lastUsed = Direction.Up;
                        else if (last.Direction.HasFlag(Direction.Down))
                            lastUsed = Direction.Down;

                        direction &= ~(Direction.Up | Direction.Down);
                        direction |= lastUsed;
                    }

                    if (last.Raw.HasFlag(Direction.Up) && !last.Raw.HasFlag(Direction.Down))
                        direction &= ~Direction.Up;
                    else if (last.Raw.HasFlag(Direction.Down) && !last.Raw.HasFlag(Direction.Up))
                        direction &= ~Direction.Down;
                    break;
            }

        return direction;
    }

    public static IEnumerable<Direction> GetComponents(SOCDMode mode, GameInput.Stick stick)
    {
        var direction = stick.Raw;

        if (mode is not SOCDMode.Bypass)
            yield return direction;

        if (direction.HasFlag(Direction.Backward))
            yield return Direction.Backward;

        if (direction.HasFlag(Direction.Up))
            yield return Direction.Up;

        if (direction.HasFlag(Direction.Down))
            yield return Direction.Down;

        if (direction.HasFlag(Direction.Forward))
            yield return Direction.Forward;
    }
}
