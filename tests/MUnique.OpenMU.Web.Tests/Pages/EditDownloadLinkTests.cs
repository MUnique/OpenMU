// <copyright file="EditDownloadLinkTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Components.Toast;
using MUnique.OpenMU.Web.Shared.Services;
using MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

/// <summary>Tests the json download link of the editor.</summary>
[TestFixture]
public sealed class EditDownloadLinkTests
{
    /// <summary>
    /// The link is relative to the base address of the page, which contains the path base of the host,
    /// e.g. <c>/admin/</c> in the distributed deployment. A link from the root would miss it.
    /// </summary>
    [Test]
    public void DownloadLinkIsRelativeToTheBaseAddress()
    {
        using var context = new BunitContext();
        var persistenceContext = new Mock<IContext>();
        persistenceContext.Setup(c => c.GetAsync<GameConfiguration>(It.IsAny<CancellationToken>())).ReturnsAsync([new GameConfiguration()]);
        var provider = new Mock<IPersistenceContextProvider>();
        provider.Setup(p => p.CreateNewContext()).Returns(persistenceContext.Object);
        using IDataSource<GameConfiguration> source = new GameConfigurationDataSource(NullLogger<GameConfigurationDataSource>.Instance, provider.Object);
        context.Services.AddSingleton(source);
        context.Services.AddSingleton(provider.Object);
        context.Services.AddSingleton(Mock.Of<IToastService>());
        context.Services.AddSingleton<IModalService>(new TestModalService(true));
        context.Services.AddSingleton<LoadingOverlayService>();
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton<IChangeNotificationService, ChangeNotificationService>();

        var editor = context.Render<EditConfig>(p => p.Add(e => e.TypeString, typeof(GameConfiguration).FullName!));
        editor.WaitForState(() => editor.FindAll("a[download]").Count == 1);

        var link = editor.Find("a[download]").GetAttribute("href");
        Assert.That(link, Does.StartWith($"download/{nameof(GameConfiguration)}/{nameof(GameConfiguration)}_"));
    }
}
