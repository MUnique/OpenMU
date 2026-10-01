// <copyright file="LocalizedStringResourceExtensionsTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using System.Resources;
using MUnique.OpenMU.Interfaces;

/// <summary>Tests language-independent resource construction and satellite discovery.</summary>
[TestFixture]
[NonParallelizable]
public class LocalizedStringResourceExtensionsTests
{
    private const string ResourceBaseName = "MUnique.OpenMU.Tests.Localization.TestNames";

    /// <summary>Gets the resource manager used by the typed expression.</summary>
    public static ResourceManager ResourceManager { get; } = new(ResourceBaseName, typeof(LocalizedStringResourceExtensionsTests).Assembly);

    /// <summary>Gets the example resource in the current UI culture.</summary>
    public static string Example => ResourceManager.GetString(nameof(Example))!;

    /// <summary>Full culture codes coexist and construction does not depend on the current UI language.</summary>
    [Test]
    [SetCulture("zh-TW")]
    [SetUICulture("zh-TW")]
    public void TypedConstructionIncludesEveryExplicitTranslation()
    {
        var name = LocalizedString.FromResource(() => Example);
        Assert.That(name.ValueInNeutralLanguage, Is.EqualTo("Example"));
        Assert.That(name.GetTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("简体示例"));
        Assert.That(name.GetTranslation(CultureInfo.GetCultureInfo("zh-TW")), Is.EqualTo("繁體範例"));
        Assert.That(name.GetTranslation(CultureInfo.GetCultureInfo("de-AT")), Is.EqualTo("Beispiel"));
        Assert.That(name.Value, Does.Contain("||zh-CN=").And.Contain("||zh-TW=").And.Contain("||de="));
        Assert.That(ResourceManager.AvailableCultures.Select(c => c.Name), Is.EquivalentTo(new[] { "de", "zh-CN", "zh-TW" }));
    }

    /// <summary>A missing satellite entry must not be filled with fallback text.</summary>
    [Test]
    public void SparseSatelliteDoesNotCopyNeutralText()
    {
        Assert.That(ResourceManager.GetLocalizedString("NeutralOnly").Value, Is.EqualTo("Neutral only"));
    }

    /// <summary>Previous fallback lookups do not create artificial translation cultures.</summary>
    [Test]
    public void CachedFallbackResourceSetsAreNotOwnSatellites()
    {
        var resources = new ResourceManager(ResourceBaseName, typeof(LocalizedStringResourceExtensionsTests).Assembly);
        Assert.That(resources.GetString("Example", CultureInfo.GetCultureInfo("de-AT")), Is.EqualTo("Beispiel"));
        Assert.That(resources.GetString("Example", CultureInfo.GetCultureInfo("fr-CA")), Is.EqualTo("Example"));
        Assert.That(resources.AvailableCultures.Select(c => c.Name), Is.EquivalentTo(new[] { "de", "zh-CN", "zh-TW" }));
        Assert.That(resources.GetLocalizedString("Example").Value, Does.Not.Contain("||de-AT=").And.Not.Contain("||fr"));
    }

    /// <summary>Invalid expressions and unknown keys fail instead of creating empty names.</summary>
    [Test]
    public void InvalidResourcesAreRejected()
    {
        Assert.That(() => LocalizedString.FromResource(() => "Example"), Throws.ArgumentException);
        Assert.That(() => LocalizedString.FromResource(null!), Throws.ArgumentNullException);
        Assert.That(() => ResourceManager.GetLocalizedString("Unknown"), Throws.ArgumentException);
    }
}
