// <copyright file="MoneyRateEditorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Web.AdminPanel.Components.Configuration;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests for the <see cref="MoneyRateEditor"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MoneyRateEditorTests
{
    private BunitContext _context = null!;

    private CultureInfo _previousCulture = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context with the english culture, because the input is formatted with it.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this._previousCulture = CultureInfo.CurrentCulture;
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._context = new BunitContext();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        CultureInfo.CurrentCulture = this._previousCulture;
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// The editor shows the configured value and stores a changed one in the configuration.
    /// </summary>
    [Test]
    public void ChangingTheValueUpdatesTheConfiguration()
    {
        var (configuration, moneyRate) = CreateConfiguration(1.5f);
        var saveCount = 0;

        var cut = this._context.Render<MoneyRateEditor>(parameters => parameters
            .Add(p => p.Configuration, configuration)
            .Add(p => p.OnSave, EventCallback.Factory.Create(this, () => saveCount++)));

        var input = cut.Find("input#money-rate");
        Assert.That(input.GetAttribute("value"), Is.EqualTo("1.5"));

        input.Change("2");

        Assert.That(moneyRate.Value, Is.EqualTo(2f));
        Assert.That(saveCount, Is.EqualTo(1), "The change should be saved immediately.");
    }

    /// <summary>
    /// A negative rate is not stored, because it would be an invalid multiplier.
    /// </summary>
    [Test]
    public void NegativeValueIsRejected()
    {
        var (configuration, moneyRate) = CreateConfiguration(1.5f);
        var saveCount = 0;

        var cut = this._context.Render<MoneyRateEditor>(parameters => parameters
            .Add(p => p.Configuration, configuration)
            .Add(p => p.OnSave, EventCallback.Factory.Create(this, () => saveCount++)));

        cut.Find("input#money-rate").Change("-1");

        Assert.That(moneyRate.Value, Is.EqualTo(1.5f));
        Assert.That(saveCount, Is.Zero);
    }

    /// <summary>
    /// Without the global money rate attribute, the editor doesn't offer a field, because the
    /// attribute belongs to the configuration and can't be created by the editor.
    /// </summary>
    [Test]
    public void HidesTheFieldWhenTheAttributeIsMissing()
    {
        var cut = this._context.Render<MoneyRateEditor>(parameters => parameters
            .Add(p => p.Configuration, new BasicModel.GameConfiguration()));

        Assert.That(cut.FindAll("input#money-rate"), Is.Empty);
    }

    private static (BasicModel.GameConfiguration Configuration, BasicModel.ConstValueAttribute MoneyRate) CreateConfiguration(float value)
    {
        var configuration = new BasicModel.GameConfiguration();
        var definition = new BasicModel.AttributeDefinition(Stats.MoneyAmountRate.Id, "Money Drop Amount Rate", "Defines a multiplier for the amount of a money drop.");
        configuration.Attributes.Add(definition);
        var moneyRate = new BasicModel.ConstValueAttribute(value, definition);
        configuration.GlobalBaseAttributeValues.Add(moneyRate);
        return (configuration, moneyRate);
    }
}
