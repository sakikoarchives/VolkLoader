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
                            Links = { new LinkEntry { Name = "pCloud Mirror (+)", Url = "https://e.pcloud.link/publink/show?code=z0ootalK" } },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 2",
                            Links = { new LinkEntry { Name = "ZZZ APK (Xeondev ±)", Url = "https://git.xeondev.com/xeon/ZZZToolkit/src/branch/main/ZZZ.apk" } },
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
                            Links = { new LinkEntry { Name = "Archive.org Mirror (+)", Url = "https://archive.org/details/honkai-star-rail-pre-cbt-v0.56" } },
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
                            Links = { new LinkEntry { Name = "Archive.org Mirror (+)", Url = "https://archive.org/download/gs-cn-cb-1.0.0/GS_CN_CB1.0.0.exe" } },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 1.1",
                            Links =
                            {
                                new LinkEntry { Name = "YuanShen_CB1.1_80d9 (+)", Url = "https://hk4e-download.oss-cn-shanghai.aliyuncs.com/client_app/pc/YuanShen_CB1.1_80d9edf828.zip" },
                                new LinkEntry { Name = "YuanShen_CB1.1_4a10 (+)", Url = "https://hk4e-download.oss-cn-shanghai.aliyuncs.com/client_app/pc/YuanShen_CB1.1_4a1067e2bd.zip" },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 2",
                            Links = { new LinkEntry { Name = "YuanShen_CB2.0.0 (+)", Url = "https://hk4e-download.oss-cn-shanghai.aliyuncs.com/client_app/pc/YuanShen_CB2.0.0_e4392f9320.zip" } },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 0.7.0",
                            Links =
                            {
                                new LinkEntry { Name = "Genshin_0.7.0 (+)", Url = "https://autopatchhk.yuanshen.com/client_app/pc_plus19/Genshin_0.7.0.zip" },
                                new LinkEntry { Name = "Genshin_0.7.1 (+)", Url = "https://autopatchhk.yuanshen.com/client_app/pc_plus19/Genshin_0.7.1.zip" },
                            },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 0.9.0",
                            Links =
                            {
                                new LinkEntry { Name = "YuanShen_0.9.3 (+)", Url = "https://autopatchcn.yuanshen.com/client_app/pc_release/YuanShen_0.9.3.zip" },
                                new LinkEntry { Name = "YuanShen_0.9.9 (+)", Url = "https://autopatchcn.yuanshen.com/client_app/pc_cb3/YuanShen_0.9.9.zi" },
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
                            Links = { new LinkEntry { Name = "Telegram Mirror (+)", Url = "https://t.me/zbV0l/11756" } },
                        },
                        new VersionEntry
                        {
                            Name = "CBT 1.3",
                            Links = { new LinkEntry { Name = "Google Drive (+)", Url = "https://drive.google.com/file/d/113OoHv_uvn3B6pz8ZqumUeeDszGZ8FA1/view" } },
                        },
                    },
                },
            },
            ExternalLinks = new ExternalLinks
            {
                ServerTools =
                {
                    new LinkEntry { Name = "Docker Desktop", Url = "https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe" },
                    new LinkEntry { Name = "SucroseProxy", Url = "https://drive.google.com/uc?export=download&id=1Ys4c8w5i-Kc07msy-mVKiYSV4zpd_WB5" },
                },
                CommunityUrl = "https://t.me/v0lchokteam",
            },
        };
    }
}
