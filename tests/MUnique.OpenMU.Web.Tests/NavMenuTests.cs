// <copyright file="NavMenuTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

using MUnique.OpenMU.Web.Tests.AdminAuth;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Auth;
using MUnique.OpenMU.Web.AdminPanel.Components.Layout;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="NavMenu"/>.
/// </summary>
[TestFixture]
public class NavMenuTests
{
    /// <summary>
    /// Tests that the menu can render after the services of its connection are disposed.
    /// The menu renders again when its background loading completes, which can happen after
    /// the browser tab was closed; it threw an <see cref="ObjectDisposedException"/> then.
    /// </summary>
    [Test]
    public void RendersAfterServicesAreDisposed()
    {
        using var context = CreateContext(AdminPolicies.Administrator);
        var menu = context.Render<NavMenu>();
        Assert.That(menu.FindAll("a[href='network-analyzer']"), Has.Count.EqualTo(1));

        context.Services.Dispose();

        // Toggling the menu renders it again, without resolving services in the test itself.
        Assert.DoesNotThrow(() => menu.Find("button.navbar-toggler").Click());
    }

    /// <summary>
    /// Tests that the system and inspect pages are grouped into collapsible groups,
    /// which are collapsed at first and expand with a click on their header.
    /// </summary>
    [Test]
    public void SystemAndInspectPagesAreGrouped()
    {
        using var context = CreateContext(AdminPolicies.Administrator);
        var menu = context.Render<NavMenu>();
        menu.WaitForState(() => menu.FindAll("a.nav-link[href='setup']").Count == 1);

        var groups = menu.FindAll("li.dropdown");
        var system = groups[0];
        var inspect = groups[1];
        Assert.That(system.QuerySelectorAll("a[href='users']"), Has.Length.EqualTo(1));
        Assert.That(system.QuerySelectorAll("a[href='api-keys']"), Has.Length.EqualTo(1));
        Assert.That(inspect.QuerySelectorAll("a[href='network-analyzer']"), Has.Length.EqualTo(1));
        Assert.That(menu.FindAll("li.dropdown > div.show"), Is.Empty);

        menu.FindAll("li.dropdown > a.dropdown-toggle")[0].Click();

        Assert.That(menu.FindAll("li.dropdown")[0].ClassList, Does.Contain("expanded"));
        Assert.That(menu.FindAll("li.dropdown > div.show"), Has.Count.EqualTo(1));

        menu.FindAll("li.dropdown > a.dropdown-toggle")[1].Click();

        Assert.That(menu.FindAll("li.dropdown > div.show"), Has.Count.EqualTo(2));
    }

    private static BunitContext CreateContext(params string[] policies)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var authorization = context.AddAuthorization();
        authorization.SetAuthorized("admin");
        authorization.SetRoles(AdminRoles.Administrator);
        authorization.SetPolicies(policies);

        var contextProvider = new Mock<IMigratableDatabaseContextProvider>();
        contextProvider.Setup(p => p.CanConnectToDatabaseAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var plugInManager = new PlugInManager(null, NullLoggerFactory.Instance, null, null);

        context.Services.AddSingleton(contextProvider.Object);
        context.Services.AddSingleton(new SetupService(contextProvider.Object, plugInManager));
        context.Services.AddSingleton(new DataUpdateService(contextProvider.Object, plugInManager));
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton<IPacketCaptureService>(new TestCaptureService());
        return context;
    }
}
