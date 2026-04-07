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
        en.Values["server_specs"] = "RAM: 16.0 GB+" +
                                    "DISK: 8.0 GB+\n" +
                                    "VIRTUALIZATION: ENABLED\n" +
                                    "OS: WINDOWS 64 - BIT";
        en.Values["server_tools"] = "DOWNLOAD TOOLS BUNDLE";
        en.Values["server_repo"] = "BROWSE SERVER REPO";
        en.Values["server_guide"] = "READ SETUP GUIDE";
        en.Values["community"] = "TELEGRAM CHANNEL";
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
        en.Values["guide_text"] = "1. Preparation: install Docker & SucroseProxy.\n" +
                                  "2. Download: extract the server (avoid problematic symbols in the path).\n" +
                                  "3. Install: run the required bootstrap script with administrator rights.\n" +
                                  "4. Start: launch the start script and verify that all background services are healthy.\n" +
                                  "5. Login: use your own lawful test environment and credentials.\n";
        en.Values["not_found"] = "[ NOT FOUND ]";
        en.Values["found"] = "[ FOUND ]";
        en.Values["aria_missing_message"] = "aria2c was not found in PATH.";
        en.Values["aria_started_message"] = "aria2c was started in the background.";
        en.Values["browser_opened_message"] = "The link was opened in the default browser.";
        en.Values["catalog_notice"] = "The bundled catalog.json is now fully populated with the original section, version, and label structure. Replace PLACEHOLDER_URL values with your own authorized links.";
        en.Values["settings_footer"] = "early-beta4 | build 1004";
        en.Values["hero_subtitle"] = "Specialized loader shell for private research, reverse engineering, and testing environments.";
        en.Values["link_not_configured"] = "This entry is present in the template, but its URL is not configured yet.";
        en.Values["server_tools_missing_message"] = "No tool URLs are configured yet. Edit catalog.json and replace PLACEHOLDER_URL.";
        en.Values["community_missing_message"] = "The community link is not configured yet. Edit catalog.json and replace PLACEHOLDER_URL.";
        en.Values["overlay_placeholder_hint"] = "Edit catalog.json and replace PLACEHOLDER_URL with your own URL.";

        var ru = new LocaleBundle();
        ru.Values["nav_home"] = "ГЛАВНАЯ";
        ru.Values["nav_games"] = "ИГРЫ";
        ru.Values["nav_server"] = "СЕРВЕР";
        ru.Values["nav_settings"] = "НАСТРОЙКИ";
        ru.Values["home_games"] = "СКАЧАТЬ ИГРЫ";
        ru.Values["home_servers"] = "СКАЧАТЬ СЕРВЕРЫ";
        ru.Values["server_specs"] = "ОЗУ: 16.0 ГБ+\n" +
                                    "ДИСК: 8.0 ГБ+\n" +
                                    "ВИРТУАЛИЗАЦИЯ: ВКЛЮЧЕНА\n" +
                                    "ОС: WINDOWS 64-BIT";
        ru.Values["server_tools"] = "СКАЧАТЬ НАБОР СОФТА";
        ru.Values["server_repo"] = "КАТАЛОГ СЕРВЕРОВ";
        ru.Values["server_guide"] = "ИНСТРУКЦИЯ ПО УСТАНОВКЕ";
        ru.Values["community"] = "TELEGRAM CHANNEL";
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
        ru.Values["guide_text"] = "1. Подготовка: установи Docker и SucroseProxy.\n" +
                                  "2. Загрузка: распакуй сервер в путь без проблемных символов.\n" +
                                  "3. Установка: запусти нужный bootstrap-скрипт с правами администратора.\n" +
                                  "4. Запуск: стартуй сервер и проверь, что фоновые сервисы работают штатно.\n" +
                                  "5. Вход: используй свою легальную тестовую среду и собственные учётные данные.";
        ru.Values["not_found"] = "[ НЕ НАЙДЕНО ]";
        ru.Values["found"] = "[ НАЙДЕНО ]";
        ru.Values["aria_missing_message"] = "aria2c не найден в PATH.";
        ru.Values["aria_started_message"] = "aria2c был запущен в фоне.";
        ru.Values["browser_opened_message"] = "Ссылка открыта в браузере по умолчанию.";
        ru.Values["catalog_notice"] = "Файл catalog.json теперь полностью заполнен исходной структурой разделов, версий и подписей. Замени значения PLACEHOLDER_URL на свои разрешённые ссылки.";
        ru.Values["settings_footer"] = "early-beta4 | build 1004";
        ru.Values["hero_subtitle"] = "Специализированная оболочка-загрузчик для приватных исследовательских и тестовых сред.";
        ru.Values["link_not_configured"] = "Эта запись уже есть в шаблоне, но URL для неё пока не задан.";
        ru.Values["server_tools_missing_message"] = "URL для набора инструментов пока не заданы. Открой catalog.json и замени PLACEHOLDER_URL.";
        ru.Values["community_missing_message"] = "Ссылка сообщества пока не задана. Открой catalog.json и замени PLACEHOLDER_URL.";
        ru.Values["overlay_placeholder_hint"] = "Открой catalog.json и замени PLACEHOLDER_URL на свой URL.";

        return new Dictionary<string, LocaleBundle>
        {
            ["EN"] = en,
            ["RU"] = ru,
        };
    }
}
