// <copyright file="LocalizedString.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Globalization;

/// <summary>
/// Represents a string which can be translated to different languages and serializes into a single string.
/// It's meant for simple usage in database fields and not for complex localization scenarios.
/// To keep compatibility with normal strings, we simply assume the first string to be in neutral (usually english) language.
/// Example: "Some text||de=Etwas Text||fr=Un peu de texte" where the first part is english, second german and third french.
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
    /// Gets the underlying serialized value of this localized string.
    /// </summary>
    public string? Value { get; }

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
        // Implementation for retrieving the localized string based on the cultureInfo
        if (this.Value is null)
        {
            return [];
        }

        if (cultureInfo.TwoLetterISOLanguageName == NeutralLanguageCode)
        {
            return this.ValueInNeutralLanguageAsSpan;
        }

        var searchPattern = Separator + cultureInfo.TwoLetterISOLanguageName + "=";
        var startIndex = this.Value.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            return fallbackToNeutral ? this.ValueInNeutralLanguageAsSpan : [];
        }

        var part = this.Value.AsSpan(startIndex + searchPattern.Length);
        var endIndex = part.IndexOf(Separator);
        if (endIndex == -1)
        {
            return part;
        }

        return part.Slice(0, endIndex);
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
        // 1. Determine language code from culture (TwoLetterISOLanguageName).
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

        var languageCode = cultureInfo.TwoLetterISOLanguageName;

        // Work with a mutable string representation
        var current = this.Value ?? string.Empty;

        if (languageCode == NeutralLanguageCode)
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

        // Handle non-neutral languages
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

            return new LocalizedString(current + Separator + languageCode + "=" + text);
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
}