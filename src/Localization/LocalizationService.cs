namespace MUnique.OpenMU.Localization;

using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

/// <summary>
/// Provides localized strings that are loaded from json files placed in a configurable directory.
/// The json files follow the naming scheme <c>{FilePrefix}.&lt;language&gt;.json</c> where &lt;language&gt; is the language code, e.g. "en" or "es".
/// </summary>
public class LocalizationService
{
    private readonly LocalizationOptions _options;
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _cache = new();
    private readonly object _languageLock = new();
    private string _currentLanguage;
    private IReadOnlyDictionary<string, string> _currentDictionary = Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    /// <param name="options">The localization options.</param>
    public LocalizationService(LocalizationOptions options)
    {
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(this._options.DefaultLanguage))
        {
            throw new ArgumentException("Default language must not be null or whitespace", nameof(options));
        }

        if (!Directory.Exists(this._options.ResourceDirectory))
        {
            Directory.CreateDirectory(this._options.ResourceDirectory);
        }

        this._currentLanguage = this._options.DefaultLanguage;
        this._currentDictionary = this.LoadDictionary(this._options.DefaultLanguage);
        this.CurrentCulture = CreateCulture(this._options.DefaultLanguage);
        CultureInfo.CurrentCulture = this.CurrentCulture;
        CultureInfo.CurrentUICulture = this.CurrentCulture;
    }

    /// <summary>
    /// Gets the default language.
    /// </summary>
    public string DefaultLanguage => this._options.DefaultLanguage;

    /// <summary>
    /// Gets the available languages.
    /// </summary>
    public IReadOnlyList<string> AvailableLanguages => this._options.AvailableLanguages;

    /// <summary>
    /// Gets the current language code.
    /// </summary>
    public string CurrentLanguage
    {
        get => this._currentLanguage;
        private set => this._currentLanguage = value;
    }

    /// <summary>
    /// Gets the current <see cref="CultureInfo"/>.
    /// </summary>
    public CultureInfo CurrentCulture { get; private set; }

    /// <summary>
    /// Occurs when <see cref="CurrentLanguage"/> has been changed.
    /// </summary>
    public event Action? LanguageChanged;

    /// <summary>
    /// Gets a localized string for the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the string.</param>
    /// <returns>The localized string if the key is found; otherwise the key itself.</returns>
    public string this[string key]
    {
        get
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (this._currentDictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            var fallbackDictionary = this.LoadDictionary(this._options.DefaultLanguage);
            return fallbackDictionary.TryGetValue(key, out var fallbackValue) ? fallbackValue : key;
        }
    }

    /// <summary>
    /// Gets a formatted localized string for the specified <paramref name="key"/> using the provided <paramref name="arguments"/>.
    /// </summary>
    /// <param name="key">The key of the string.</param>
    /// <param name="arguments">The format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    public string GetString(string key, params object?[] arguments)
    {
        var format = this[key];
        return arguments.Length > 0 ? string.Format(this.CurrentCulture, format, arguments) : format;
    }

    /// <summary>
    /// Changes the current language.
    /// </summary>
    /// <param name="language">The language code.</param>
    public void SetLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Language must not be null or whitespace", nameof(language));
        }

        language = language.ToLowerInvariant();

        lock (this._languageLock)
        {
            if (string.Equals(this.CurrentLanguage, language, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var dictionary = this.LoadDictionary(language);
            this._currentDictionary = dictionary;
            this.CurrentLanguage = language;
            this.CurrentCulture = CreateCulture(language);
            CultureInfo.CurrentCulture = this.CurrentCulture;
            CultureInfo.CurrentUICulture = this.CurrentCulture;
        }

        this.LanguageChanged?.Invoke();
    }

    /// <summary>
    /// Clears the cached dictionaries forcing them to be reloaded on next use.
    /// </summary>
    public void ClearCache() => this._cache.Clear();

    private IReadOnlyDictionary<string, string> LoadDictionary(string language)
    {
        return this._cache.GetOrAdd(language, key =>
        {
            var fileName = $"{this._options.FilePrefix}.{key}.json";
            var filePath = Path.Combine(this._options.ResourceDirectory, fileName);
            if (!File.Exists(filePath))
            {
                return Empty;
            }

            using var stream = File.OpenRead(filePath);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(stream, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            });

            return dictionary ?? Empty;
        });
    }

    private static CultureInfo CreateCulture(string language)
    {
        try
        {
            return CultureInfo.GetCultureInfo(language);
        }
        catch (CultureNotFoundException)
        {
            return language switch
            {
                "en" => CultureInfo.GetCultureInfo("en-US"),
                "es" => CultureInfo.GetCultureInfo("es-ES"),
                _ => CultureInfo.InvariantCulture,
            };
        }
    }

    private static IReadOnlyDictionary<string, string> Empty { get; } = new Dictionary<string, string>();
}

/// <summary>
/// Defines options for <see cref="LocalizationService"/>.
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// Gets or sets the directory where the localization json files are stored.
    /// </summary>
    public string ResourceDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "Localization");

    /// <summary>
    /// Gets or sets the prefix of the json files. Defaults to <c>strings</c>.
    /// </summary>
    public string FilePrefix { get; set; } = "strings";

    /// <summary>
    /// Gets or sets the default language. Defaults to <c>en</c>.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets the list of available languages. Defaults to English and Spanish.
    /// </summary>
    public IReadOnlyList<string> AvailableLanguages { get; set; } = new[] { "en", "es" };
}
