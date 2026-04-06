# VolkLoaderAvalonia

This project is a C# + Avalonia rewrite of the uploaded Python GUI's desktop shell: sidebar navigation, Home/Games/Server/Settings pages, EN/RU localization, theme switching, aria2c detection, browser launch, and the download-method overlay.

## What is included

- Avalonia desktop app targeting `.NET 8`
- Local file-backed `catalog.json`
- Saved user settings in `user-settings.json`
- Theme switching and language switching
- Browser opening through the OS shell
- Optional `aria2c` launching from PATH

## Catalog status

The bundled `catalog.json` now reflects the real game names, version names, and link labels from the uploaded Python script. Direct mirror URLs and server-binary download links were intentionally left blank, so you can fill them locally only with resources you are authorized to use.

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
