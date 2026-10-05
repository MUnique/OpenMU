// <copyright file="LocalizedStringResourceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Localization;

using System.Globalization;
using System.Resources;
using MUnique.OpenMU.Interfaces;
using NUnit.Framework;

/// <summary>
/// Tests for <see cref="LocalizedStringResources"/> and <see cref="LocalizedStringResourceExtensions"/>.
/// </summary>
public class LocalizedStringResourceTests
{
    /// <summary>
    /// Tests that the available cultures contain exactly the cultures with their own resources.
    /// </summary>
    [Test]
    public void AvailableCultures_ContainsOnlyCulturesWithOwnResources()
    {
        // A lookup with fallback first, so that the ResourceManager caches the parent's set under zh-HK.
        _ = TestCaptions.ResourceManager.GetString(nameof(TestCaptions.Lorencia), CultureInfo.GetCultureInfo("zh-HK"));

        var names = TestCaptions.ResourceManager.AvailableCultures.Select(c => c.Name).ToList();

        Assert.That(names, Is.EquivalentTo(new[] { "de", "zh-CN", "zh-TW" }));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedStringResourceExtensions.FromResource"/> includes all translations, the source key and the source stamp.
    /// </summary>
    [Test]
    public void FromResource_IncludesTranslationsAndSource()
    {
        var result = LocalizedString.FromResource(() => TestCaptions.Lorencia);

        Assert.That(result.ValueInNeutralLanguage, Is.EqualTo("Lorencia"));
        Assert.That(result.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("勇者大陆"));
        Assert.That(result.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-TW")), Is.EqualTo("勇者大陸"));
        Assert.That(result.GetOwnTranslation(CultureInfo.GetCultureInfo("de")), Is.Null, "the sparse german resources don't contain Lorencia");
        Assert.That(result.SourceKey, Is.EqualTo("TestCaptions/Lorencia"));
        Assert.That(result.IsUnchangedSinceSourceStamp, Is.True);
    }

    /// <summary>
    /// Tests that sparse translations are only added where they exist.
    /// </summary>
    [Test]
    public void FromResource_SparseTranslations()
    {
        var result = LocalizedString.FromResource(() => TestCaptions.Devias);

        Assert.That(result.GetTranslations().Select(t => t.Key), Is.EquivalentTo(new[] { "de", "zh-CN" }));
    }

    /// <summary>
    /// Tests that an expression which isn't a resource property is rejected.
    /// </summary>
    [Test]
    public void FromResource_RejectsInvalidExpressions()
    {
        Assert.Throws<ArgumentException>(() => LocalizedString.FromResource(() => "Lorencia"));
        Assert.Throws<ArgumentException>(() => LocalizedString.FromResource(() => TestCaptions.NotAResource));
    }

    /// <summary>
    /// Tests that the current source value can be retrieved by the source key.
    /// </summary>
    [Test]
    public void GetFromSource_ReturnsCurrentSourceValue()
    {
        var original = LocalizedString.FromResource(() => TestCaptions.Lorencia);
        var customized = original.WithTranslation(CultureInfo.GetCultureInfo("zh-CN"), "罗兰");

        var fromSource = customized.GetFromSource();

        Assert.That(customized.IsUnchangedSinceSourceStamp, Is.False);
        Assert.That(fromSource, Is.EqualTo(original));
    }

    /// <summary>
    /// Tests that unknown or unregistered sources aren't resolved.
    /// </summary>
    [Test]
    public void GetFromSource_UnknownSource_ReturnsNull()
    {
        Assert.That(new LocalizedString("x").GetFromSource(), Is.Null);
        Assert.That(new LocalizedString("x").WithSourceKey("NotRegistered/Lorencia").GetFromSource(), Is.Null);
        Assert.That(new LocalizedString("x").WithSourceKey("TestCaptions/NoSuchKey").GetFromSource(), Is.Null);
        Assert.That(new LocalizedString("x").WithSourceKey("NoSeparator").GetFromSource(), Is.Null);
    }

    /// <summary>
    /// Tests that a source name can't be registered for two different resources.
    /// </summary>
    [Test]
    public void Register_ConflictingName_Throws()
    {
        var name = "Conflict" + Guid.NewGuid().ToString("N");
        var first = new ResourceManager(typeof(TestCaptions));
        var second = new ResourceManager(typeof(TestCaptions));
        LocalizedStringResources.Register(name, first);
        LocalizedStringResources.Register(name, first);

        Assert.Throws<InvalidOperationException>(() => LocalizedStringResources.Register(name, second));
        Assert.Throws<ArgumentException>(() => LocalizedStringResources.Register("Invalid/Name", second));
    }

    /// <summary>
    /// Tests that a resource without registration produces a value without source key.
    /// </summary>
    [Test]
    public void GetLocalizedString_UnregisteredResource_HasNoSource()
    {
        var unregistered = new ResourceManager("MUnique.OpenMU.Tests.Localization.TestCaptions", typeof(TestCaptions).Assembly);

        var result = unregistered.GetLocalizedString(nameof(TestCaptions.Lorencia));

        Assert.That(result.SourceKey, Is.Null);
        Assert.That(result.SourceStamp, Is.Null);
        Assert.That(result.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("勇者大陆"));
    }
}
