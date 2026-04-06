# VolkLoaderAvalonia

This project is a C# + Avalonia rewrite of the uploaded Python GUI's core desktop shell: sidebar navigation, Home/Games/Server/Settings pages, EN/RU localization, theme switching, aria2c detection, browser launch, and the download-method overlay. The source app structure came from the uploaded `customtkinter` script. Replace the bundled placeholder catalog with lawful resources you are actually allowed to distribute.

## What is included

- Avalonia desktop app targeting `.NET 8`
- Local file-backed `catalog.json`
- Saved user settings in `user-settings.json`
- Theme switching and language switching
- Browser opening through the OS shell
- Optional `aria2c` launching from PATH

## What was intentionally not copied verbatim

The uploaded Python file contains a catalog of mirrors, archived builds, and server-resource links. This rewrite keeps the same *structure* but ships only placeholder entries in `catalog.json`. Fill that file with your own lawful URLs.

## Run

1. Install the .NET 8 SDK.
2. Restore packages:
   ```bash
   dotnet restore
   ```
3. Run the app:
   ```bash
   dotnet run
   ```

## Notes

- Avalonia's current docs say Avalonia is a cross-platform .NET UI framework and show `dotnet run` for a desktop app flow.
- The current stable NuGet line for `Avalonia` and `Avalonia.Desktop` is `11.3.13`, which is what this project references.
- On .NET, opening a URL with `Process.Start` requires shell execution when you want the OS to open the default associated app or browser.
