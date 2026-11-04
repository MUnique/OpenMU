// <copyright file="ConfigurationCaptionDisplayTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;

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
    /// <summary>Quest captions follow the UI language and preserve stored quest names.</summary>
    /// <param name="cultureName">The UI language.</param>
    /// <param name="expected">The expected localized caption.</param>
    [TestCase("zh-CN", "#18, 最近完成的任务: —, 当前任务: 猎杀蜘蛛！")]
    [TestCase("en", "#18, Last Finished Quest: —, Active Quest: Hunt spiders!")]
    public void QuestStateCaptionUsesUiCulture(string cultureName, string expected)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            var name = new LocalizedString("Hunt spiders!")
                .WithTranslation(CultureInfo.GetCultureInfo("zh-CN"), "猎杀蜘蛛！")
                .WithSourceKey("Test/Quest");
            var storedName = name.Value;
            var state = new CharacterQuestState { Group = 18, ActiveQuest = new QuestDefinition { Name = name } };
            Assert.That(state.GetName(), Is.EqualTo(expected));
            Assert.That(state.ActiveQuest.Name.Value, Is.EqualTo(storedName));
            state.LastFinishedQuest = state.ActiveQuest;
            state.ActiveQuest = null;
            Assert.That(state.GetName(), Does.Not.Contain("@src="));
            Assert.That(state.GetName(), Does.EndWith(": —"));
            state.LastFinishedQuest = null;
            Assert.That(state.GetName().Count(character => character == '—'), Is.EqualTo(2));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

}
