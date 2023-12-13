namespace Utilities;

public static class DictionaryExtensions
{
    public static void ApplyWithDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue, TValue> op, TValue defaultValue = default)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = defaultValue;
        }
        
        dictionary[key] = op(dictionary[key]);
    }
}