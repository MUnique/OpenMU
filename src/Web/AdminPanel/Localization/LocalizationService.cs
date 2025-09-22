namespace MUnique.OpenMU.Web.AdminPanel.Localization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Provides localized strings that are loaded from json files placed in the <c>Localization</c> folder of the project.
/// The json files follow the naming scheme <c>strings.&lt;language&gt;.json</c> where &lt;language&gt; is the language code, e.g. "en" or "es".
/// </summary>
public class LocalizationService
{
    private readonly IHostEnvironment _environment;
    private readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> _cache = new();
    private readonly object _languageLock = new();
    private string _currentLanguage = DefaultLanguage;
    private IReadOnlyDictionary<string, string> _currentDictionary = Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    /// <param name="environment">The host environment.</param>
    public LocalizationService(IHostEnvironment environment)
    {
        this._environment = environment;
        this._currentDictionary = this.LoadDictionary(DefaultLanguage);
        this.CurrentCulture = CreateCulture(DefaultLanguage);
        CultureInfo.CurrentCulture = this.CurrentCulture;
        CultureInfo.CurrentUICulture = this.CurrentCulture;
    }

    /// <summary>
    /// Gets the default language.
    /// </summary>
    public const string DefaultLanguage = "en";

    /// <summary>
    /// Gets the available languages.
    /// </summary>
    public IReadOnlyList<string> AvailableLanguages { get; } = new[] { "en", "es" };

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
    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("en-US");

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
            if (this._currentDictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            // fallback to default language dictionary
            var defaultDictionary = this.LoadDictionary(DefaultLanguage);
            return defaultDictionary.TryGetValue(key, out var defaultValue) ? defaultValue : key;
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

    private IReadOnlyDictionary<string, string> LoadDictionary(string language)
    {
        return this._cache.GetOrAdd(language, key =>
        {
            var fileName = $"strings.{key}.json";
            var filePath = Path.Combine(this._environment.ContentRootPath, "Localization", fileName);
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
            var cultureName = language switch
            {
                "en" => "en-US",
                "es" => "es-ES",
                _ => DefaultLanguage,
            };
            return CultureInfo.GetCultureInfo(cultureName);
        }
    }

    private static IReadOnlyDictionary<string, string> Empty { get; } = new Dictionary<string, string>();
}
