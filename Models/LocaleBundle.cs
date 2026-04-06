using System.Collections.Generic;

namespace VolkLoaderAvalonia.Models;

public sealed class LocaleBundle
{
    public Dictionary<string, string> Values { get; } = new();

    public string this[string key]
    {
        get
        {
            return Values.TryGetValue(key, out var value) ? value : key;
        }
    }
}
