# FileEncryptorApp - Приложение для Шифрования Файлов с DevSecOps

Данный проект представляет собой Windows Forms приложение на C# для шифрования и дешифрования файлов с использованием AES. Особенностью проекта является интеграция практик DevSecOps, включая автоматизированный анализ безопасности и сборку с помощью GitHub Actions.

## Статусы Сборки и Анализа

| Сервис                                   | Статус                                                                                                                                                                       |
| :--------------------------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **CI Сборка & Анализ (.NET)**            | [![.NET Security Analysis and Build](https://github.com/Charlottka314/vkr_devsecops/actions/workflows/security_analysis.yml/badge.svg)](https://github.com/Charlottka314/vkr_devsecops/actions/workflows/security_analysis.yml) |
| **Качество Кода (SonarCloud)**           | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Charlottka314_vkr_devsecops&metric=alert_status)](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) |
| **Уязвимости (SonarCloud)**            | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Charlottka314_vkr_devsecops&metric=vulnerabilities)](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) |
| **Покрытие Кода (SonarCloud)**         | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Charlottka314_vkr_devsecops&metric=coverage)](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) |
| **Технический Долг (SonarCloud)**      | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Charlottka314_vkr_devsecops&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) |
| **Дублирование Кода (SonarCloud)**     | [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Charlottka314_vkr_devsecops&metric=duplicated_lines_density)](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) |

## Функциональность Приложения

*   **Выбор файла:** Пользователь может выбрать любой файл для шифрования или дешифрования.
*   **Выбор папки для резервного копирования:** Перед шифрованием создается резервная копия исходного файла в указанную пользователем папку.
*   **Шифрование:**
    *   Файл шифруется с использованием алгоритма AES-256 (ключ 256 бит, вектор инициализации 128 бит).
    *   Ключ и вектор инициализации (IV) генерируются криптографически стойким генератором случайных чисел для каждой сессии шифрования. **Важно:** В текущей реализации ключ и IV хранятся только в памяти на время сессии работы приложения. Для реального использования их необходимо безопасно сохранять или генерировать на основе пароля.
    *   Вычисляется SHA512 хэш исходного файла и сохраняется в отдельный `.hash` файл рядом с зашифрованным.
    *   Зашифрованный файл получает расширение `.enc`.
*   **Дешифрование:**
    *   Пользователь выбирает ранее зашифрованный `.enc` файл.
    *   Используются ключ и IV, сгенерированные во время шифрования в текущей сессии.
    *   Дешифрованный файл получает расширение `.dec`.
    *   Проверяется целостность дешифрованного файла путем сравнения его SHA512 хэша с сохраненным в `.hash` файле.
    *   Отображается статус операции и результат проверки целостности.

## Настройка DevSecOps и CI/CD

Интеграция DevSecOps в данном проекте реализована с помощью GitHub Actions и SonarCloud.

### 1. GitHub Actions Workflow (`.github/workflows/security_analysis.yml`)

Рабочий процесс GitHub Actions (`security_analysis.yml`) автоматизирует следующие шаги при каждом `push` в ветку `main` или при создании/обновлении `pull request` на `main`:

*   **Checkout Code:** Загрузка исходного кода репозитория.
*   **Setup .NET SDK:** Установка .NET SDK версии 8.0.x.
*   **Setup JDK:** Установка Zulu OpenJDK 17, требуемого для SonarScanner.
*   **Caching SonarCloud Components:** Кэширование пакетов и сканера SonarCloud для ускорения последующих сборок.
*   **Install SonarCloud Scanner:** Установка `dotnet-sonarscanner` (локально в `.sonar/scanner`), если он отсутствует в кэше.
*   **Restore Dependencies:** Восстановление NuGet-зависимостей проекта.
*   **Scan for Vulnerable Dependencies:**
    *   Используется команда `dotnet list package --vulnerable` для проекта `FileEncryptorApp/FileEncryptorApp.sln`.
    *   Результаты сканирования сохраняются в артефакт `vulnerability-scan-report.txt`.
    *   Если найдены уязвимости, рабочий процесс **завершается с ошибкой** после выполнения всех шагов анализа, но отчет об уязвимостях все равно загружается.
