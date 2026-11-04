// <copyright file="AttributeTextTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>Verifies editable raw values remain separate from localized labels.</summary>
[TestFixture]
[NonParallelizable]
public class AttributeTextTests
{
    /// <summary>Lists use Chinese labels, while forms preserve the English update key.</summary>
    [Test]
    public void ChineseHintDoesNotReplaceTheStoredName()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            using var context = new BunitContext();
            context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
            var attribute = new AttributeDefinition(Guid.NewGuid(), "Base Strength", string.Empty);
            var component = context.Render<CascadingValue<EditContext>>(parameters => parameters
                .Add(cascade => cascade.Value, new EditContext(attribute))
                .AddChildContent<TextField>(fields => fields
                    .Add(field => field.Value, attribute.Designation!)
                    .Add(field => field.ValueExpression, () => attribute.Designation!)
                    .Add(field => field.ValueChanged, value => attribute.Designation = value)));
            Assert.That(attribute.GetName(), Is.EqualTo("基础力量"));
            Assert.That(component.Find("input").GetAttribute("value"), Is.EqualTo("Base Strength"));
            Assert.That(component.Find(".form-text").TextContent, Is.EqualTo("基础力量"));
            Assert.That(attribute.Designation, Is.EqualTo("Base Strength"));
            component.Find("input").Change("Custom strength");
            Assert.That(attribute.Designation, Is.EqualTo("Custom strength"));
            Assert.That(attribute.GetName(), Is.EqualTo("Custom strength"));
            Assert.That(component.FindAll(".form-text"), Is.Empty);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    /// <summary>Recently added built-in names resolve to Chinese without altering their identifiers.</summary>
    /// <param name="name">The stored name.</param>
    /// <param name="expected">The translated caption.</param>
    [TestCase("Shield Recovery Active Everywhere", "允许在所有区域恢复防护值")]
    [TestCase("Weapon And Armor Duration Increase (MST)", "武器和防具耐久消耗减缓（大师技能）")]
    [TestCase("Jewelry And Wings Duration Increase (MST)", "首饰和翅膀耐久消耗减缓（大师技能）")]
    [TestCase("Pet Duration Increase (MST)", "宠物耐久消耗减缓（大师技能）")]
    [TestCase("Trainable Pet Duration Increase (MST)", "黑王马和天鹰耐久消耗减缓（大师技能）")]
    [TestCase("Weapon Duration Increase", "武器耐久消耗减缓")]
    [TestCase("Skill Final Damage Multiplier (PvE) (skill attribute)", "技能最终伤害倍率（对怪物，技能属性）")]
    [TestCase("Master Skill Value (skill attribute)", "大师技能当前等级数值（技能属性）")]
    public void NewAttributeNamesAreTranslated(string name, string expected)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            var attribute = new AttributeDefinition(Guid.NewGuid(), name, string.Empty);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            Assert.That(attribute.GetName(), Is.EqualTo(expected));
            Assert.That(attribute.Designation, Is.EqualTo(name));
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            Assert.That(attribute.GetName(), Is.EqualTo(name));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    /// <summary>Power-up and constant attribute captions use translated labels without modifying stored names.</summary>
    [Test]
    public void AttributeValuesUseTheDisplayLanguage()
    {
        var previous = CultureInfo.CurrentUICulture;
        var previousFormattingCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture;
            var attribute = new AttributeDefinition(Guid.NewGuid(), "Base Strength", string.Empty);
            var provider = new InMemoryPersistenceContextProvider();
            using var dataContext = provider.CreateNewContext();
            var powerUp = new PowerUpDefinition { TargetAttribute = attribute, Boost = dataContext.CreateNew<PowerUpDefinitionValue>() };
            powerUp.Boost.ConstantValue.Value = 5;
            Assert.That(powerUp.ToString(), Is.EqualTo("5 基础力量"));
            var constant = new ConstValueAttribute(5, attribute);
            Assert.That(constant.GetName(), Is.EqualTo("基础力量: 5 (基础值相加)"));
            Assert.That(constant.ToString(), Is.EqualTo("Base Strength: 5 (AddRaw)"));
            Assert.That(attribute.Designation, Is.EqualTo("Base Strength"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
            CultureInfo.CurrentCulture = previousFormattingCulture;
        }
    }

}
