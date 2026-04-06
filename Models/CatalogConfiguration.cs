using System.Collections.Generic;

namespace VolkLoaderAvalonia.Models;

public sealed class CatalogConfiguration
{
    public List<CatalogSection> Games { get; set; } = new();

    public List<CatalogSection> ServerRepo { get; set; } = new();

    public ExternalLinks ExternalLinks { get; set; } = new();
}

public sealed class CatalogSection
{
    public string Title { get; set; } = string.Empty;

    public List<VersionEntry> Versions { get; set; } = new();
}

public sealed class VersionEntry
{
    public string Name { get; set; } = string.Empty;

    public List<LinkEntry> Links { get; set; } = new();
}

public sealed class LinkEntry
{
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}

public sealed class ExternalLinks
{
    public List<LinkEntry> ServerTools { get; set; } = new();

    public string CommunityUrl { get; set; } = string.Empty;
}
