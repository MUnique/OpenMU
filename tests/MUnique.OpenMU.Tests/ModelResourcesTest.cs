// // <copyright file="ModelResourcesTest.cs" company="MUnique">
// // Licensed under the MIT License. See LICENSE file in the project root for full license information.
// // </copyright>

namespace MUnique.OpenMU.Tests;

using System.Globalization;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Unit tests for verifying that <see cref="ModelResourceProvider"/> returns expected captions and descriptions
/// for types, properties and enum values, including fallbacks for unknown languages and types.
/// </summary>
[TestFixture]
public class ModelResourcesTest
{
    /// <summary>
    /// Verifies that <see cref="ModelResourceProvider.GetTypeCaption{T}"/> returns the expected caption
    /// for <see cref="AreaSkillSettings"/> in English culture.
    /// </summary>
    [Test]
    public void TypeCaption()
    {
        var typeName = ModelResourceProvider.GetTypeCaption<AreaSkillSettings>(CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("Area Skill Settings"));
    }

    /// <summary>
    /// Verifies that <see cref="ModelResourceProvider.GetPluralizedTypeCaption{T}"/> returns the pluralized caption
    /// for <see cref="Account"/> in English culture.
    /// </summary>
    [Test]
    public void TypeCaptionPlural()
    {
        var typeName = ModelResourceProvider.GetPluralizedTypeCaption<Account>(CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("Accounts"));
    }

    /// <summary>
    /// Verifies that <see cref="ModelResourceProvider.GetTypeDescription{T}"/> returns the expected (empty) description
    /// for <see cref="AreaSkillSettings"/> in English culture.
    /// </summary>
    [Test]
    public void TypeDescription()
    {
        var typeName = ModelResourceProvider.GetTypeDescription<AreaSkillSettings>(CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo(""));
    }

    /// <summary>
    /// Verifies that <see cref="ModelResourceProvider.GetPropertyCaption{T}"/> returns a humanized caption
    /// for the property <see cref="AreaSkillSettings.DelayBetweenHits"/>.
    /// </summary>
    [Test]
    public void PropertyCaption()
    {
        var typeName = ModelResourceProvider.GetPropertyCaption<AreaSkillSettings>(nameof(AreaSkillSettings.DelayBetweenHits), CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("Delay Between Hits"));
    }

    /// <summary>
    /// Verifies that a caption can be retrieved for a property inherited from a base type,
    /// here <see cref="ExitGate.X1"/>.
    /// </summary>
    [Test]
    public void InheritedPropertyCaption()
    {
        var typeName = ModelResourceProvider.GetPropertyCaption<ExitGate>(nameof(ExitGate.X1), CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("X1"));
    }

    /// <summary>
    /// Verifies that <see cref="ModelResourceProvider.GetPropertyDescription{T}"/> returns the expected (empty) description
    /// for property <see cref="AreaSkillSettings.DelayBetweenHits"/>.
    /// </summary>
    [Test]
    public void PropertyDescription()
    {
        var typeName = ModelResourceProvider.GetPropertyDescription<AreaSkillSettings>(nameof(AreaSkillSettings.DelayBetweenHits), CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo(""));
    }

    /// <summary>
    /// Verifies that an unknown language (Swahili here) falls back to a default caption for a known type.
    /// </summary>
    [Test]
    public void TypeCaptionUnknownLanguage()
    {
        var typeName = ModelResourceProvider.GetTypeCaption<AreaSkillSettings>(CultureInfo.GetCultureInfo("sw"));
        Assert.That(typeName, Is.EqualTo("Area Skill Settings"));
    }

    /// <summary>
    /// Verifies that unknown types get a humanized caption from their type name (splitting PascalCase).
    /// </summary>
    [Test]
    public void TypeCaptionUnknownType()
    {
        var typeName = ModelResourceProvider.GetTypeCaption<ModelResourcesTest>(CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("Model Resources Test"));
    }

    /// <summary>
    /// Verifies that an unknown type and property name gets a humanized caption (PascalCase splitting).
    /// </summary>
    [Test]
    public void PropertyCaptionUnknownTypeAndProperty()
    {
        var typeName = ModelResourceProvider.GetPropertyCaption<ModelResourcesTest>("FooBar", CultureInfo.GetCultureInfo("en"));
        Assert.That(typeName, Is.EqualTo("Foo Bar"));
    }

    /// <summary>
    /// Verifies that the generic overload of <see cref="ModelResourceProvider.GetEnumCaption{TEnum}"/>
    /// returns the expected caption for an enum value.
    /// </summary>
    [Test]
    public void EnumCaptionGeneric()
    {
        var caption = ModelResourceProvider.GetEnumCaption<AccountState>(AccountState.GameMaster, CultureInfo.GetCultureInfo("en"));
        Assert.That(caption, Is.EqualTo("Game Master"));
    }

    /// <summary>
    /// Verifies that the non-generic overload of <see cref="ModelResourceProvider.GetEnumCaption(Type, object, CultureInfo)"/>
    /// returns the expected caption for an enum value.
    /// </summary>
    [Test]
    public void EnumCaption()
    {
        var caption = ModelResourceProvider.GetEnumCaption(typeof(AccountState), AccountState.GameMaster, CultureInfo.GetCultureInfo("en"));
        Assert.That(caption, Is.EqualTo("Game Master"));
    }
}