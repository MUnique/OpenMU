// <copyright file="CultureSelectorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.Web.Shared.Components;
using Resources = MUnique.OpenMU.Web.AdminPanel.Properties.Resources;

/// <summary>
/// Tests that regional UI cultures remain distinct when selecting a language.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CultureSelectorTests
{
    /// <summary>
    /// Tests discovery of the deployed regional resource even when ICU enumerates its script alias.
    /// </summary>
    [Test]
    public void DiscoversSimplifiedChineseResources()
    {
        var cultures = CultureHelper.GetAvailableCultures<Resources>().Select(culture => culture.Name).ToList();
        Assert.That(cultures, Does.Contain("en"));
        Assert.That(cultures, Does.Contain("zh-CN"));
        Assert.That(cultures, Is.Unique);
        Assert.That(Resources.ResourceManager.GetString("WelcomeMessage", CultureInfo.GetCultureInfo("zh-CN")),
            Is.EqualTo("欢迎使用 OpenMU 管理后台。"));
    }

    /// <summary>
    /// Tests that the selector retains the selected culture and navigates with its full name.
    /// </summary>
    /// <param name="initialCulture">The initially selected culture.</param>
    /// <param name="targetCulture">The requested culture.</param>
    [TestCase("en", "zh-CN")]
    [TestCase("zh-CN", "en")]
    [TestCase("zh-CN", "zh-TW")]
    public void SwitchingLanguagePreservesRegionalCulture(string initialCulture, string targetCulture)
    {
        var previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(initialCulture);
            using var context = new BunitContext();
            context.Services.Configure<RequestLocalizationOptions>(options =>
                options.AddSupportedUICultures("en", "zh-CN", "zh-TW"));
            var navigation = context.Services.GetRequiredService<NavigationManager>();
            navigation.NavigateTo("/plugins?name=Bots");
            var component = context.Render<CultureSelector>();

            Assert.That(component.Find("option[selected]").GetAttribute("value"), Is.EqualTo(initialCulture));
            Assert.That(component.Find("option[value='zh-CN']").TextContent, Is.EqualTo("简体中文"));
            Assert.That(component.FindAll("option").Select(option => option.GetAttribute("value")),
                Is.EquivalentTo(new[] { "en", "zh-CN", "zh-TW" }));

            component.Find("select").Change(targetCulture);

            Assert.That(navigation.Uri, Does.Contain($"culture={targetCulture}"));
            Assert.That(navigation.Uri, Does.Contain("redirectUri=%2Fplugins%3Fname%3DBots"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }
}
