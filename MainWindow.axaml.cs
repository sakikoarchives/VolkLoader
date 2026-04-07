using Avalonia;
using Avalonia.Controls;
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
        "┈┈┏╮┏╮┈┈┈┈┈┈┈┈╭╮   VOLKLOADER [STABLE-BUILD]\n" +
        "┈╭┛┗┛┗┳━━━━━━╮┃┃   early-beta4 | build 1004\n" +
        "┈┃▅┃▅┈┃╰╰╰╰╰╰┣╯┃   MADE BY VOLCHOKTEAM\n" +
        "▇┻━╯┈┈┃╰╰╰╰╰╰┣━╯   --------------------------------------\n" +
        "┣━━━╯┈╰╰╰╰╰╰╰┃┈┈   Specialized loader shell for private\n" +
        "╰━━┳┳━┓┏━┳┳┓┏╯┈┈   research, reverse engineering, and testing.";

    private readonly string _baseDirectory = AppContext.BaseDirectory;

    private readonly CatalogService _catalogService = new();

    private readonly SettingsService _settingsService = new();

    private readonly IReadOnlyDictionary<string, LocaleBundle> _locales = LocalizedResources.Build();

    private readonly IReadOnlyDictionary<string, AppTheme> _themes;

    private CatalogConfiguration _catalog = new();

    private UserSettings _settings = new();

    private string _activeSection = "HOME";

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

        AttachSidebarEffects(HomeButton, "HOME");
        AttachSidebarEffects(GamesButton, "GAMES");
        AttachSidebarEffects(ServerButton, "SERVER");
        AttachSidebarEffects(SettingsButton, "SETTINGS");

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

    private void OnOverlayBackdropPressed(object? sender, PointerPressedEventArgs e)
    {
        HideOverlay();
    }

    private void ShowHome()
    {
        SetActiveSection("HOME");
        ClearContent();
        AddLogo();
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["home_games"], CurrentTheme.Sidebar, CurrentTheme.Accent, ShowGames));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["home_servers"], CurrentTheme.Sidebar, CurrentTheme.Neon, ShowServer));
        ContentPanel.Children.Add(CreateNoticeBlock(CurrentLocale["catalog_notice"]));
    }

    private void ShowGames()
    {
        SetActiveSection("GAMES");
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
        SetActiveSection("SERVER");
        ClearContent();
        AddLogo();
        ContentPanel.Children.Add(CreateInfoCard(CurrentLocale["server_specs"], CurrentTheme.Neon));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_tools"], CurrentTheme.Accent, "#FFFFFF", OpenServerTools, isEnabled: HasConfiguredToolUrls()));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_repo"], CurrentTheme.Neon, "#FFFFFF", ShowServerRepo));
        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["server_guide"], CurrentTheme.Background, "#909090", ShowGuide));
    }

    private void ShowServerRepo()
    {
        SetActiveSection("SERVER");
        ClearContent();
        ContentPanel.Children.Add(CreateHeaderBlock(CurrentLocale["server_repo_title"], CurrentTheme.Neon));

        foreach (var section in _catalog.ServerRepo)
        {
            ContentPanel.Children.Add(CreateWideButton(
                $"📂 {section.Title}",
                CurrentTheme.Sidebar,
                CurrentTheme.Text,
                () => ShowVersions(section.Title, section.Versions, ShowServerRepo),
                580,
                65));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], ShowServer, "#909090"));
    }

    private void ShowVersions(string title, IReadOnlyList<VersionEntry> versions, Action backAction)
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeaderBlock(title.ToUpperInvariant(), CurrentTheme.Neon));

        foreach (var version in versions)
        {
            ContentPanel.Children.Add(CreateVersionButton(
                version.Name,
                () => ShowLinks(version.Name, version.Links, () => ShowVersions(title, versions, backAction))));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["return"], backAction, "#909090"));
    }

    private void ShowLinks(string versionName, IReadOnlyList<LinkEntry> links, Action backAction)
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeaderBlock(versionName.ToUpperInvariant(), CurrentTheme.Accent));

        foreach (var link in links)
        {
            ContentPanel.Children.Add(CreateLinkResourceButton(link.Name, () => ShowActionOverlay(link.Url), IsConfiguredUrl(link.Url)));
        }

        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], backAction, "#909090"));
    }

    private void ShowGuide()
    {
        ClearContent();
        ContentPanel.Children.Add(CreateHeaderBlock(CurrentLocale["setup_guide_title"], CurrentTheme.Neon));
        ContentPanel.Children.Add(CreateInfoCard(CurrentLocale["guide_text"], CurrentTheme.Accent));
        ContentPanel.Children.Add(CreateLinkButton(CurrentLocale["back"], ShowServer, "#909090"));
    }

    private void ShowSettings()
    {
        SetActiveSection("SETTINGS");
        ClearContent();
        AddLogo();

        var ariaInstalled = IsAriaInstalled();
        var statusText = ariaInstalled ? CurrentLocale["found"] : CurrentLocale["not_found"];
        var statusColor = ariaInstalled ? "#00FF66" : "#FF5555";

        var statusRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 8,
            Margin = new Thickness(0, 8, 0, 4),
        };
        statusRow.Children.Add(new TextBlock
        {
            Text = CurrentLocale["aria_status"],
            Foreground = Brush(CurrentTheme.Text),
            FontFamily = MonoFont(),
            FontSize = 16,
            VerticalAlignment = VerticalAlignment.Center,
        });
        statusRow.Children.Add(new TextBlock
        {
            Text = statusText,
            Foreground = Brush(statusColor),
            FontFamily = MonoFont(),
            FontSize = 16,
            FontWeight = FontWeight.Bold,
            VerticalAlignment = VerticalAlignment.Center,
        });
        ContentPanel.Children.Add(WrapCard(statusRow, CurrentTheme.Neon, new Thickness(60, 8, 60, 8), new Thickness(20, 16)));

        ContentPanel.Children.Add(CreateHeaderBlock(CurrentLocale["theme_selector"], CurrentTheme.Neon, 20));
        ContentPanel.Children.Add(WrapCard(CreateThemeSelector(), CurrentTheme.Accent, new Thickness(120, 2, 120, 8), new Thickness(16, 14)));

        ContentPanel.Children.Add(CreateHeaderBlock(CurrentLocale["lang_selector"], CurrentTheme.Neon, 20));
        ContentPanel.Children.Add(WrapCard(CreateLanguageSelector(), CurrentTheme.Accent, new Thickness(160, 2, 160, 8), new Thickness(16, 14)));

        ContentPanel.Children.Add(CreateWideButton(CurrentLocale["community"], CurrentTheme.Background, CurrentTheme.Neon, OpenCommunityLink, 320, 52, IsConfiguredUrl(_catalog.ExternalLinks.CommunityUrl)));
        ContentPanel.Children.Add(new TextBlock
        {
            Text = CurrentLocale["settings_footer"],
            Foreground = Brush(CurrentTheme.Accent),
            HorizontalAlignment = HorizontalAlignment.Center,
            FontFamily = MonoFont(),
            FontSize = 12,
            Margin = new Thickness(0, 10, 0, 0),
        });
    }

    private ComboBox CreateThemeSelector()
    {
        var comboBox = new ComboBox
        {
            Width = 280,
            HorizontalAlignment = HorizontalAlignment.Center,
            ItemsSource = _themes.Keys.ToList(),
            SelectedItem = _settings.ThemeName,
            Background = Brush(CurrentTheme.Sidebar),
            Foreground = Brush(CurrentTheme.Text),
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
            Background = Brush(CurrentTheme.Sidebar),
            Foreground = Brush(CurrentTheme.Text),
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

    private void SetActiveSection(string sectionName)
    {
        _activeSection = sectionName;
        RefreshSidebarButtons();
    }

    private void RefreshSidebarButtons()
    {
        ApplySidebarButtonVisual(HomeButton, "HOME", false, false);
        ApplySidebarButtonVisual(GamesButton, "GAMES", false, false);
        ApplySidebarButtonVisual(ServerButton, "SERVER", false, false);
        ApplySidebarButtonVisual(SettingsButton, "SETTINGS", false, false);
    }

    private void ClearContent()
    {
        ContentPanel.Children.Clear();
        HideOverlay();
    }

    private void AddLogo()
    {
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
            TextAlignment = TextAlignment.Left,
        });

        panel.Children.Add(new Border
        {
            Height = 1,
            Background = Brush(CurrentTheme.Border),
            Margin = new Thickness(26, 2, 26, 2),
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

        ContentPanel.Children.Add(WrapCard(panel, CurrentTheme.Neon, new Thickness(40, 30, 40, 18), new Thickness(24)));
    }

    private Control CreateNoticeBlock(string text)
    {
        return WrapCard(
            new TextBlock
            {
                Text = text,
                Foreground = Brush(CurrentTheme.Text),
                FontFamily = MonoFont(),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
            },
            CurrentTheme.Accent,
            new Thickness(60, 12, 60, 12),
            new Thickness(18));
    }

    private Control CreateInfoCard(string text, string accentHex)
    {
        return WrapCard(
            new TextBlock
            {
                Text = text,
                Foreground = Brush(CurrentTheme.Text),
                FontFamily = MonoFont(),
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
            },
            accentHex,
            new Thickness(60, 10, 60, 10),
            new Thickness(24));
    }

    private Control CreateHeaderBlock(string text, string colorHex, double fontSize = 28)
    {
        var panel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 4,
            Margin = new Thickness(0, 28, 0, 16),
        };

        panel.Children.Add(new TextBlock
        {
            Text = text,
            Foreground = Brush(colorHex),
            FontFamily = MonoFont(),
            FontSize = fontSize,
            FontWeight = FontWeight.Bold,
            LetterSpacing = 0.8,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
        });

        panel.Children.Add(new Border
        {
            Width = 180,
            Height = 1,
            Background = Brush(CurrentTheme.Border),
            HorizontalAlignment = HorizontalAlignment.Center,
        });

        return panel;
    }

    private Button CreateWideButton(string text, string backgroundHex, string textOrAccentHex, Action onClick, double width = 580, double height = 75, bool isEnabled = true)
    {
        var button = CreateBaseButton(text, backgroundHex, textOrAccentHex, width, height);
        if (isEnabled)
        {
            AttachInteractiveEffects(button, backgroundHex, textOrAccentHex, CurrentTheme.Border, pressedUsesForeground: false);
            button.Click += (_, _) => onClick();
        }
        else
        {
            button.IsEnabled = false;
            button.Opacity = 0.60;
            SetButtonVisual(button, backgroundHex, "#888888", CurrentTheme.Border);
        }

        return button;
    }

    private Button CreateVersionButton(string text, Action onClick)
    {
        var button = CreateBaseButton(text, CurrentTheme.Sidebar, CurrentTheme.Text, 580, 60);
        SetButtonVisual(button, CurrentTheme.Sidebar, CurrentTheme.Text, CurrentTheme.Neon);
        AttachInteractiveEffects(button, CurrentTheme.Sidebar, CurrentTheme.Text, CurrentTheme.Neon, pressedUsesForeground: true);
        button.Click += (_, _) => onClick();
        return button;
    }

    private Button CreateLinkResourceButton(string text, Action onClick, bool isAvailable)
    {
        var button = CreateBaseButton($"★ {text}", CurrentTheme.Sidebar, CurrentTheme.Text, 620, 55);
        if (isAvailable)
        {
            SetButtonVisual(button, CurrentTheme.Sidebar, CurrentTheme.Text, CurrentTheme.Border);
            AttachInteractiveEffects(button, CurrentTheme.Sidebar, CurrentTheme.Text, CurrentTheme.Border, pressedUsesForeground: true);
            button.Click += (_, _) => onClick();
        }
        else
        {
            button.IsEnabled = false;
            button.Opacity = 0.60;
            SetButtonVisual(button, CurrentTheme.Sidebar, "#888888", CurrentTheme.Border);
        }
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
        OverlayPanel.Children.Add(CreateHeaderBlock(CurrentLocale["select_method"], CurrentTheme.Neon, 20));

        var displayText = IsConfiguredUrl(url)
            ? url
            : CurrentLocale["link_not_configured"] + "\n\n" + CurrentLocale["overlay_placeholder_hint"];

        OverlayPanel.Children.Add(WrapCard(
            new TextBlock
            {
                Text = displayText,
                Foreground = Brush(CurrentTheme.Text),
                FontFamily = MonoFont(),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
            },
            CurrentTheme.Border,
            new Thickness(4, 0, 4, 4),
            new Thickness(14, 12)));

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
        OverlayPanel.Children.Add(CreateLinkButton(CurrentLocale["cancel"], HideOverlay, "#FF5555"));
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
        AttachInteractiveEffects(button, backgroundHex, "#FFFFFF", backgroundHex, pressedUsesForeground: false);
        button.Click += (_, _) => onClick();
        return button;
    }

    private void ShowInfoOverlay(string message)
    {
        OverlayPanel.Children.Clear();
        OverlayPanel.Children.Add(CreateHeaderBlock(message, CurrentTheme.Neon, 18));
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
        var openedAny = false;
        foreach (var link in _catalog.ExternalLinks.ServerTools)
        {
            if (IsConfiguredUrl(link.Url))
            {
                TryOpenTarget(link.Url);
                openedAny = true;
            }
        }

        ShowInfoOverlay(openedAny ? CurrentLocale["browser_opened_message"] : CurrentLocale["server_tools_missing_message"]);
    }

    private void OpenCommunityLink()
    {
        if (IsConfiguredUrl(_catalog.ExternalLinks.CommunityUrl))
        {
            TryOpenTarget(_catalog.ExternalLinks.CommunityUrl);
            ShowInfoOverlay(CurrentLocale["browser_opened_message"]);
            return;
        }

        ShowInfoOverlay(CurrentLocale["community_missing_message"]);
    }

    private void OpenInBrowser()
    {
        if (IsConfiguredUrl(_selectedActionUrl))
        {
            TryOpenTarget(_selectedActionUrl);
            ShowInfoOverlay(CurrentLocale["browser_opened_message"]);
            return;
        }

        ShowInfoOverlay(CurrentLocale["link_not_configured"]);
    }

    private void StartAriaDownload()
    {
        if (!IsAriaInstalled())
        {
            ShowInfoOverlay(CurrentLocale["aria_missing_message"]);
            return;
        }

        if (!IsConfiguredUrl(_selectedActionUrl))
        {
            ShowInfoOverlay(CurrentLocale["link_not_configured"]);
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

        RefreshSidebarButtons();
    }

    private void AttachSidebarEffects(Button button, string sectionName)
    {
        button.PointerEntered += (_, _) => ApplySidebarButtonVisual(button, sectionName, hover: true, pressed: false);
        button.PointerExited += (_, _) => ApplySidebarButtonVisual(button, sectionName, hover: false, pressed: false);
        button.PointerPressed += (_, _) => ApplySidebarButtonVisual(button, sectionName, hover: false, pressed: true);
        button.PointerReleased += (_, _) => ApplySidebarButtonVisual(button, sectionName, button.IsPointerOver, false);
    }

    private void ApplySidebarButtonVisual(Button button, string sectionName, bool hover, bool pressed)
    {
        var isActive = string.Equals(_activeSection, sectionName, StringComparison.Ordinal);

        string background;
        string foreground;
        string border;

        if (isActive)
        {
            background = CurrentTheme.Neon;
            foreground = CurrentTheme.Background;
            border = CurrentTheme.Neon;
        }
        else if (pressed)
        {
            background = AdjustColor(CurrentTheme.Neon, -0.08);
            foreground = CurrentTheme.Background;
            border = CurrentTheme.Neon;
        }
        else if (hover)
        {
            background = AdjustColor(CurrentTheme.Sidebar, 0.08);
            foreground = CurrentTheme.Neon;
            border = CurrentTheme.Neon;
        }
        else
        {
            background = "#00000000";
            foreground = CurrentTheme.Neon;
            border = CurrentTheme.Neon;
        }

        SetButtonVisual(button, background, foreground, border);
        button.BorderThickness = new Thickness(1);
        button.FontFamily = MonoFont();
        button.FontSize = 14;
        button.FontWeight = FontWeight.Bold;
        button.HorizontalContentAlignment = HorizontalAlignment.Center;
        button.Padding = new Thickness(12, 6);
    }

    private void AttachInteractiveEffects(Button button, string normalBackground, string normalForeground, string normalBorder, bool pressedUsesForeground)
    {
        var hoverBackground = CurrentTheme.Mode == ThemeVariant.Dark
            ? AdjustColor(normalBackground, 0.08)
            : AdjustColor(normalBackground, -0.06);

        var hoverBorder = normalBorder == "#00000000"
            ? CurrentTheme.Neon
            : AdjustColor(normalBorder, 0.10);

        var pressedBackground = pressedUsesForeground ? AdjustColor(CurrentTheme.Neon, -0.08) : AdjustColor(normalBackground, -0.10);
        var pressedForeground = pressedUsesForeground ? CurrentTheme.Background : normalForeground;
        var pressedBorder = pressedUsesForeground ? CurrentTheme.Neon : hoverBorder;

        button.PointerEntered += (_, _) => SetButtonVisual(button, hoverBackground, normalForeground, hoverBorder);
        button.PointerExited += (_, _) => SetButtonVisual(button, normalBackground, normalForeground, normalBorder);
        button.PointerPressed += (_, _) => SetButtonVisual(button, pressedBackground, pressedForeground, pressedBorder);
        button.PointerReleased += (_, _) =>
        {
            if (button.IsPointerOver)
            {
                SetButtonVisual(button, hoverBackground, normalForeground, hoverBorder);
            }
            else
            {
                SetButtonVisual(button, normalBackground, normalForeground, normalBorder);
            }
        };
    }

    private Control WrapCard(Control child, string accentHex, Thickness margin, Thickness padding)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("6,*"),
        };

        grid.Children.Add(new Border
        {
            Background = Brush(accentHex),
            CornerRadius = new CornerRadius(3),
        });

        var host = new Border
        {
            Background = Brush(CurrentTheme.Sidebar),
            BorderBrush = Brush(CurrentTheme.Border),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = padding,
            Child = child,
        };
        Grid.SetColumn(host, 1);
        grid.Children.Add(host);

        return new Border
        {
            Background = Brushes.Transparent,
            Margin = margin,
            Child = grid,
        };
    }

    private static void SetButtonVisual(Button button, string backgroundHex, string foregroundHex, string borderHex)
    {
        button.Background = backgroundHex == "#00000000" ? Brushes.Transparent : Brush(backgroundHex);
        button.Foreground = Brush(foregroundHex);
        button.BorderBrush = borderHex == "#00000000" ? Brushes.Transparent : Brush(borderHex);
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

    private bool HasConfiguredToolUrls()
    {
        return _catalog.ExternalLinks.ServerTools.Any(static link => IsConfiguredUrl(link.Url));
    }

    private static bool IsConfiguredUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (string.Equals(value, "PLACEHOLDER_URL", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
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

    private static string AdjustColor(string hex, double delta)
    {
        var color = Color.Parse(hex);
        return $"#{Adjust(color.R, delta):X2}{Adjust(color.G, delta):X2}{Adjust(color.B, delta):X2}";
    }

    private static byte Adjust(byte component, double delta)
    {
        var adjusted = delta >= 0
            ? component + ((255 - component) * delta)
            : component * (1.0 + delta);

        adjusted = Math.Clamp(adjusted, 0, 255);
        return (byte)Math.Round(adjusted);
    }
}