*   **SonarCloud Analysis:**
    *   **Begin Analysis:** Инициализация анализа SonarCloud с использованием ключа проекта `Charlottka314_vkr_devsecops`, организации `charlottka314` и токена аутентификации из секретов. Передаются параметры для интеграции с Pull Request'ами.
    *   **Build Project:** Сборка проекта `FileEncryptorApp/FileEncryptorApp.sln` в конфигурации `Release`.
    *   **End Analysis:** Завершение анализа и отправка результатов на сервер SonarCloud.
*   **Upload Vulnerability Report:** Артефакт с отчетом об уязвимостях зависимостей загружается для последующего просмотра.
*   **Fail build on vulnerabilities:** Если шаг сканирования зависимостей обнаружил уязвимости, сборка помечается как неуспешная.

Полное содержимое файла `security_analysis.yml` можно найти в репозитории по пути `.github/workflows/security_analysis.yml`.

### 2. Настройка Секретов в GitHub

Для взаимодействия с SonarCloud рабочий процесс использует следующие секреты, которые настроены в данном репозитории GitHub (`Settings > Secrets and variables > Actions > Repository secrets`):

*   `SONAR_TOKEN`: Персональный токен доступа к SonarCloud. В данном проекте используется токен, сгенерированный пользователем `Charlottka314` для этого репозитория. Пример значения (не актуальный): `aac1e35dd6bf12635e1e2dbaa7bd502acbb232ad`. Актуальный токен хранится в секретах.
*   `SONAR_PROJECT_KEY`: Уникальный ключ проекта на SonarCloud, для данного проекта это `Charlottka314_vkr_devsecops`.
*   `SONAR_ORGANIZATION`: Имя организации на SonarCloud, для данного проекта это `charlottka314`.

`GITHUB_TOKEN` предоставляется GitHub Actions автоматически и используется для интеграции с Pull Request'ами.

### 3. SonarCloud

SonarCloud используется для статического анализа кода (SAST) на предмет:

*   **Уязвимостей в коде:** Поиск потенциальных проблем безопасности.
*   **Code Smells ("Запахи кода"):** Проблемы в коде, указывающие на сложности в поддержке.
*   **Bugs:** Потенциальные ошибки в логике программы.
*   **Технический долг:** Оценка усилий для исправления проблем.
*   **Покрытие кода тестами:** (Текущая конфигурация не включает выполнение тестов).
*   **Дублирование кода.**

Результаты анализа доступны на дашборде проекта в SonarCloud: [https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops](https://sonarcloud.io/summary/overall?id=Charlottka314_vkr_devsecops) и частично интегрируются в Pull Request'ы на GitHub.

### 4. Устраненные уязвимости и улучшения в коде

В процессе работы над проектом и на основе рекомендаций SonarCloud были внесены следующие улучшения:

*   **Безопасная генерация ключей и IV:** Вместо `System.Random` для генерации ключей шифрования и векторов инициализации (IV) теперь используется `System.Security.Cryptography.RandomNumberGenerator`.
*   **Обработка Nullable Reference Types:** Код адаптирован для корректной работы с nullable reference types. Поля, которые могут быть `null`, помечены как `nullable` (`?`), и добавлены проверки перед их использованием.
*   **Удаление закомментированного кода:** Весь неиспользуемый закомментированный код удален.
*   **Явное указание режимов AES:** В классе `AesEncryption` теперь явно указываются `CipherMode.CBC` и `PaddingMode.PKCS7`.
*   **Улучшения в `HashUtility`:** Возвращаемый тип изменен на `string?`. Добавлены более точные проверки существования файла и обработки состояний, когда хэш не может быть вычислен.

## Дальнейшие шаги и возможные улучшения

*   **Управление ключами:** Реализовать безопасное хранение или генерацию ключей шифрования (например, на основе пароля пользователя с использованием PBKDF2).
*   **Юнит-тесты:** Написать юнит-тесты и интегрировать их выполнение в CI/CD пайплайн.
*   **Мониторинг зависимостей:** Активировать Dependabot для автоматического отслеживания уязвимостей в NuGet-пакетах.
*   **Логирование:** Добавить более подробное логирование.
*   **Улучшение UI/UX:** Дальнейшие улучшения пользовательского интерфейса.