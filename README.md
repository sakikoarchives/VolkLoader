# VolkLoaderAvalonia: Technical Documentation

**VolkLoaderAvalonia** — кроссплатформенный десктоп-лоадер для загрузки игровых клиентов и серверных сборок. Переписан с **Python (customtkinter)** на **C#** с использованием **Avalonia UI**.

---

## 🇷🇺 Описание проекта (RU)

**VolkLoader** — это графическое приложение для удобного доступа к ссылкам на загрузку игровых клиентов (Genshin Impact, Honkai: Star Rail, Zenless Zone Zero) и серверных окружений. Поддерживает переключение тем оформления, локализацию и два метода загрузки.

### Основные возможности
* **Навигация:** Боковое меню (Главная, Игры, Сервер, Настройки).
* **Динамический каталог:** Загрузка данных из локального файла `catalog.json`.
* **Серверные сборки:** Отдельный раздел для серверных окружений.
* **Кастомизация:** Поддержка различных тем оформления и языков (EN/RU).
* **Методы загрузки:** Через системный браузер или многопоточный `aria2c`.

### Технические требования
| Компонент | Требование |
| :--- | :--- |
| **Среда выполнения** | .NET 8 SDK или выше |
| **Операционные системы** | Windows, Linux, macOS (Avalonia UI) |
| **Дополнительно** | aria2c в системном PATH (для быстрой загрузки) |

### Инструкция по запуску
Вы можете воспользоваться готовым релизом под Windows, Linux или macOS из вкладки Releases. После скачивания архива достаточно открыть VolkLoaderAvalonia внутри.

Для запуска из исходного кода используйте следующие команды:

```bash
dotnet restore
dotnet build
dotnet run
```

---

## 🇺🇸 Project Description (EN)

**VolkLoader** is a graphical application designed for convenient access to download links for game clients and server environments. Rewritten from Python to C# for improved performance and cross-platform compatibility.

### Key Features
* **Sidebar Navigation:** Home, Games, Server, and Settings pages.
* **Dynamic Catalog:** Content managed via `catalog.json`.
* **Theme & Localization:** Integrated support for multiple themes and EN/RU languages.
* **Download Management:** Choice between default browser and `aria2c` (16 parallel connections).

### System Requirements
* **.NET 8 SDK** or higher.
* **OS:** Windows / Linux / macOS.
* **Optional:** `aria2c` installed and added to PATH.

### Getting Started
You can use a compiled release from the "Releases" tab. After unzipping the archive, open the VolkLoaderAvalonia binary inside.

To compile from source, use:

```bash
dotnet restore
dotnet build
dotnet run
```

---

## Сравнение методов загрузки / Download Methods

| Метод / Method | Описание / Description |
| :--- | :--- |
| **Browser** | Открывает URL в браузере по умолчанию / Opens URL in the system default browser. |
| **aria2c** | 16 параллельных соединений через отдельный процесс / 16 parallel connections via a separate process. |
