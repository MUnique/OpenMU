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
public readonly struct LocalizedString
{
    private const string Separator = "||";
    private const string NeutralLanguageCode = "en";

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedString"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public LocalizedString(string? value)
    {
        this.Value = value;
    }

    public string? Value { get; }

    // override to string to keep compatibility with normal strings
    public override string? ToString()
    {
        return this.GetTranslation(CultureInfo.CurrentCulture).ToString();
    }

    // override implicit conversion to keep compatibility with normal strings
    public static implicit operator string?(LocalizedString? localizedString)
    {
        return localizedString?.Value;
    }

    public static implicit operator LocalizedString?(string? localizedString)
    {
        if (localizedString is null)
        {
            return null;
        }

        return new LocalizedString(localizedString);
    }

    public ReadOnlySpan<char> GetTranslation(CultureInfo cultureInfo)
    {
        // Implementation for retrieving the localized string based on the cultureInfo
        if (this.Value is null)
        {
            return null;
        }

        if (cultureInfo.TwoLetterISOLanguageName == NeutralLanguageCode)
        {
            return this.GetValueInNeutralLanguage();
        }

        var searchPattern = Separator + cultureInfo.TwoLetterISOLanguageName + "=";
        var startIndex = this.Value.IndexOf(searchPattern, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            return this.GetValueInNeutralLanguage();
        }

        var part = this.Value.AsSpan(startIndex + searchPattern.Length);
        var endIndex = part.IndexOf(Separator);
        if (endIndex == -1)
        {
            return part;
        }

        return part.Slice(0, endIndex);
    }

    private ReadOnlySpan<char> GetValueInNeutralLanguage()
    {
        if (this.Value is null)
        {
            return null;
        }

        var separatorIndex = this.Value.IndexOf(Separator, StringComparison.OrdinalIgnoreCase);
        if (separatorIndex == -1)
        {
            return this.Value.AsSpan();
        }

        return this.Value.AsSpan(0, separatorIndex);
    }
}