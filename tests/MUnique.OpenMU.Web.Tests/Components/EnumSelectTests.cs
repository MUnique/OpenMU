// <copyright file="EnumSelectTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="EnumSelect{TValue}"/> component.
/// </summary>
[TestFixture]
public class EnumSelectTests
{
    /// <summary>
    /// All enum values render as options and only the current value is marked selected.
    /// </summary>
    [Test]
    public void CurrentValue_RendersSingleSelectedOption()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        var account = new Account { State = AccountState.Normal };
        var component = context.Render<EnumSelect<AccountState>>(parameters => parameters
            .Add(field => field.Value, account.State)
            .Add(field => field.ValueExpression, () => account.State)
            .Add(field => field.ValueChanged, value => account.State = value));

        var values = Enum.GetValues(typeof(AccountState)).OfType<AccountState>().Select(v => v.ToString()).ToList();
        var options = component.FindAll("option");
        Assert.That(options.Select(o => o.GetAttribute("value")), Is.EqualTo(values));

        var selected = component.FindAll("option[selected]");
        Assert.That(selected, Has.Count.EqualTo(1));
        Assert.That(selected[0].GetAttribute("value"), Is.EqualTo("Normal"));
    }

    /// <summary>
    /// Unmatched attributes flow through to the select element.
    /// </summary>
    [Test]
    public void AdditionalAttributes_FlowThroughToSelect()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        var account = new Account { State = AccountState.Normal };
        var component = context.Render<EnumSelect<AccountState>>(parameters => parameters
            .Add(field => field.Value, account.State)
            .Add(field => field.ValueExpression, () => account.State)
            .Add(field => field.ValueChanged, value => account.State = value)
            .AddUnmatched("data-test", "enum-select"));

        Assert.That(component.Find("select").GetAttribute("data-test"), Is.EqualTo("enum-select"));
    }

    /// <summary>
    /// Changing the selection updates the bound value.
    /// </summary>
    [Test]
    public void ChangingSelection_UpdatesBoundValue()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        var account = new Account { State = AccountState.Normal };
        var component = context.Render<EnumSelect<AccountState>>(parameters => parameters
            .Add(field => field.Value, account.State)
            .Add(field => field.ValueExpression, () => account.State)
            .Add(field => field.ValueChanged, value => account.State = value));

        component.Find("select").Change("Banned");

        Assert.That(account.State, Is.EqualTo(AccountState.Banned));
    }
}
