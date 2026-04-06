using System;
using System.IO;
using System.Text.Json;
using VolkLoaderAvalonia.Models;

namespace VolkLoaderAvalonia.Services;

public sealed class CatalogService
{
    private const string CatalogFileName = "catalog.json";

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public CatalogConfiguration LoadOrCreateSample(string baseDirectory)
    {
        var fullPath = Path.Combine(baseDirectory, CatalogFileName);
        if (!File.Exists(fullPath))
        {
            var sample = CreateDefaultConfiguration();
            var json = JsonSerializer.Serialize(sample, _serializerOptions);
            File.WriteAllText(fullPath, json);
            return sample;
        }

        var content = File.ReadAllText(fullPath);
        var config = JsonSerializer.Deserialize<CatalogConfiguration>(content, _serializerOptions);
        return config ?? CreateDefaultConfiguration();
    }

    private static CatalogConfiguration CreateDefaultConfiguration()
    {
        return new CatalogConfiguration
        {
            Games =
            {
                new CatalogSection
                {
                    Title = "Zenless Zone Zero",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "CBT 1",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "pCloud Mirror (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 2",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "ZZZ APK (Xeondev ±)",
                                    Url = string.Empty,
                                },
                            },
                        },
                    },
                },
                new CatalogSection
                {
                    Title = "Honkai: Star Rail",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "Pre-CBT v0.56",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Archive.org Mirror (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                    },
                },
                new CatalogSection
                {
                    Title = "Genshin Impact",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "CBT 1",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Archive.org Mirror (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 1.1",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "YuanShen_CB1.1_80d9 (+)",
                                    Url = string.Empty,
                                },
                                new LinkEntry
                                {
                                    Name = "YuanShen_CB1.1_4a10 (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 2",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "YuanShen_CB2.0.0 (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 0.7.0",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Genshin_0.7.0 (+)",
                                    Url = string.Empty,
                                },
                                new LinkEntry
                                {
                                    Name = "Genshin_0.7.1 (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 0.9.0",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "YuanShen_0.9.3 (+)",
                                    Url = string.Empty,
                                },
                                new LinkEntry
                                {
                                    Name = "YuanShen_0.9.9 (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                    },
                },
            },
            ServerRepo =
            {
                new CatalogSection
                {
                    Title = "Genshin Impact",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "CBT 1 (Story Mode)",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Telegram Mirror (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 1.3",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Google Drive (+)",
                                    Url = string.Empty,
                                },
                            },
                        },
                    },
                },
            },
            ExternalLinks = new ExternalLinks
            {
                ServerTools =
                {
                    new LinkEntry
                    {
                        Name = "Docker Desktop",
                        Url = "https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe",
                    },
                },
                CommunityUrl = string.Empty,
            },
        };
    }
}
