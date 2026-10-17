// <copyright file="NavMenuTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests;

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
        using var context = CreateContext();
        var menu = context.Render<NavMenu>();
        Assert.That(menu.FindAll("a[href='network-analyzer']"), Has.Count.EqualTo(1));

        context.Services.Dispose();

        // Toggling the menu renders it again, without resolving services in the test itself.
        Assert.DoesNotThrow(() => menu.Find("button.navbar-toggler").Click());
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var authorization = context.AddAuthorization();
        authorization.SetAuthorized("admin");
        authorization.SetRoles(AdminRoles.Administrator);

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
