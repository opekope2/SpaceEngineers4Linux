public static class Extensions
{
    public static void Replace(ref string[] values, char oldChar, char newChar)
    {
        for (int i = 0, l = values.Length; i < l; i++)
        {
            values[i] = values[i].Replace(oldChar, newChar);
        }
    }
}
