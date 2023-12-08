namespace InputDisplay.Util;

public class Cycle<T>(IEnumerable<T> values, int currentIndex = 0)
{
    readonly T[] values = values.ToArray();

    public T Current => values[currentIndex];

    public void SetIndex(int index)
    {
        currentIndex = index;
        CheckIndex();
    }

    public T Next()
    {
        currentIndex++;
        CheckIndex();
        return values[currentIndex];
    }

    public T Previous()
    {
        currentIndex--;
        CheckIndex();
        return values[currentIndex];
    }

    void CheckIndex()
    {
        if (currentIndex < 0)
            currentIndex = values.Length - 1;
        else if (currentIndex >= values.Length)
            currentIndex = 0;
    }
}
