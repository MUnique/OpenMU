// <copyright file="LocalizedStringSourceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Localization;

using System.Globalization;
using MUnique.OpenMU.Interfaces;
using NUnit.Framework;

/// <summary>
/// Tests for the metadata entries (source key and source stamp) and the exact lookup of <see cref="LocalizedString"/>.
/// </summary>
public class LocalizedStringSourceTests
{
    private static readonly CultureInfo ChineseSimplified = CultureInfo.GetCultureInfo("zh-CN");
    private static readonly CultureInfo ChineseTraditional = CultureInfo.GetCultureInfo("zh-TW");
    private static readonly CultureInfo Chinese = CultureInfo.GetCultureInfo("zh");
    private static readonly CultureInfo German = CultureInfo.GetCultureInfo("de");

    /// <summary>
    /// Tests that the source key is stored as metadata entry and can be read back.
    /// </summary>
    [Test]
    public void WithSourceKey_StoresMetadataEntry()
    {
        var result = new LocalizedString("Lorencia").WithSourceKey("MapNames/Lorencia");

        Assert.That(result.Value, Is.EqualTo("Lorencia||@src=MapNames/Lorencia"));
        Assert.That(result.SourceKey, Is.EqualTo("MapNames/Lorencia"));
    }

    /// <summary>
    /// Tests that the source key can be replaced and removed.
    /// </summary>
    [Test]
    public void WithSourceKey_ReplacesAndRemoves()
    {
        var value = new LocalizedString("Lorencia||de=Lorencia (de)").WithSourceKey("A/B");

        var replaced = value.WithSourceKey("C/D");
        var removed = replaced.WithSourceKey(null);

        Assert.That(replaced.SourceKey, Is.EqualTo("C/D"));
        Assert.That(removed.SourceKey, Is.Null);
        Assert.That(removed.Value, Is.EqualTo("Lorencia||de=Lorencia (de)"));
    }

    /// <summary>
    /// Tests that a source key containing the separator character is rejected.
    /// </summary>
    [Test]
    public void WithSourceKey_RejectsSeparator()
    {
        Assert.Throws<ArgumentException>(() => new LocalizedString("x").WithSourceKey("a|b"));
    }

    /// <summary>
    /// Tests that new translations are inserted before the metadata entries.
    /// </summary>
    [Test]
    public void WithTranslation_InsertsBeforeMetadata()
    {
        var value = new LocalizedString("Lorencia").WithSourceKey("MapNames/Lorencia");

        var result = value.WithTranslation(ChineseSimplified, "勇者大陆");

        Assert.That(result.Value, Is.EqualTo("Lorencia||zh-CN=勇者大陆||@src=MapNames/Lorencia"));
    }

    /// <summary>
    /// Tests that changing the neutral text keeps translations and metadata.
    /// </summary>
    [Test]
    public void WithTranslation_Neutral_KeepsMetadata()
    {
        var value = new LocalizedString("Lorencia||zh-CN=勇者大陆||@src=MapNames/Lorencia");

        var result = value.WithTranslation(CultureInfo.GetCultureInfo("en"), "Lorencia City");

        Assert.That(result.Value, Is.EqualTo("Lorencia City||zh-CN=勇者大陆||@src=MapNames/Lorencia"));
    }

