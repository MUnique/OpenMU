// <copyright file="TranslationCoverageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Localization;

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Bunit;
using Microsoft.AspNetCore.Components.QuickGrid;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Web.Shared.Components;
using MUnique.OpenMU.Web.Shared.Components.Form;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>Verifies plugin metadata and the shared controls which previously fell back to English.</summary>
[TestFixture]
[NonParallelizable]
public class TranslationCoverageTests
{
    /// <summary>Checks completeness and format arguments of every included Chinese resource set.</summary>
    [Test]
    public void ChineseResourceSetsMatchNeutralKeysAndPlaceholders()
    {
        var sets = new (Type Anchor, string Resource)[]
        {
            (typeof(MUnique.OpenMU.DataModel.Configuration.GameConfiguration), "ModelResources"),
            (typeof(MUnique.OpenMU.DataModel.Configuration.GameConfiguration), "Resources"),
            (typeof(MUnique.OpenMU.Interfaces.LocalizedString), "ModelResources"),
            (typeof(MaximumConnectionsPerIpPlugInConfiguration), "ModelResources"),
            (typeof(MaximumConnectionsPerIpPlugInConfiguration), "PlugInResources"),
            (typeof(MUnique.OpenMU.GameServer.GameServer), "PlugInResources"),
            (typeof(MUnique.OpenMU.Network.IpResolverType), "Resources"),
            (typeof(MUnique.OpenMU.Network.IpResolverType), "PlugInResources"),
            (typeof(MUnique.OpenMU.Persistence.Initialization.DataInitializationBase), "PlugInResources"),
            (typeof(MUnique.OpenMU.Web.AdminPanel.Auth.RoleCaption), "Resources"),
            (typeof(FieldCaption), "Resources"),
            (typeof(FieldCaption), "PlugInPointResources"),
        };
        foreach (var (anchor, resource) in sets)
        {
            var assembly = anchor.Assembly;
            var name = $"{assembly.GetName().Name}.Properties.{resource}";
            var manager = new ResourceManager(name, assembly);
            using var neutral = manager.GetResourceSet(CultureInfo.InvariantCulture, true, false)!;
            using var chinese = manager.GetResourceSet(CultureInfo.GetCultureInfo("zh-CN"), true, false)!;
            Assert.That(chinese, Is.Not.Null, name);
            foreach (DictionaryEntry entry in neutral)
            {
                var key = (string)entry.Key;
                var original = (string)entry.Value!;
                var translated = chinese.GetString(key);
                Assert.That(translated, Is.Not.Null, $"{name}.{key}");
                if (original.Length > 0)
                {
                    Assert.That(translated, Is.Not.Empty, $"{name}.{key}");
                }

                var originalArguments = Regex.Matches(original, @"\{[^{}]+\}").Select(match => match.Value);
                var translatedArguments = Regex.Matches(translated!, @"\{[^{}]+\}").Select(match => match.Value);
                Assert.That(translatedArguments, Is.EquivalentTo(originalArguments), $"{name}.{key}");
            }
        }
    }

    /// <summary>Ensures all built-in display attributes resolve valid resource properties.</summary>
    [Test]
    public void BuiltInPluginDisplayResourcesResolveInBothLanguages()
    {
        var assemblies = new[]
        {
            typeof(MaximumConnectionsPerIpPlugInConfiguration).Assembly,
            typeof(MUnique.OpenMU.GameServer.GameServer).Assembly,
            typeof(MUnique.OpenMU.Network.IpResolverType).Assembly,
            typeof(MUnique.OpenMU.Persistence.Initialization.DataInitializationBase).Assembly,
        };
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            foreach (var language in new[] { "en", "zh-CN" })
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(language);
                foreach (var type in assemblies.SelectMany(assembly => assembly.GetTypes()))
                {
                    foreach (var member in new MemberInfo[] { type }.Concat(type.GetProperties()))
                    {
                        if (member.GetCustomAttribute<DisplayAttribute>() is not { } display)
                        {
                            continue;
                        }

                        Assert.DoesNotThrow(() => display.GetName(), $"{language}: {type.FullName}.{member.Name} name");
                        Assert.DoesNotThrow(() => display.GetDescription(), $"{language}: {type.FullName}.{member.Name} description");
                    }
                }
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    /// <summary>Checks actual paging and refreshes when the total changes, without replacing pagination state.</summary>
    [Test]
    public async Task ChinesePaginatorUsesTheOriginalGridStateAsync()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            using var context = new BunitContext();
            var state = new PaginationState { ItemsPerPage = 10 };
            context.JSInterop.Mode = JSRuntimeMode.Loose;
            var items = Enumerable.Range(1, 21).ToList();
            var grid = context.Render<QuickGrid<int>>(parameters => parameters
                .Add(p => p.Items, items.AsQueryable())
                .Add(p => p.Pagination, state));
            var component = context.Render<LocalizedPaginator>(parameters => parameters.Add(p => p.State, state));
            Assert.That(component.Find("div").TextContent, Does.Contain("共 21 条"));
            var next = component.FindAll("button").Single(b => b.TextContent == "下一页");
            next.Click();
            Assert.That(state.CurrentPageIndex, Is.EqualTo(1));
            Assert.That(component.Find("div").TextContent, Does.Contain("第 2 页，共 3 页"));
            component.FindAll("button").Single(b => b.TextContent == "末页").Click();
            Assert.That(state.CurrentPageIndex, Is.EqualTo(2));
            Assert.That(component.FindAll("button").Single(b => b.TextContent == "下一页").HasAttribute("disabled"), Is.True);
            component.FindAll("button").Single(b => b.TextContent == "首页").Click();
            items.Clear();
            await grid.InvokeAsync(() => grid.Instance.RefreshDataAsync());
            component.WaitForAssertion(() => Assert.That(component.Find("div").TextContent, Does.Contain("共 0 条")));
            Assert.That(component.Find("div").TextContent, Does.Contain("第 1 页，共 1 页"));
            Assert.That(component.FindAll("button").All(button => button.HasAttribute("disabled")), Is.True);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    /// <summary>Checks localized validation labels and extension point names without changing identifiers.</summary>
    [Test]
    public void MetadataCaptionsUseChineseResources()
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            var model = new MaximumConnectionsPerIpPlugInConfiguration();
            Assert.That(FieldCaption.Get(new(model, nameof(model.MaximumConnectionsPerIp))), Is.EqualTo("每个 IP 的最大同时连接数"));
            Assert.That(PlugInPointCaption.Get("Periodic Tasks"), Is.EqualTo("定时任务"));
            Assert.That(PlugInPointCaption.Get("External plugin"), Is.EqualTo("External plugin"));
            Assert.That(MUnique.OpenMU.Web.AdminPanel.Auth.RoleCaption.Get("Administrator,Viewer"), Is.EqualTo("管理员, 只读用户"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
