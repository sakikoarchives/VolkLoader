using System;
using System.IO;
using System.Text.Json;
using VolkLoaderAvalonia.Models;

namespace VolkLoaderAvalonia.Services;

public sealed class SettingsService
{
    private const string SettingsFileName = "user-settings.json";

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public UserSettings Load(string baseDirectory)
    {
        var fullPath = Path.Combine(baseDirectory, SettingsFileName);
        if (!File.Exists(fullPath))
        {
            return new UserSettings();
        }

        try
        {
            var content = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<UserSettings>(content, _serializerOptions) ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    public void Save(string baseDirectory, UserSettings settings)
    {
        try
        {
            var fullPath = Path.Combine(baseDirectory, SettingsFileName);
            var content = JsonSerializer.Serialize(settings, _serializerOptions);
            File.WriteAllText(fullPath, content);
        }
        catch
        {
            // Deliberately ignored: the UI should stay responsive even if persistence fails.
        }
    }
}
