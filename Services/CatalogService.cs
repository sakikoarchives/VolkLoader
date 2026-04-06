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
            var sample = CreateSampleConfiguration();
            var json = JsonSerializer.Serialize(sample, _serializerOptions);
            File.WriteAllText(fullPath, json);
            return sample;
        }

        var content = File.ReadAllText(fullPath);
        var config = JsonSerializer.Deserialize<CatalogConfiguration>(content, _serializerOptions);
        return config ?? CreateSampleConfiguration();
    }

    private static CatalogConfiguration CreateSampleConfiguration()
    {
        return new CatalogConfiguration
        {
            Games =
            {
                new CatalogSection
                {
                    Title = "Sample Game",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "Sample Version 1",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Official page",
                                    Url = "https://example.com/game",
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
                    Title = "Sample Server Resources",
                    Versions =
                    {
                        new VersionEntry
                        {
                            Name = "Setup archive",
                            Links =
                            {
                                new LinkEntry
                                {
                                    Name = "Documentation",
                                    Url = "https://example.com/server-docs",
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
                        Url = "https://www.docker.com/products/docker-desktop/",
                    },
                    new LinkEntry
                    {
                        Name = "Resource placeholder",
                        Url = "https://example.com/tools-bundle",
                    },
                },
                CommunityUrl = "https://example.com/community",
            },
        };
    }
}
