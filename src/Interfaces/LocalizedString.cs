// <copyright file="LocalizedString.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Represents a string which can be translated to different languages and serializes into a single string.
/// It's meant for simple usage in database fields and not for complex localization scenarios.
/// To keep compatibility with normal strings, we simply assume the first string to be in neutral (usually english) language.
/// Example: "Some text||de=Etwas Text||fr=Un peu de texte" where the first part is english, second german and third french.
/// Translations can also be stored for specific cultures, e.g. "Some text||zh-CN=简体文本||zh-TW=繁體文本".
/// When looking up a translation, the exact culture name is tried first, then its parent cultures
/// (e.g. zh-CN, zh-Hans, zh) and finally any other culture of the same language.
/// Entries starting with "@" are metadata and never a translation, e.g. "Lorencia||zh-CN=勇者大陆||@src=MapNames/Lorencia||@stamp=1a2b3c4d".
/// They identify the source of the texts (<see cref="SourceKey"/>) and allow to detect later changes (<see cref="SourceStamp"/>).
/// </summary>
public readonly struct LocalizedString : IEquatable<LocalizedString>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedString"/> struct.
    /// </summary>
    /// <param name="value">The value.</param>
    public LocalizedString(string? value)
    {
        this.Value = value;
    }

    /// <summary>
    /// Gets the separator string which separates the different languages.
    /// </summary>
    public static string Separator => "||";

    /// <summary>
    /// Gets the neutral language code.
    /// </summary>
    public static string NeutralLanguageCode => "en";

    /// <summary>
    /// Gets the prefix of metadata entries. Culture names never start with it, so metadata entries
    /// can be stored like translations without being mistaken for one.
    /// </summary>
    public static string MetadataPrefix => "@";

    /// <summary>
    /// Gets the code of the metadata entry which holds the <see cref="SourceKey"/>.
    /// </summary>
    public static string SourceKeyCode => "@src";

    /// <summary>
    /// Gets the code of the metadata entry which holds the <see cref="SourceStamp"/>.
    /// </summary>
    public static string SourceStampCode => "@stamp";

    /// <summary>
    /// Gets the underlying serialized value of this localized string.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Gets the key of the source (e.g. a resource entry like "MapNames/Lorencia") from which the texts were taken.
    /// It allows to update the texts from their source later, without relying on the neutral text.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string? SourceKey => this.GetMetadata(SourceKeyCode);

    /// <summary>
    /// Gets the hash of the content (see <see cref="ComputeContentHash"/>) at the time the texts were taken from their source.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string? SourceStamp => this.GetMetadata(SourceStampCode);

    /// <summary>
    /// Gets a value indicating whether the texts are unchanged since they were taken from their source,
    /// i.e. the <see cref="SourceStamp"/> matches the current content.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsUnchangedSinceSourceStamp => this.SourceStamp is { } stamp && stamp == this.ComputeContentHash();

    /// <summary>
    /// Gets the value in the neutral language as a <see cref="string"/>.
    /// </summary>
    /// <value>
    ///   A <see cref="string"/> that contains the neutral language text, or an empty string
    ///   if the underlying value is <see langword="null"/>.
    /// </value>
    public string ValueInNeutralLanguage
    {
        get
        {
            if (this.Value?.IndexOf(Separator, StringComparison.OrdinalIgnoreCase) is not >= 0)
            {
                return this.Value ?? string.Empty;
            }

            var span = this.ValueInNeutralLanguageAsSpan;
            return new(span);
        }
    }

    /// <summary>
    /// Gets the value in the neutral language as a <see cref="ReadOnlySpan{T}"/> of characters.
    /// </summary>
    /// <value>
    ///   A <see cref="ReadOnlySpan{T}"/> that contains the neutral language text, or an empty span
    ///   if the underlying value is <see langword="null"/>.
    /// </value>
    [System.Text.Json.Serialization.JsonIgnore]
    public ReadOnlySpan<char> ValueInNeutralLanguageAsSpan
    {
        get
        {
            if (this.Value is null)
            {
                return [];
            }

            var separatorIndex = this.Value.IndexOf(Separator, StringComparison.OrdinalIgnoreCase);
            if (separatorIndex == -1)
            {
                return this.Value.AsSpan();
            }

            return this.Value.AsSpan(0, separatorIndex);
        }
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="LocalizedString"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="localizedString">The localized string.</param>
    /// <returns>The underlying serialized value of the localized string, or <see langword="null"/> if the instance is <see langword="null"/>.</returns>
    public static implicit operator string?(LocalizedString? localizedString)
    {
        return localizedString?.Value;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="LocalizedString"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="localizedString">The localized string.</param>
    /// <returns>The underlying serialized value of the localized string, or an empty string if the underlying value is <see langword="null"/>.</returns>
    public static implicit operator string(LocalizedString localizedString)
    {
        return localizedString.Value ?? string.Empty;
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string"/> to a nullable <see cref="LocalizedString"/>.
    /// </summary>
    /// <param name="localizedString">The serialized localized string value.</param>
    /// <returns>A new <see cref="LocalizedString"/> instance, or <see langword="null"/> if <paramref name="localizedString"/> is <see langword="null"/>.</returns>
    public static implicit operator LocalizedString?(string? localizedString)
    {
        if (localizedString is null)
        {
            return null;
        }

        return new LocalizedString(localizedString);
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="string"/> to <see cref="LocalizedString"/>.
    /// </summary>
    /// <param name="localizedString">The serialized localized string value.</param>
    /// <returns>A new <see cref="LocalizedString"/> instance.</returns>
    public static implicit operator LocalizedString(string localizedString)
    {
        return new LocalizedString(localizedString);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance for the current UI culture.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> containing the translation for the current UI culture,
    /// or the neutral language translation if none is available for the current culture.
    /// </returns>
    public override string? ToString()
    {
        return this.Value is null ? null : this.GetTranslation(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the translation for the specified culture.
    /// </summary>
    /// <param name="cultureInfo">The culture for which the translation is requested.</param>
    /// <param name="fallbackToNeutral">
    /// If set to <see langword="true"/>, falls back to the neutral language if no translation for
    /// <paramref name="cultureInfo"/> is available; otherwise returns an empty string.
    /// </param>
    /// <returns>
    /// The translation for the specified culture, the neutral language translation if
    /// <paramref name="fallbackToNeutral"/> is <see langword="true"/> and no specific translation exists,
    /// or an empty string if neither is available.
    /// </returns>
    public string? GetTranslation(CultureInfo cultureInfo, bool fallbackToNeutral = true)
    {
        var span = this.GetTranslationAsSpan(cultureInfo, fallbackToNeutral);
        return span.IsEmpty
            ? null
            : new(span);
    }

    /// <summary>
    /// Gets the translation for the specified culture as a <see cref="ReadOnlySpan{T}"/> of characters.
    /// </summary>
    /// <param name="cultureInfo">The culture for which the translation is requested.</param>
    /// <param name="fallbackToNeutral">
    /// If set to <see langword="true"/>, falls back to the neutral language if no translation for
    /// <paramref name="cultureInfo"/> is available; otherwise returns an empty span.
    /// </param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}"/> containing the translation for the specified culture,
    /// the neutral language translation if <paramref name="fallbackToNeutral"/> is <see langword="true"/>
    /// and no specific translation exists, or an empty span if neither is available.
    /// </returns>
    public ReadOnlySpan<char> GetTranslationAsSpan(CultureInfo cultureInfo, bool fallbackToNeutral = true)
    {
        if (this.Value is null)
        {
            return [];
        }

        if (IsNeutralLanguage(cultureInfo))
        {
            return this.ValueInNeutralLanguageAsSpan;
        }

        // Exact culture first, then the parent cultures, e.g. zh-CN -> zh-Hans -> zh.
        for (var culture = cultureInfo; !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
        {
            if (this.TryFindTranslation(culture.Name, out var translation))
            {
                return translation;
            }
        }

        // The parent chain may differ between platforms (ICU/NLS), so try the plain language code explicitly.
        if (this.TryFindTranslation(cultureInfo.TwoLetterISOLanguageName, out var languageTranslation))
        {
            return languageTranslation;
        }

        // Last resort: any other culture of the same language, e.g. "zh-CN" when "zh" was requested.
        if (this.TryFindTranslationOfLanguage(cultureInfo.TwoLetterISOLanguageName, out var siblingTranslation))
        {
            return siblingTranslation;
        }

        return fallbackToNeutral ? this.ValueInNeutralLanguageAsSpan : [];
    }

    /// <summary>
    /// Returns a new <see cref="LocalizedString"/> with the specified translation added, updated, or removed.
    /// </summary>
    /// <param name="cultureInfo">The culture for which the translation should be added or updated.</param>
    /// <param name="text">
    /// The translation text. If <see langword="null"/> or empty, an existing translation for the specified culture
    /// is removed.
    /// </param>
    /// <returns>
    /// A new <see cref="LocalizedString"/> instance with the modified translation for the specified culture.
    /// </returns>
    public LocalizedString WithTranslation(CultureInfo cultureInfo, string? text)
    {
        // Plan (pseudocode):
        // 1. Determine language code from culture (full culture name, e.g. "de" or "zh-CN").
        // 2. If language is neutral ("en"):
        //    a. If Value is null or empty:
        //       - If text is null or empty: return this (no change).
        //       - Else: create new base + keep all existing non-neutral parts (if any) and return new instance.
        //    b. If Value has separator:
        //       - Replace the part before first separator with the new text (may be null/empty).
        //    c. If Value has no separator:
        //       - Replace whole value with new text.
        // 3. If language is non-neutral:
        //    a. If Value is null or empty:
        //       - If text is null or empty: return this.
        //       - Else: initialize base as empty string and append "||xx=text".
        //    b. Search for existing "||xx=" section.
        //       - If found:
        //         i. If text is null or empty: remove that section (and possible trailing/leading separators).
        //         ii. Else: replace its content with text.
        //       - If not found and text is not null/empty: append "||xx=text".
        // 4. Return new LocalizedString with computed value.
        var languageCode = GetLanguageCode(cultureInfo);

        // Work with a mutable string representation
        var current = this.Value ?? string.Empty;

        if (IsNeutralLanguage(cultureInfo))
        {
            // Handle neutral language as the base string (before first separator)
            var separatorIndex = current.IndexOf(Separator, StringComparison.OrdinalIgnoreCase);
            if (separatorIndex == -1)
            {
                // Only neutral text present
                if (string.IsNullOrEmpty(text))
                {
                    if (string.IsNullOrEmpty(current))
                    {
                        return this;
                    }

                    return new LocalizedString(string.Empty);
                }

                return new LocalizedString(text);
            }

            // There are additional translations after the base text
            var suffix = current.Substring(separatorIndex); // includes the separator
            var newBase = text ?? string.Empty;
            return new LocalizedString(newBase + suffix);
        }

        return this.WithEntry(languageCode, text);
    }

    /// <summary>
    /// Gets the translation which is stored exactly for the specified culture, without any fallback
    /// to parent cultures, other cultures of the same language or the neutral language.
    /// For the neutral language, the neutral text is returned.
    /// </summary>
    /// <param name="cultureInfo">The culture.</param>
    /// <returns>The translation stored for exactly this culture, or <see langword="null"/> if there is none.</returns>
    public string? GetOwnTranslation(CultureInfo cultureInfo)
    {
        if (this.Value is null)
        {
            return null;
        }

        if (IsNeutralLanguage(cultureInfo))
        {
            var neutral = this.ValueInNeutralLanguage;
            return neutral.Length > 0 ? neutral : null;
        }

        return this.TryFindTranslation(GetLanguageCode(cultureInfo), out var translation) && !translation.IsEmpty
            ? new string(translation)
            : null;
    }

    /// <summary>
    /// Gets all stored translations, excluding the neutral text and metadata entries.
    /// </summary>
    /// <returns>The culture names and their translations, in stored order.</returns>
    public IEnumerable<KeyValuePair<string, string>> GetTranslations()
    {
        foreach (var (code, text) in this.GetEntries())
        {
            if (!IsMetadataCode(code))
            {
                yield return new(code, text);
            }
        }
    }

    /// <summary>
    /// Returns a new <see cref="LocalizedString"/> with the specified <see cref="SourceKey"/>.
    /// </summary>
    /// <param name="sourceKey">The source key, e.g. "MapNames/Lorencia". If <see langword="null"/> or empty, the source key is removed.</param>
    /// <returns>A new <see cref="LocalizedString"/> instance with the modified source key.</returns>
    public LocalizedString WithSourceKey(string? sourceKey)
    {
        if (sourceKey?.Contains('|', StringComparison.Ordinal) is true)
        {
            throw new ArgumentException("The source key must not contain '|'.", nameof(sourceKey));
        }

        return this.WithEntry(SourceKeyCode, sourceKey);
    }

    /// <summary>
    /// Returns a new <see cref="LocalizedString"/> with a <see cref="SourceStamp"/> of the current content.
    /// It allows to detect later if the texts were changed after they were taken from their source.
    /// </summary>
    /// <returns>A new <see cref="LocalizedString"/> instance with the updated source stamp.</returns>
    public LocalizedString WithSourceStamp()
    {
        return this.WithEntry(SourceStampCode, this.ComputeContentHash());
    }

    /// <summary>
    /// Computes a stable hash of the texts, i.e. the neutral text and all translations, excluding metadata entries.
    /// The order of the translations doesn't matter.
    /// </summary>
    /// <returns>The hash as hexadecimal string with 8 characters.</returns>
    public string ComputeContentHash()
    {
        var builder = new StringBuilder();
        builder.Append(this.ValueInNeutralLanguageAsSpan).Append('\0');
        foreach (var (code, text) in this.GetTranslations()
                     .Select(t => (Code: t.Key.ToLowerInvariant(), Text: t.Value))
                     .OrderBy(t => t.Code, StringComparer.Ordinal))
        {
            builder.Append(code).Append('=').Append(text).Append('\0');
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString()));
        return Convert.ToHexStringLower(hash.AsSpan(0, 4));
    }

    /// <inheritdoc />
    public bool Equals(LocalizedString other)
    {
        return this.Value == other.Value;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is LocalizedString other && this.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Value != null ? this.Value.GetHashCode() : 0;
    }

    private static bool IsNeutralLanguage(CultureInfo cultureInfo)
    {
        return cultureInfo.TwoLetterISOLanguageName == NeutralLanguageCode;
    }

    private static bool IsMetadataCode(string code)
    {
        return code.StartsWith(MetadataPrefix, StringComparison.Ordinal);
    }

    private static string GetLanguageCode(CultureInfo cultureInfo)
    {
        return string.IsNullOrEmpty(cultureInfo.Name) ? cultureInfo.TwoLetterISOLanguageName : cultureInfo.Name;
    }

    private static ReadOnlySpan<char> GetTranslationPart(string value, int partStart)
    {
        var part = value.AsSpan(partStart);
        var endIndex = part.IndexOf(Separator);
        return endIndex == -1 ? part : part.Slice(0, endIndex);
    }

    private string? GetMetadata(string code)
    {
        return this.Value is not null && this.TryFindTranslation(code, out var value) && !value.IsEmpty
            ? new string(value)
            : null;
    }

    private IEnumerable<(string Code, string Text)> GetEntries()
    {
        if (this.Value is null)
        {
            yield break;
        }

        var parts = this.Value.Split(Separator);
        for (var i = 1; i < parts.Length; i++)
        {
            var equalsIndex = parts[i].IndexOf('=', StringComparison.Ordinal);
            if (equalsIndex > 0)
            {
                yield return (parts[i][..equalsIndex], parts[i][(equalsIndex + 1)..]);
            }
        }
    }

    private bool TryFindTranslation(string languageCode, out ReadOnlySpan<char> translation)
    {
        var searchPattern = Separator + languageCode + "=";
        var startIndex = this.Value!.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            translation = default;
            return false;
        }

        translation = GetTranslationPart(this.Value, startIndex + searchPattern.Length);
        return true;
    }

    private bool TryFindTranslationOfLanguage(string twoLetterLanguageCode, out ReadOnlySpan<char> translation)
    {
        var searchPattern = Separator + twoLetterLanguageCode + "-";
        var startIndex = this.Value!.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            translation = default;
            return false;
        }

        var equalsIndex = this.Value.IndexOf('=', startIndex + searchPattern.Length);
        if (equalsIndex == -1)
        {
            translation = default;
            return false;
        }

        translation = GetTranslationPart(this.Value, equalsIndex + 1);
        return true;
    }

    private LocalizedString WithEntry(string languageCode, string? text)
    {
        var current = this.Value ?? string.Empty;
        var searchPattern = Separator + languageCode + "=";
        var startIndex = current.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);

        if (startIndex == -1)
        {
            // No existing translation for this language
            if (string.IsNullOrEmpty(text))
            {
                return this;
            }

            if (string.IsNullOrEmpty(current))
            {
                // No base text yet, just start with empty base and language entry
                return new LocalizedString(string.Empty + Separator + languageCode + "=" + text);
            }

            // Translations are kept in front of metadata entries.
            var entry = Separator + languageCode + "=" + text;
            var metadataIndex = IsMetadataCode(languageCode)
                ? -1
                : current.IndexOf(Separator + MetadataPrefix, StringComparison.Ordinal);
            return metadataIndex == -1
                ? new LocalizedString(current + entry)
                : new LocalizedString(current.Insert(metadataIndex, entry));
        }

        // Existing translation found
        var partStart = startIndex + searchPattern.Length;
        var span = current.AsSpan(partStart);
        var endIndex = span.IndexOf(Separator);
        var removeLength = endIndex == -1 ? current.Length - partStart : endIndex;

        if (string.IsNullOrEmpty(text))
        {
            // Remove this translation entry completely, including "||xx=" prefix
            var prefix = current.AsSpan(0, startIndex);
            ReadOnlySpan<char> suffixSpan;
            if (endIndex == -1)
            {
                suffixSpan = ReadOnlySpan<char>.Empty;
            }
            else
            {
                suffixSpan = current.AsSpan(partStart + removeLength);
            }

            var result = string.Concat(prefix, suffixSpan);
            return new LocalizedString(result);
        }
        else
        {
            // Replace the content of this translation
            var prefix = current.AsSpan(0, partStart);
            ReadOnlySpan<char> suffixSpan;
            if (endIndex == -1)
            {
                suffixSpan = ReadOnlySpan<char>.Empty;
            }
            else
            {
                suffixSpan = current.AsSpan(partStart + removeLength);
            }

            var result = string.Concat(prefix, text, suffixSpan);
            return new LocalizedString(result);
        }
    }
}