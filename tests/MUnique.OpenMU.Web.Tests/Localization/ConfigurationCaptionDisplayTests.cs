// <copyright file="ConfigurationCaptionDisplayTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>Checks that nested captions display translated text instead of serialized source metadata.</summary>
[TestFixture]
[NonParallelizable]
public class ConfigurationCaptionDisplayTests
{
    /// <summary>Configuration objects render translations without changing their stored source metadata.</summary>
    [Test]
    public void NestedCaptionsDoNotExposeSourceMetadata()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
            var caption = new LocalizedString("English caption").WithTranslation(CultureInfo.CurrentCulture, "中文名称").WithSourceKey("Test/Caption");
            Assert.That(new DropItemGroup { Description = caption }.ToString(), Is.EqualTo("中文名称"));
            Assert.That(new CharacterClass { Name = caption }.ToString(), Is.EqualTo("中文名称"));
            Assert.That(new MiniGameDefinition { Name = caption }.ToString(), Is.EqualTo("中文名称"));
            Assert.That(caption.Value, Does.Contain("@src=Test/Caption"));
            Assert.That(caption.ValueInNeutralLanguage, Is.EqualTo("English caption"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
