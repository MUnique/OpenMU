// <copyright file="LocalizedStringTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.Interfaces;
using NUnit.Framework;

/// <summary>
/// Tests for the <see cref="LocalizedString"/> type and its localization behavior.
/// </summary>
public class LocalizedStringTests
{
    /// <summary>
    /// Tests that the implicit conversion from <see cref="string"/> to <see cref="LocalizedString"/> allows a <see langword="null"/> value.
    /// </summary>
    [Test]
    public void ImplicitConversion_FromString_ToLocalizedString_AllowsNull()
    {
        string? source = null;

        LocalizedString? result = source;

        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that the implicit conversion from <see cref="string"/> to <see cref="LocalizedString"/> correctly sets the <see cref="LocalizedString.Value"/>.
    /// </summary>
    [Test]
    public void ImplicitConversion_FromString_ToLocalizedString_SetsValue()
    {
        const string text = "Some text";

        LocalizedString result = text;

        Assert.That(result.Value, Is.EqualTo(text));
    }

    /// <summary>
    /// Tests that the implicit conversion from <see cref="LocalizedString"/> to nullable <see cref="string"/> allows a <see langword="null"/> source.
    /// </summary>
    [Test]
    public void ImplicitConversion_FromLocalizedString_ToNullableString_AllowsNull()
    {
        LocalizedString? source = null;

        string? result = source;

        Assert.IsNull(result);
    }

    /// <summary>
    /// Tests that the implicit conversion from <see cref="LocalizedString"/> to <see cref="string"/> returns an empty string when the underlying value is <see langword="null"/>.
    /// </summary>
    [Test]
    public void ImplicitConversion_FromLocalizedString_ToString_ReturnsEmptyWhenNull()
    {
        var source = new LocalizedString(null!);

        string result = source;

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.ToString"/> uses the current culture and returns the neutral language text for a neutral culture.
    /// </summary>
    [Test]
    public void ToString_UsesCurrentCulture_NeutralCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            var culture = new CultureInfo(LocalizedString.NeutralLanguageCode);
            CultureInfo.CurrentCulture = culture;

            var value = "Some text||de=Etwas Text";
            var localized = new LocalizedString(value);

            var result = localized.ToString();

            Assert.That(result, Is.EqualTo("Some text"));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> returns an empty span when the underlying value is <see langword="null"/>.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsEmptySpan_WhenValueIsNull()
    {
        var localized = new LocalizedString(null!);

        var result = localized.GetTranslation(new CultureInfo("en"));

        Assert.IsEmpty(result);
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> returns the neutral translation when the requested culture is neutral.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsNeutral_WhenCultureIsNeutral()
    {
        var localized = new LocalizedString("Some text||de=Etwas Text");

        var result = localized.GetTranslation(new CultureInfo("en"));

        Assert.That(result, Is.EqualTo("Some text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> returns a specific translation when it is available for the requested culture.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsSpecificTranslation_WhenAvailable()
    {
        var localized = new LocalizedString("Some text||de=Etwas Text||fr=Un peu de texte");

        var result = localized.GetTranslation(new CultureInfo("de"));

        Assert.That(result, Is.EqualTo("Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> is tolerant to extra separators between translations.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsSpecificTranslation_TolerantToExtraSeparator()
    {
        var localized = new LocalizedString("Some text|||de=Etwas Text|||fr=Un peu de texte");

        var result = localized.GetTranslation(new CultureInfo("de"));

        Assert.That(result, Is.EqualTo("Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> is not tolerant to additional separators inside the translation text itself.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsSpecificTranslation_IntolerantToExtraSeparatorInText()
    {
        var localized = new LocalizedString("Some text|||de=Etwas ||Text|||fr=Un peu de texte");

        var result = localized.GetTranslation(new CultureInfo("de"));

        Assert.That(result, Is.EqualTo("Etwas "));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> returns the last translation entry even when it does not end with a separator.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsSpecificTranslation_LastEntryWithoutTrailingSeparator()
    {
        var localized = new LocalizedString("Some text||de=Etwas Text");

        var result = localized.GetTranslation(new CultureInfo("de"));

        Assert.That(result, Is.EqualTo("Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> falls back to the neutral text when the requested translation is missing and fallback is enabled.
    /// </summary>
    [Test]
    public void GetTranslation_FallsBackToNeutral_WhenTranslationMissing_AndFallbackEnabled()
    {
        var localized = new LocalizedString("Some text||de=Etwas Text");

        var result = localized.GetTranslation(new CultureInfo("fr"), true);

        Assert.That(result, Is.EqualTo("Some text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> returns an empty span when the requested translation is missing and fallback is disabled.
    /// </summary>
    [Test]
    public void GetTranslation_ReturnsEmpty_WhenTranslationMissing_AndFallbackDisabled()
    {
        var localized = new LocalizedString("Some text||de=Etwas Text");

        var result = localized.GetTranslation(new CultureInfo("fr"), false);

        Assert.IsEmpty(result);
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslation(CultureInfo,bool)"/> performs a case-insensitive lookup for language codes.
    /// </summary>
    [Test]
    public void GetTranslation_UsesCaseInsensitiveLanguageLookup()
    {
        var localized = new LocalizedString("Some text||DE=Etwas Text");

        var result = localized.GetTranslation(new CultureInfo("de"));

        Assert.That(result, Is.EqualTo("Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> sets the neutral translation as base text when the current value is empty.
    /// </summary>
    [Test]
    public void WithTranslation_SetNeutral_OnEmptyValue_SetsBaseText()
    {
        var localized = new LocalizedString(null!);

        var result = localized.WithTranslation(new CultureInfo("en"), "Base");

        Assert.That(result.Value, Is.EqualTo("Base"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> replaces the neutral translation when no other translations exist.
    /// </summary>
    [Test]
    public void WithTranslation_ReplaceNeutral_WithoutOtherTranslations()
    {
        var localized = new LocalizedString("Base");

        var result = localized.WithTranslation(new CultureInfo("en"), "NewBase");

        Assert.That(result.Value, Is.EqualTo("NewBase"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> replaces the neutral translation and keeps the non-neutral suffix.
    /// </summary>
    [Test]
    public void WithTranslation_ReplaceNeutral_WithOtherTranslations_KeepsSuffix()
    {
        var localized = new LocalizedString("Base||de=Etwas Text||fr=Un peu de texte");

        var result = localized.WithTranslation(new CultureInfo("en"), "NewBase");

        Assert.That(result.Value, Is.EqualTo("NewBase||de=Etwas Text||fr=Un peu de texte"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> clears the neutral translation and sets the value to empty when only the neutral entry exists.
    /// </summary>
    [Test]
    public void WithTranslation_ClearNeutral_WhenOnlyNeutralExists_SetsEmpty()
    {
        var localized = new LocalizedString("Base");

        var result = localized.WithTranslation(new CultureInfo("en"), null);

        Assert.That(result.Value, Is.EqualTo(string.Empty));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> does not change the value when the neutral translation is already empty and set to <see langword="null"/>.
    /// </summary>
    [Test]
    public void WithTranslation_ClearNeutral_WhenEmptyAndNull_NoChange()
    {
        var localized = new LocalizedString(string.Empty);

        var result = localized.WithTranslation(new CultureInfo("en"), null);

        Assert.That(result.Value, Is.EqualTo(localized.Value));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> adds a non-neutral translation when no value exists yet.
    /// </summary>
    [Test]
    public void WithTranslation_AddNonNeutral_WhenNoValueYet()
    {
        var localized = new LocalizedString(null!);

        var result = localized.WithTranslation(new CultureInfo("de"), "Etwas Text");

        Assert.That(result.Value, Is.EqualTo("||de=Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> adds a non-neutral translation when a base text already exists.
    /// </summary>
    [Test]
    public void WithTranslation_AddNonNeutral_WhenBaseExists()
    {
        var localized = new LocalizedString("Base");

        var result = localized.WithTranslation(new CultureInfo("de"), "Etwas Text");

        Assert.That(result.Value, Is.EqualTo("Base||de=Etwas Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> can add multiple non-neutral translations.
    /// </summary>
    [Test]
    public void WithTranslation_AddMultipleNonNeutral()
    {
        var localized = new LocalizedString("Base");

        var withDe = localized.WithTranslation(new CultureInfo("de"), "Etwas Text");
        var withFr = withDe.WithTranslation(new CultureInfo("fr"), "Un peu de texte");

        Assert.That(withFr.Value, Is.EqualTo("Base||de=Etwas Text||fr=Un peu de texte"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> replaces an existing non-neutral translation in the middle of the list.
    /// </summary>
    [Test]
    public void WithTranslation_ReplaceExistingNonNeutral_InMiddle()
    {
        var localized = new LocalizedString("Base||de=Etwas Text||fr=Un peu de texte");

        var result = localized.WithTranslation(new CultureInfo("de"), "Neuer Text");

        Assert.That(result.Value, Is.EqualTo("Base||de=Neuer Text||fr=Un peu de texte"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> replaces an existing non-neutral translation at the end of the list.
    /// </summary>
    [Test]
    public void WithTranslation_ReplaceExistingNonNeutral_AtEnd()
    {
        var localized = new LocalizedString("Base||de=Etwas Text");

        var result = localized.WithTranslation(new CultureInfo("de"), "Neuer Text");

        Assert.That(result.Value, Is.EqualTo("Base||de=Neuer Text"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> removes an existing non-neutral translation in the middle of the list.
    /// </summary>
    [Test]
    public void WithTranslation_RemoveExistingNonNeutral_InMiddle()
    {
        var localized = new LocalizedString("Base||de=Etwas Text||fr=Un peu de texte");

        var result = localized.WithTranslation(new CultureInfo("de"), null);

        Assert.That(result.Value, Is.EqualTo("Base||fr=Un peu de texte"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> removes an existing non-neutral translation at the end of the list.
    /// </summary>
    [Test]
    public void WithTranslation_RemoveExistingNonNeutral_AtEnd()
    {
        var localized = new LocalizedString("Base||de=Etwas Text");

        var result = localized.WithTranslation(new CultureInfo("de"), string.Empty);

        Assert.That(result.Value, Is.EqualTo("Base"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.WithTranslation(CultureInfo,string)"/> does not change the value when trying to remove a non-existing non-neutral translation.
    /// </summary>
    [Test]
    public void WithTranslation_RemoveNonExistingNonNeutral_NoChange()
    {
        var localized = new LocalizedString("Base||de=Etwas Text");

        var result = localized.WithTranslation(new CultureInfo("fr"), null);

        Assert.That(result.Value, Is.EqualTo(localized.Value));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetValueInNeutralLanguage"/> returns the whole string when no separator is present.
    /// </summary>
    [Test]
    public void GetValueInNeutralLanguage_ReturnsWholeString_WhenNoSeparator()
    {
        var localized = new LocalizedString("Base");

        var result = localized.GetValueInNeutralLanguage();

        Assert.That(result, Is.EqualTo("Base"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetValueInNeutralLanguage"/> returns the text up to the first separator.
    /// </summary>
    [Test]
    public void GetValueInNeutralLanguage_ReturnsUpToFirstSeparator()
    {
        var localized = new LocalizedString("Base||de=Etwas Text");

        var result = localized.GetValueInNeutralLanguage();

        Assert.That(result, Is.EqualTo("Base"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetValueInNeutralLanguage"/> is tolerant to triple separators and still returns the base text.
    /// </summary>
    [Test]
    public void GetValueInNeutralLanguage_TolerantToTripleSeparator()
    {
        var localized = new LocalizedString("Base|||de=Etwas Text");

        var result = localized.GetValueInNeutralLanguage();

        Assert.That(result, Is.EqualTo("Base"));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetValueInNeutralLanguage"/> returns an empty span when the value is <see langword="null"/>.
    /// </summary>
    [Test]
    public void GetValueInNeutralLanguage_ReturnsEmpty_WhenValueIsNull()
    {
        var localized = new LocalizedString(null!);

        var result = localized.GetValueInNeutralLanguage();

        Assert.IsEmpty(result);
    }
}