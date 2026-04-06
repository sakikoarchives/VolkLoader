using System.Collections.Generic;
using VolkLoaderAvalonia.Models;

namespace VolkLoaderAvalonia.Data;

public static class LocalizedResources
{
    public static IReadOnlyDictionary<string, LocaleBundle> Build()
    {
        var en = new LocaleBundle();
        en.Values["nav_home"] = "HOME";
        en.Values["nav_games"] = "GAMES";
        en.Values["nav_server"] = "SERVER";
        en.Values["nav_settings"] = "SETTINGS";
        en.Values["home_games"] = "DOWNLOAD GAMES";
        en.Values["home_servers"] = "DOWNLOAD SERVERS";
        en.Values["server_specs"] = "RAM: 16.0 GB+\nDISK: 8.0 GB+\nVIRTUALIZATION: ENABLED\nOS: WINDOWS 64-BIT";
        en.Values["server_tools"] = "OPEN TOOL LINKS";
        en.Values["server_repo"] = "BROWSE RESOURCE REPO";
        en.Values["server_guide"] = "READ SETUP NOTES";
        en.Values["community"] = "COMMUNITY LINK";
        en.Values["back"] = "[ BACK ]";
        en.Values["return"] = "[ RETURN ]";
        en.Values["select_method"] = "--- SELECT METHOD ---";
        en.Values["method_browser"] = "BROWSER";
        en.Values["method_aria"] = "ARIA2C";
        en.Values["cancel"] = "CANCEL";
        en.Values["close"] = "CLOSE";
        en.Values["theme_selector"] = "THEME SELECTOR";
        en.Values["lang_selector"] = "LANGUAGE";
        en.Values["aria_status"] = "ARIA2C STATUS:";
        en.Values["setup_guide_title"] = "SETUP GUIDE";
        en.Values["server_repo_title"] = "SERVER REPOSITORY";
        en.Values["guide_text"] = "1. Preparation: install the required runtime tools.\n2. Download: keep your archives in a path without exotic symbols.\n3. Install: follow the README for your lawful resources.\n4. Start: make sure all required background services are healthy.\n5. Verify: test links and local files before distributing the build.";
        en.Values["not_found"] = "[ NOT FOUND ]";
        en.Values["found"] = "[ FOUND ]";
        en.Values["aria_missing_message"] = "aria2c was not found in PATH.";
        en.Values["aria_started_message"] = "aria2c was started in the background.";
        en.Values["browser_opened_message"] = "The link was opened in the default browser.";
        en.Values["catalog_notice"] = "This Avalonia port loads data from catalog.json. The bundled file contains only placeholders; replace them with lawful resources you are allowed to distribute.";
        en.Values["settings_footer"] = "Avalonia port generated from the uploaded Python app";
        en.Values["hero_subtitle"] = "Desktop launcher shell with themes, locales, file-backed catalog and aria2c integration.";

        var ru = new LocaleBundle();
        ru.Values["nav_home"] = "ГЛАВНАЯ";
        ru.Values["nav_games"] = "ИГРЫ";
        ru.Values["nav_server"] = "СЕРВЕР";
        ru.Values["nav_settings"] = "НАСТРОЙКИ";
        ru.Values["home_games"] = "СКАЧАТЬ ИГРЫ";
        ru.Values["home_servers"] = "СКАЧАТЬ СЕРВЕРЫ";
        ru.Values["server_specs"] = "ОЗУ: 16.0 ГБ+\nДИСК: 8.0 ГБ+\nВИРТУАЛИЗАЦИЯ: ВКЛЮЧЕНА\nОС: WINDOWS 64-BIT";
        ru.Values["server_tools"] = "ОТКРЫТЬ ИНСТРУМЕНТЫ";
        ru.Values["server_repo"] = "КАТАЛОГ РЕСУРСОВ";
        ru.Values["server_guide"] = "ИНСТРУКЦИЯ";
        ru.Values["community"] = "ССЫЛКА НА СООБЩЕСТВО";
        ru.Values["back"] = "[ НАЗАД ]";
        ru.Values["return"] = "[ В МЕНЮ ]";
        ru.Values["select_method"] = "--- ВЫБЕРИТЕ МЕТОД ---";
        ru.Values["method_browser"] = "БРАУЗЕР";
        ru.Values["method_aria"] = "ARIA2C";
        ru.Values["cancel"] = "ОТМЕНА";
        ru.Values["close"] = "ЗАКРЫТЬ";
        ru.Values["theme_selector"] = "ВЫБОР ТЕМЫ";
        ru.Values["lang_selector"] = "ЯЗЫК";
        ru.Values["aria_status"] = "СТАТУС ARIA2C:";
        ru.Values["setup_guide_title"] = "ИНСТРУКЦИЯ";
        ru.Values["server_repo_title"] = "РЕПОЗИТОРИЙ СЕРВЕРОВ";
        ru.Values["guide_text"] = "1. Подготовка: установи необходимые инструменты среды.\n2. Загрузка: держи архивы в пути без экзотических символов.\n3. Установка: следуй README для своих легальных ресурсов.\n4. Запуск: убедись, что все фоновые сервисы запущены.\n5. Проверка: проверь ссылки и локальные файлы перед распространением сборки.";
        ru.Values["not_found"] = "[ НЕ НАЙДЕНО ]";
        ru.Values["found"] = "[ НАЙДЕНО ]";
        ru.Values["aria_missing_message"] = "aria2c не найден в PATH.";
        ru.Values["aria_started_message"] = "aria2c был запущен в фоне.";
        ru.Values["browser_opened_message"] = "Ссылка открыта в браузере по умолчанию.";
        ru.Values["catalog_notice"] = "Этот порт на Avalonia загружает данные из catalog.json. В комплекте лежат только заглушки; замени их на легальные ресурсы, которые ты имеешь право распространять.";
        ru.Values["settings_footer"] = "Порт на Avalonia, собранный по загруженному Python-приложению";
        ru.Values["hero_subtitle"] = "Desktop-оболочка с темами, локалями, каталогом из файла и интеграцией aria2c.";

        return new Dictionary<string, LocaleBundle>
        {
            ["EN"] = en,
            ["RU"] = ru,
        };
    }
}
