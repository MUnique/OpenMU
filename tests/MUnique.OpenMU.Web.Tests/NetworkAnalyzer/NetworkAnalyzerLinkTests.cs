// <copyright file="NetworkAnalyzerLinkTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the links which lead to the network analyzer page.
/// </summary>
[TestFixture]
public class NetworkAnalyzerLinkTests
{
    /// <summary>
    /// Tests if an online account is linked to the traffic of its connection.
    /// </summary>
    [Test]
    public void OnlineAccountIsLinkedToItsTraffic()
    {
        using var context = CreateContext(new TestCaptureService());

        var component = context.Render<LoggedIn>();

        var link = component.Find("a[title='Analyze the network traffic']");
        Assert.That(link.GetAttribute("href"), Is.EqualTo("network-analyzer/player/3/Test%20Account"));
    }

    /// <summary>
    /// Tests if the link is not offered when the capture service isn't registered, which is
    /// the case in the distributed deployment.
    /// </summary>
    [Test]
    public void NoLinkWithoutTheCaptureService()
    {
        using var context = CreateContext();

        var component = context.Render<LoggedIn>();

        Assert.That(component.FindAll("a[title='Analyze the network traffic']"), Is.Empty);
    }

    private static BunitContext CreateContext(IPacketCaptureService? captureService = null)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var serverProvider = new Mock<IServerProvider>();
        serverProvider.Setup(provider => provider.Servers).Returns(new List<IManageableServer>());

        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(new LoggedInAccountService(Mock.Of<ILoginServer>(), serverProvider.Object));
        context.Services.AddSingleton(new OfflineAccountService(serverProvider.Object));
        context.Services.AddSingleton<IDataService<LoggedInAccount>>(
            new TestDataService<LoggedInAccount>([new LoggedInAccount("Test Account", 3)]));
        context.Services.AddSingleton<IDataService<OfflineAccount>>(new TestDataService<OfflineAccount>([]));
        if (captureService is not null)
        {
            context.Services.AddSingleton(captureService);
        }

        return context;
    }

    private sealed class TestDataService<T> : IDataService<T>
        where T : class
    {
        private readonly List<T> _items;

        public TestDataService(List<T> items)
        {
            this._items = items;
        }

        public Task<List<T>> GetAsync(int offset, int count)
        {
            return Task.FromResult(this._items.Skip(offset).Take(count).ToList());
        }
    }
}
