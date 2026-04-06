# VolkLoaderAvalonia

This project is a C# + Avalonia rewrite of the uploaded Python GUI's desktop shell: sidebar navigation, Home/Games/Server/Settings pages, EN/RU localization, theme switching, aria2c detection, browser launch, and the download-method overlay.

## What is included

- Avalonia desktop app targeting `.NET 8`
- Local file-backed `catalog.json`
- Saved user settings in `user-settings.json`
- Theme switching and language switching
- Browser opening through the OS shell
- Optional `aria2c` launching from PATH

## Catalog template status

The bundled `catalog.json` is now fully populated with the original safe structure from the uploaded Python script:
- real game names
- real version names
- real link labels
- server-repository section names
- external-link entry names

Every URL is set to `PLACEHOLDER_URL`.
The app treats `PLACEHOLDER_URL` and empty strings as **not configured**.

## How to fill the catalog manually

1. Close the app.
2. Open `catalog.json` in any text editor.
3. Replace each `PLACEHOLDER_URL` with your own authorized URL.
4. Save the file.
5. Run the app again.

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
