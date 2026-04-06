using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VolkLoaderAvalonia.Data;
using VolkLoaderAvalonia.Models;
using VolkLoaderAvalonia.Services;

namespace VolkLoaderAvalonia;

public partial class MainWindow : Window
{
    private const string BannerText =
        "┈┈┏╮┏╮┈┈┈┈┈┈┈┈╭╮   VOLKLOADER [AVALONIA-PORT]\n" +
        "┈╭┛┗┛┗┳━━━━━━╮┃┃   safe desktop shell\n" +
        "┈┃▅┃▅┈┃╰╰╰╰╰╰┣╯┃   C# + Avalonia\n" +
        "▇┻━╯┈┈┃╰╰╰╰╰╰┣━╯   --------------------------------------\n" +
        "┣━━━╯┈╰╰╰╰╰╰╰┃┈┈   File-backed catalog, browser launch,\n" +
        "╰━━┳┳━┓┏━┳┳┓┏╯┈┈   aria2c integration, themes and i18n.";

    private readonly string _baseDirectory = AppContext.BaseDirectory;

    private readonly CatalogService _catalogService = new();

    private readonly SettingsService _settingsService = new();

    private readonly IReadOnlyDictionary<string, LocaleBundle> _locales = LocalizedResources.Build();

    private readonly IReadOnlyDictionary<string, AppTheme> _themes;

    private CatalogConfiguration _catalog = new();

    private UserSettings _settings = new();

    private string _keyBuffer = string.Empty;

    private string _selectedActionUrl = string.Empty;

    public MainWindow()
    {
        InitializeComponent();

        _themes = BuildThemes();
        _settings = _settingsService.Load(_baseDirectory);
        _catalog = _catalogService.LoadOrCreateSample(_baseDirectory);

        if (!_themes.ContainsKey(_settings.ThemeName))
        {
            _settings.ThemeName = "VOLK_PURPLE";
        }

        if (!_locales.ContainsKey(_settings.Language))
        {
            _settings.Language = "EN";
        }

        KeyDown += OnWindowKeyDown;

        ApplyTheme();
        UpdateSidebarText();
        ShowHome();
    }

    private AppTheme CurrentTheme => _themes[_settings.ThemeName];

    private LocaleBundle CurrentLocale => _locales[_settings.Language];

    private void OnHomeClicked(object? sender, RoutedEventArgs e)
    {
        ShowHome();
    }

    private void OnGamesClicked(object? sender, RoutedEventArgs e)
    {
        ShowGames();
    }

    private void OnServerClicked(object? sender, RoutedEventArgs e)
    {
        ShowServer();
    }

    private void OnSettingsClicked(object? sender, RoutedEventArgs e)
    {
        ShowSettings();
    }

