namespace Utilities;


public static class StringExtensions
{
    public static void Deconstruct(this string[] parts, out string first, out string second)
    {
        first = parts[0];
        second = (parts.Length > 1 ? parts[1] : null)!;
    }
}