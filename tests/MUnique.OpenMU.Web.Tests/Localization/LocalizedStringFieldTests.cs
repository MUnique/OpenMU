// <copyright file="LocalizedStringFieldTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>Tests that regional translations have distinct editor fields.</summary>
[TestFixture]
public class LocalizedStringFieldTests
{
    /// <summary>Labels and identifiers distinguish cultures, and editing one preserves the others.</summary>
    [Test]
    public void RegionalTranslationsHaveDistinctFields()
    {
        using var context = new BunitContext();
        context.Services.Configure<RequestLocalizationOptions>(options => options.AddSupportedUICultures("en", "zh-CN", "zh-TW"));
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();
        var model = new ItemOptionType { Name = "Armor||zh-CN=铠||zh-TW=鎧" };
        var component = context.Render<CascadingValue<EditContext>>(parameters => parameters
            .Add(parameter => parameter.Value, new EditContext(model))
            .AddChildContent<LocalizedStringField>(field => field
                .Add(parameter => parameter.Value, model.Name)
                .Add(parameter => parameter.ValueExpression, () => model.Name)
                .Add(parameter => parameter.ValueChanged, (LocalizedString value) => model.Name = value)));

        Assert.That(component.FindAll(".badge").Select(badge => badge.TextContent), Is.EqualTo(new[] { "en", "zh-CN", "zh-TW" }));
        Assert.That(component.FindAll("input").Select(input => input.Id), Is.EqualTo(new[] { "Name", "Name_zh-CN", "Name_zh-TW" }));
        Assert.That(component.Find("#Name_zh-CN").GetAttribute("value"), Is.EqualTo("铠"));
        Assert.That(component.Find("#Name_zh-TW").GetAttribute("value"), Is.EqualTo("鎧"));
        component.Find("#Name_zh-CN").Change("自定义铠");
        Assert.That(model.Name.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-CN")), Is.EqualTo("自定义铠"));
        Assert.That(model.Name.GetOwnTranslation(CultureInfo.GetCultureInfo("zh-TW")), Is.EqualTo("鎧"));
        Assert.That(model.Name.ValueInNeutralLanguage, Is.EqualTo("Armor"));
    }
}