    private void ShowHome()
    {
        SetActiveSection(HomeButton);
        ClearContent();
        AddLogo();
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["home_games"], CurrentTheme.Sidebar, CurrentTheme.Accent, ShowGames));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["home_servers"], CurrentTheme.Sidebar, CurrentTheme.Neon, ShowServer));
        ContentPanel.Children.Add(CreateNoticeBlock(CurrentLocale["catalog_notice"]));
    }

    private void ShowGames()
    {
        SetActiveSection(GamesButton);
        ClearContent();
        AddLogo();

        foreach (var section in _catalog.Games)
        {
            ContentPanel.Children.Add(CreateWideButton(
                $"▓▒░ {section.Title} ░▒▓",
                CurrentTheme.Sidebar,
                CurrentTheme.Accent,
                () => ShowVersions(section.Title, section.Versions, ShowGames)));
        }
    }

    private void ShowServer()
    {
        SetActiveSection(ServerButton);
        ClearContent();
        AddLogo();
        ContentPanel.Children.Add(CreateInfoCard(CurrentLocale["server_specs"]));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_tools"], CurrentTheme.Accent, "#FFFFFF", OpenServerTools));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_repo"], CurrentTheme.Neon, "#FFFFFF", ShowServerRepo));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_guide"], CurrentTheme.Background, "#808080", ShowGuide));
    }

    private void ShowServerRepo()
    {
        SetActiveSection(ServerButton);
        ClearContent();
        ContentPanel.Children.Add(CreateHeader(CurrentLocale["server_repo_title"], CurrentTheme.Neon));

        foreach (var section in _catalog.ServerRepo)
        {
            ContentPanel.Children.Add(CreateWideButton(
                $"📂 {section.Title}",
                CurrentTheme.Sidebar,
                CurrentTheme.Text,
                () => ShowVersions(section.Title, section.Versions, ShowServerRepo)));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], ShowServer, "#808080"));
    }

    private void ShowVersions(string title, IReadOnlyList<VersionEntry> versions, Action backAction)
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeader(title.ToUpperInvariant(), CurrentTheme.Neon));

        foreach (var version in versions)
        {
            ContentPanel.Children.Add(CreateVersionButton(version.Name, () => ShowLinks(version.Name, version.Links, () => ShowVersions(title, versions, backAction))));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["return"], backAction, "#808080"));
    }

    private void ShowLinks(string versionName, IReadOnlyList<LinkEntry> links, Action backAction)
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeader(versionName.ToUpperInvariant(), CurrentTheme.Accent));

        foreach (var link in links)
        {
            ContentPanel.Children.Add(CreateLinkResourceButton(link.Name, () => ShowActionOverlay(link.Url)));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], backAction, "#808080"));
    }

    private void ShowGuide()
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeader(CurrentLocale["setup_guide_title"], CurrentTheme.Neon));
        ContentPanel.Children.Add(CreateInfoCard(CurrentLocale["guide_text"]));
        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], ShowServer, "#808080"));
    }

    private void ShowSettings()
    {
        SetActiveSection(SettingsButton);
        ClearContent();
        AddLogo();

        var ariaInstalled = IsAriaInstalled();
        var statusText = ariaInstalled ? CurrentLocale["found"] : CurrentLocale["not_found"];
        var statusColor = ariaInstalled ? "#00FF00" : "#FF0000";

        var statusRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 8,
            Margin = new Thickness(0, 6, 0, 6),
        };
        statusRow.Children.Add(new TextBlock
        {
            Text = CurrentLocale["aria_status"],
            Foreground = Brush(CurrentTheme.Text),
            FontFamily = MonoFont(),
            FontSize = 16,
        });
        statusRow.Children.Add(new TextBlock
        {
            Text = statusText,
            Foreground = Brush(statusColor),
            FontFamily = MonoFont(),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
        });
        ContentPanel.Children.Add(statusRow);

        ContentPanel.Children.Add(CreateHeader(CurrentLocale["theme_selector"], CurrentTheme.Neon, 20));
        ContentPanel.Children.Add(CreateThemeSelector());

        ContentPanel.Children.Add(CreateHeader(CurrentLocale["lang_selector"], CurrentTheme.Neon, 20));
        ContentPanel.Children.Add(CreateLanguageSelector());

        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["community"], CurrentTheme.Background, CurrentTheme.Neon, OpenCommunityLink, 300, 50));
        ContentPanel.Children.Add(new TextBlock
        {
            Text = CurrentLocale["settings_footer"],
            Foreground = Brush(CurrentTheme.Accent),
            HorizontalAlignment = HorizontalAlignment.Center,
            FontFamily = MonoFont(),
            FontSize = 12,
            Margin = new Thickness(0, 8, 0, 0),
        });
    }

    private ComboBox CreateThemeSelector()
    {
        var comboBox = new ComboBox
        {
            Width = 260,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemsSource = _themes.Keys.ToList(),
            SelectedItem = _settings.ThemeName,
        };
        comboBox.SelectionChanged += (_, _) =>
        {
            if (comboBox.SelectedItem is string selectedTheme && _themes.ContainsKey(selectedTheme))
            {
                _settings.ThemeName = selectedTheme;
                _settingsService.Save(_baseDirectory, _settings);
                ApplyTheme();
                ShowSettings();
            }
        };

        return comboBox;
    }

    private ComboBox CreateLanguageSelector()
    {
        var comboBox = new ComboBox
        {
            Width = 180,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemsSource = _locales.Keys.ToList(),
            SelectedItem = _settings.Language,
        };
        comboBox.SelectionChanged += (_, _) =>
        {
            if (comboBox.SelectedItem is string selectedLanguage && _locales.ContainsKey(selectedLanguage))
            {
                _settings.Language = selectedLanguage;
                _settingsService.Save(_baseDirectory, _settings);
                UpdateSidebarText();
                ShowSettings();
            }
        };

        return comboBox;
    }

    private void SetActiveSection(Button activeButton)
    {
        ApplySidebarButtonStyle(HomeButton);
        ApplySidebarButtonStyle(GamesButton);
        ApplySidebarButtonStyle(ServerButton);
        ApplySidebarButtonStyle(SettingsButton);

        activeButton.Background = Brush(CurrentTheme.Neon);
        activeButton.Foreground = Brush(CurrentTheme.Background);
        activeButton.BorderBrush = Brush(CurrentTheme.Neon);
    }

    private void ClearContent()
    {
        ContentPanel.Children.Clear();
        HideOverlay();
    }

    private void AddLogo()
    {
        var wrapper = new Border
        {
            Background = Brush(CurrentTheme.Sidebar),
            BorderBrush = Brush(CurrentTheme.Border),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(24),
            Margin = new Thickness(40, 30, 40, 18),
        };

        var panel = new StackPanel
        {
            Spacing = 10,
        };

        panel.Children.Add(new TextBlock
        {
            Text = BannerText,
            Foreground = Brush(CurrentTheme.Neon),
            FontFamily = MonoFont(),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        });

        panel.Children.Add(new Border
        {
            Height = 1,
            Background = Brush(CurrentTheme.Border),
            Margin = new Thickness(30, 2, 30, 2),
        });

        panel.Children.Add(new TextBlock
        {
            Text = CurrentLocale["hero_subtitle"],
            Foreground = Brush(CurrentTheme.Text),
            FontFamily = MonoFont(),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        });

        wrapper.Child = panel;
        ContentPanel.Children.Add(wrapper);
    }

    private Border CreateNoticeBlock(string text)
    {
        return new Border
        {
            Background = Brush(CurrentTheme.Sidebar),
            BorderBrush = Brush(CurrentTheme.Border),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(18),
            Margin = new Thickness(60, 12, 60, 12),
            Child = new TextBlock
            {
                Text = text,
                Foreground = Brush(CurrentTheme.Text),
                FontFamily = MonoFont(),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
            },
        };
    }

    private Border CreateInfoCard(string text)
    {
        return new Border
        {
            Background = Brush(CurrentTheme.Sidebar),
            BorderBrush = Brush(CurrentTheme.Border),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(24),
            Margin = new Thickness(60, 10, 60, 10),
            Child = new TextBlock
            {
                Text = text,
                Foreground = Brush(CurrentTheme.Text),
                FontFamily = MonoFont(),
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
            },
        };
    }

    private TextBlock CreateHeader(string text, string colorHex, double fontSize = 28)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = Brush(colorHex),
            FontFamily = MonoFont(),
            FontSize = fontSize,
            FontWeight = FontWeight.Bold,
            LetterSpacing = 0.8,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 30, 0, 20),
        };
    }

    private Button CreateWideButton(string text, string backgroundHex, string textOrAccentHex, Action onClick, double width = 580, double height = 75)
    {
        var button = CreateBaseButton(text, backgroundHex, textOrAccentHex, width, height);
        button.Click += (_, _) => onClick();
        return button;
    }

    private Button CreateVersionButton(string text, Action onClick)
    {
        var button = CreateBaseButton(text, CurrentTheme.Sidebar, CurrentTheme.Text, 580, 60);
        button.BorderBrush = Brush(CurrentTheme.Neon);
        button.Click += (_, _) => onClick();
        return button;
    }

    private Button CreateLinkResourceButton(string text, Action onClick)
    {
        var button = CreateBaseButton($"★ {text}", CurrentTheme.Sidebar, CurrentTheme.Text, 620, 55);
        button.BorderBrush = Brush(CurrentTheme.Border);
        button.Click += (_, _) => onClick();
        return button;
    }

    private Button CreateLinkButton(string text, Action onClick, string colorHex)
    {
        var button = new Button
        {
            Content = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = Brushes.Transparent,
            Foreground = Brush(colorHex),
            BorderBrush = Brushes.Transparent,
            FontFamily = MonoFont(),
            FontSize = 14,
            Margin = new Thickness(0, 18, 0, 0),
        };
        button.Click += (_, _) => onClick();
        return button;
    }

    private Button CreateBaseButton(string text, string backgroundHex, string textHex, double width, double height)
    {
        return new Button
        {
            Content = text,
            Width = width,
            Height = height,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = Brush(backgroundHex),
            Foreground = Brush(textHex),
            BorderBrush = Brush(CurrentTheme.Border),
            BorderThickness = new Thickness(1),
            FontFamily = MonoFont(),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(18, 8),
            Margin = new Thickness(0, 4, 0, 4),
        };
    }

    private void ShowActionOverlay(string url)
    {
        _selectedActionUrl = url;
        OverlayPanel.Children.Clear();
        OverlayPanel.Children.Add(CreateHeader(CurrentLocale["select_method"], CurrentTheme.Neon, 20));

        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16,
            Margin = new Thickness(0, 8, 0, 8),
        };
        row.Children.Add(CreateOverlayActionButton(CurrentLocale["method_browser"], CurrentTheme.Neon, OpenInBrowser));
        row.Children.Add(CreateOverlayActionButton(CurrentLocale["method_aria"], CurrentTheme.Accent, StartAriaDownload));
        OverlayPanel.Children.Add(row);
        OverlayPanel.Children.Add(CreateLinkButton(CurrentLocale["cancel"], HideOverlay, "#FF4444"));
        OverlayBackdrop.IsVisible = true;
        OverlayBorder.IsVisible = true;
    }

    private Button CreateOverlayActionButton(string text, string backgroundHex, Action onClick)
    {
        var button = new Button
        {
            Content = text,
            Width = 180,
            Height = 50,
            Background = Brush(backgroundHex),
            Foreground = Brushes.White,
            BorderBrush = Brush(backgroundHex),
            BorderThickness = new Thickness(1),
            FontFamily = MonoFont(),
            FontSize = 14,
            FontWeight = FontWeight.Bold,
        };
        button.Click += (_, _) => onClick();
        return button;
    }

    private void ShowInfoOverlay(string message)
    {
        OverlayPanel.Children.Clear();
        OverlayPanel.Children.Add(CreateHeader(message, CurrentTheme.Neon, 18));
        OverlayPanel.Children.Add(CreateLinkButton(CurrentLocale["close"], HideOverlay, CurrentTheme.Text));
        OverlayBackdrop.IsVisible = true;
        OverlayBorder.IsVisible = true;
    }

    private void HideOverlay()
    {
        OverlayBackdrop.IsVisible = false;
        OverlayBorder.IsVisible = false;
        OverlayPanel.Children.Clear();
    }

    private void OpenServerTools()
    {
        foreach (var link in _catalog.ExternalLinks.ServerTools)
        {
            TryOpenTarget(link.Url);
        }

        ShowInfoOverlay(CurrentLocale["browser_opened_message"]);
    }

    private void OpenCommunityLink()
    {
        if (!string.IsNullOrWhiteSpace(_catalog.ExternalLinks.CommunityUrl))
        {
            TryOpenTarget(_catalog.ExternalLinks.CommunityUrl);
            ShowInfoOverlay(CurrentLocale["browser_opened_message"]);
        }
    }

    private void OpenInBrowser()
    {
        if (!string.IsNullOrWhiteSpace(_selectedActionUrl))
        {
            TryOpenTarget(_selectedActionUrl);
            ShowInfoOverlay(CurrentLocale["browser_opened_message"]);
        }
    }

    private void StartAriaDownload()
    {
        if (!IsAriaInstalled())
        {
            ShowInfoOverlay(CurrentLocale["aria_missing_message"]);
            return;
        }

        try
        {
            var uri = new Uri(_selectedActionUrl);
            var fileName = Path.GetFileName(uri.AbsolutePath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "download.bin";
            }

            var downloadsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(downloadsDirectory);

            var startInfo = new ProcessStartInfo
            {
                FileName = "aria2c",
                Arguments = $"-x 16 -s 16 -d {Quote(downloadsDirectory)} -o {Quote(fileName)} {Quote(_selectedActionUrl)}",
                UseShellExecute = false,
                WorkingDirectory = downloadsDirectory,
            };

            Process.Start(startInfo);
            ShowInfoOverlay(CurrentLocale["aria_started_message"]);
        }
        catch (Exception ex)
        {
            ShowInfoOverlay(ex.Message);
        }
    }

    private void TryOpenTarget(string target)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = target,
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            ShowInfoOverlay(ex.Message);
        }
    }

    private void UpdateSidebarText()
    {
        HomeButton.Content = $"❯ {CurrentLocale["nav_home"]}";
        GamesButton.Content = $"❯ {CurrentLocale["nav_games"]}";
        ServerButton.Content = $"❯ {CurrentLocale["nav_server"]}";
        SettingsButton.Content = $"❯ {CurrentLocale["nav_settings"]}";
    }

    private void ApplyTheme()
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = CurrentTheme.Mode;
        }

        Background = Brush(CurrentTheme.Background);
        Sidebar.Background = Brush(CurrentTheme.Sidebar);
        Sidebar.BorderBrush = Brush(CurrentTheme.Border);
        SidebarTitle.Foreground = Brush(CurrentTheme.Neon);
        SidebarSubtitle.Foreground = Brush(CurrentTheme.Text);
        SidebarDivider.Background = Brush(CurrentTheme.Border);
        SidebarFooterDivider.Background = Brush(CurrentTheme.Border);
        SidebarFooterLine1.Foreground = Brush(CurrentTheme.Accent);
        SidebarFooterLine2.Foreground = Brush(CurrentTheme.Text);
        ContainerBorder.Background = Brush(CurrentTheme.Background);
        ContainerBorder.BorderBrush = Brush(CurrentTheme.Border);
        OverlayBackdrop.Background = new SolidColorBrush(Color.FromArgb(0x88, 0x00, 0x00, 0x00));
        OverlayBorder.Background = Brush(CurrentTheme.Sidebar);
        OverlayBorder.BorderBrush = Brush(CurrentTheme.Neon);

        ApplySidebarButtonStyle(HomeButton);
        ApplySidebarButtonStyle(GamesButton);
        ApplySidebarButtonStyle(ServerButton);
        ApplySidebarButtonStyle(SettingsButton);
    }

    private void ApplySidebarButtonStyle(Button button)
    {
        button.Background = Brushes.Transparent;
        button.Foreground = Brush(CurrentTheme.Neon);
        button.BorderBrush = Brush(CurrentTheme.Neon);
        button.BorderThickness = new Thickness(1);
        button.FontFamily = MonoFont();
        button.FontSize = 14;
        button.FontWeight = FontWeight.Bold;
        button.HorizontalContentAlignment = HorizontalAlignment.Center;
        button.Padding = new Thickness(12, 6);
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        var keyText = e.Key.ToString().ToLowerInvariant();
        if (keyText.Length == 1)
        {
            _keyBuffer += keyText;
            if (_keyBuffer.Length > 10)
            {
                _keyBuffer = _keyBuffer.Substring(_keyBuffer.Length - 10, 10);
            }
        }

        if (_keyBuffer.Contains("volk", StringComparison.Ordinal))
        {
            TryOpenTarget("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            _keyBuffer = string.Empty;
        }
    }

    private static FontFamily MonoFont()
    {
        return new FontFamily("Consolas, Menlo, Monaco, Courier New");
    }

    private static IReadOnlyDictionary<string, AppTheme> BuildThemes()
    {
        return new Dictionary<string, AppTheme>
        {
            ["VOLK_PURPLE"] = new AppTheme("VOLK_PURPLE", "#0A0A0A", "#121212", "#D000FF", "#FF00FF", "#1F1F1F", "#EEEEEE", ThemeVariant.Dark),
            ["PINK_WHITE"] = new AppTheme("PINK_WHITE", "#FFFFFF", "#FFF0F5", "#FF69B4", "#FF1493", "#FFC0CB", "#333333", ThemeVariant.Light),
            ["CLASSIC_LIGHT"] = new AppTheme("CLASSIC_LIGHT", "#F0F0F0", "#E0E0E0", "#333333", "#555555", "#CCCCCC", "#222222", ThemeVariant.Light),
            ["CYBER_YELLOW"] = new AppTheme("CYBER_YELLOW", "#1A1A1A", "#111111", "#FFD700", "#FFA500", "#333333", "#FFD700", ThemeVariant.Dark),
        };
    }

    private static bool IsAriaInstalled()
    {
        var executableNames = OperatingSystem.IsWindows()
            ? new[] { "aria2c.exe", "aria2c.cmd", "aria2c.bat" }
            : new[] { "aria2c" };

        var pathVariable = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathVariable))
        {
            return false;
        }

        foreach (var directory in pathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var executableName in executableNames)
            {
                var fullPath = Path.Combine(directory, executableName);
                if (File.Exists(fullPath))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string Quote(string value)
    {
        return $"\"{value.Replace("\"", "\\\"")}\"";
    }

    private static SolidColorBrush Brush(string hex)
    {
        return new SolidColorBrush(Color.Parse(hex));
    }
}
