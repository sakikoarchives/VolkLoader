using Avalonia.Styling;

namespace VolkLoaderAvalonia.Models;

public sealed class AppTheme
{
    public AppTheme(
        string name,
        string background,
        string sidebar,
        string neon,
        string accent,
        string border,
        string text,
        ThemeVariant mode)
    {
        Name = name;
        Background = background;
        Sidebar = sidebar;
        Neon = neon;
        Accent = accent;
        Border = border;
        Text = text;
        Mode = mode;
    }

    public string Name { get; }

    public string Background { get; }

    public string Sidebar { get; }

    public string Neon { get; }

    public string Accent { get; }

    public string Border { get; }

    public string Text { get; }

    public ThemeVariant Mode { get; }
}
