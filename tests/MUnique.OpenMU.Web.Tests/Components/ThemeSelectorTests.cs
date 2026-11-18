// <copyright file="ThemeSelectorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Components;

using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Web.Shared.Components;

/// <summary>
/// Tests for the <see cref="ThemeSelector"/> component.
/// </summary>
[TestFixture]
public class ThemeSelectorTests
{
    /// <summary>
    /// The menu is closed initially and lists the three themes with the current one checked when opened.
    /// </summary>
    [Test]
    public void Toggle_OpensMenuWithCurrentThemeChecked()
    {
        using var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var component = context.Render<ThemeSelector>(parameters => parameters.Add(selector => selector.Theme, "dark"));

        Assert.That(component.FindAll(".theme-selector__menu"), Is.Empty);

        component.Find(".theme-selector__toggle").Click();

        var items = component.FindAll(".theme-selector__menu .dropdown-item");
        Assert.That(items, Has.Count.EqualTo(3));
        var checkedItems = component.FindAll(".theme-selector__menu .dropdown-item[aria-checked=true]");
        Assert.That(checkedItems, Has.Count.EqualTo(1));
        Assert.That(checkedItems[0].ClassList, Does.Contain("active"));
        Assert.That(checkedItems[0].QuerySelector(".oi-moon"), Is.Not.Null);
    }

    /// <summary>
    /// Selecting a theme stores it through the JavaScript module and closes the menu, without navigating.
    /// </summary>
    [Test]
    public void Select_StoresThemeAndClosesMenu()
    {
        using var context = new BunitContext();
        var module = context.JSInterop.SetupModule("./_content/MUnique.OpenMU.Web.Shared/Components/ThemeSelector.razor.js");
        module.Setup<string?>("current").SetResult("light");
        var setInvocation = module.SetupVoid("set", _ => true);
        setInvocation.SetVoidResult();
        var navigationManager = context.Services.GetRequiredService<NavigationManager>();
        var initialUri = navigationManager.Uri;
        var component = context.Render<ThemeSelector>(parameters => parameters.Add(selector => selector.Theme, "light"));

        component.Find(".theme-selector__toggle").Click();
        component.FindAll(".theme-selector__menu .dropdown-item").Last().Click();

        Assert.That(setInvocation.Invocations, Has.Count.EqualTo(1));
        Assert.That(setInvocation.Invocations.Single().Arguments, Is.EqualTo(new object?[] { "OpenMU.Theme", "auto" }));
        Assert.That(component.FindAll(".theme-selector__menu"), Is.Empty);
        Assert.That(component.Find(".theme-selector__toggle .oi-contrast"), Is.Not.Null);
        Assert.That(navigationManager.Uri, Is.EqualTo(initialUri));
    }
}
