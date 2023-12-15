namespace Utilities;

public static class DictionaryExtensions
{
    public static void ApplyWithDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue, TValue> op, TValue defaultValue)
    {
        dictionary.TryAdd(key, defaultValue);

        dictionary[key] = op(dictionary[key]);
    }
}