    /// <summary>
    /// Tests that metadata entries are never returned as translation, not even by the fallbacks.
    /// </summary>
    [Test]
    public void GetTranslation_IgnoresMetadata()
    {
        var value = new LocalizedString("Lorencia||@src=MapNames/Lorencia||@stamp=0123abcd");

        Assert.That(value.GetTranslation(German, fallbackToNeutral: false), Is.Null);
        Assert.That(value.GetTranslation(ChineseSimplified), Is.EqualTo("Lorencia"));
        Assert.That(value.ValueInNeutralLanguage, Is.EqualTo("Lorencia"));
        Assert.That(value.GetTranslations(), Is.Empty);
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetTranslations"/> returns all translations in stored order.
    /// </summary>
    [Test]
    public void GetTranslations_ReturnsTranslationsWithoutMetadata()
    {
        var value = new LocalizedString("Lorencia||zh-CN=勇者大陆||de=Lorencia (de)||@src=MapNames/Lorencia");

        Assert.That(
            value.GetTranslations(),
            Is.EqualTo(new[] { new KeyValuePair<string, string>("zh-CN", "勇者大陆"), new KeyValuePair<string, string>("de", "Lorencia (de)") }));
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetOwnTranslation"/> doesn't fall back to other cultures of the same language.
    /// </summary>
    [Test]
    public void GetOwnTranslation_DoesNotFallBack()
    {
        var value = new LocalizedString("Lorencia||zh-CN=勇者大陆");

        Assert.That(value.GetOwnTranslation(ChineseSimplified), Is.EqualTo("勇者大陆"));
        Assert.That(value.GetOwnTranslation(Chinese), Is.Null);
        Assert.That(value.GetOwnTranslation(ChineseTraditional), Is.Null);
        Assert.That(value.GetOwnTranslation(German), Is.Null);
        Assert.That(value.GetTranslation(Chinese, fallbackToNeutral: false), Is.EqualTo("勇者大陆"), "the normal lookup still falls back");
    }

    /// <summary>
    /// Tests that <see cref="LocalizedString.GetOwnTranslation"/> returns the neutral text for the neutral language.
    /// </summary>
    [Test]
    public void GetOwnTranslation_NeutralLanguage_ReturnsNeutralText()
    {
        var value = new LocalizedString("Lorencia||zh-CN=勇者大陆");

        Assert.That(value.GetOwnTranslation(CultureInfo.GetCultureInfo("en-US")), Is.EqualTo("Lorencia"));
    }

    /// <summary>
    /// Tests that the content hash ignores the order of translations and the metadata.
    /// </summary>
    [Test]
    public void ComputeContentHash_IgnoresOrderAndMetadata()
    {
        var a = new LocalizedString("Lorencia||zh-CN=勇者大陆||de=Lorencia (de)");
        var b = new LocalizedString("Lorencia||de=Lorencia (de)||zh-CN=勇者大陆||@src=MapNames/Lorencia");

        Assert.That(a.ComputeContentHash(), Has.Length.EqualTo(8));
        Assert.That(b.ComputeContentHash(), Is.EqualTo(a.ComputeContentHash()));
    }

    /// <summary>
    /// Tests that the content hash changes when a text changes.
    /// </summary>
    [Test]
    public void ComputeContentHash_ChangesWithText()
    {
        var a = new LocalizedString("Lorencia||zh-CN=勇者大陆");

        Assert.That(a.WithTranslation(ChineseSimplified, "罗兰").ComputeContentHash(), Is.Not.EqualTo(a.ComputeContentHash()));
        Assert.That(a.WithTranslation(German, "Lorencia").ComputeContentHash(), Is.Not.EqualTo(a.ComputeContentHash()));
        Assert.That(new LocalizedString("Lorencia2||zh-CN=勇者大陆").ComputeContentHash(), Is.Not.EqualTo(a.ComputeContentHash()));
    }

    /// <summary>
    /// Tests that the source stamp detects whether the texts were changed afterwards.
    /// </summary>
    [Test]
    public void SourceStamp_DetectsChanges()
    {
        var stamped = new LocalizedString("Lorencia||zh-CN=勇者大陆").WithSourceKey("MapNames/Lorencia").WithSourceStamp();

        Assert.That(stamped.SourceStamp, Is.Not.Null);
        Assert.That(stamped.IsUnchangedSinceSourceStamp, Is.True);
        Assert.That(stamped.WithTranslation(German, "Lorencia (de)").IsUnchangedSinceSourceStamp, Is.False);
        Assert.That(stamped.WithTranslation(ChineseSimplified, "罗兰").IsUnchangedSinceSourceStamp, Is.False);
        Assert.That(stamped.WithSourceKey("Other/Key").IsUnchangedSinceSourceStamp, Is.True, "metadata isn't part of the content");
        Assert.That(new LocalizedString("Lorencia").IsUnchangedSinceSourceStamp, Is.False, "no stamp");
    }
}
