// <copyright file="CashShopCoinsPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Pages;

using System.Globalization;
using System.Threading;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Tests for the <see cref="CashShopCoins"/> page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CashShopCoinsPageTests
{
    private BunitContext _context = null!;

    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    private Mock<IToastService> _toastService = null!;

    private CultureInfo _previousUiCulture = null!;

    /// <summary>
    /// Sets up the test context with an account.
    /// </summary>
    /// <returns>The task.</returns>
    [SetUp]
    public async Task SetupAsync()
    {
        this._previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        using (var context = this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration()))
        {
            var account = context.CreateNew<Account>();
            account.LoginName = "buyer";
            account.WCoinC = 1234;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        var gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GameConfiguration());
        this._toastService = new Mock<IToastService>();

        this._context = new BunitContext();
        this._context.Services.AddSingleton<IPersistenceContextProvider>(this._persistenceContextProvider);
        this._context.Services.AddSingleton(gameConfigurationSource.Object);
        this._context.Services.AddSingleton(this._toastService.Object);
        this._context.Services.AddSingleton(Mock.Of<ILogger<CashShopCoins>>());
        this._context.ComponentFactories.AddStub<MUnique.OpenMU.Web.Shared.Components.Breadcrumb>();
    }

    /// <summary>
    /// Restores the culture and disposes the test context.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        this._context.Dispose();
        CultureInfo.CurrentUICulture = this._previousUiCulture;
    }

    /// <summary>
    /// The page shows the balances, and a submitted grant is shown as pending.
    /// </summary>
    [Test]
    public void GrantIsShownAsPending()
    {
        var cut = this._context.Render<CashShopCoins>(parameters => parameters.Add(p => p.LoginName, "buyer"));
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(1234.ToString("N0"))));

        cut.Find("#amount").Change("500");
        cut.Find("#reason").Change("Event reward");
        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain("Event reward")));
        Assert.That(cut.Markup, Does.Contain(Resources.CashShopPending));
        Assert.That(cut.Markup, Does.Contain(string.Format(Resources.CashShopPendingAmount, "+500")));
        this._toastService.Verify(t => t.ShowSuccess(Resources.CashShopCoinsGranted), Times.Once);
    }

    /// <summary>
    /// A zero amount isn't granted.
    /// </summary>
    [Test]
    public void ZeroAmountIsNotGranted()
    {
        var cut = this._context.Render<CashShopCoins>(parameters => parameters.Add(p => p.LoginName, "buyer"));
        cut.WaitForAssertion(() => Assert.That(cut.Markup, Does.Contain(1234.ToString("N0"))));

        cut.Find("form").Submit();

        this._toastService.Verify(t => t.ShowError(Resources.CashShopAmountMustNotBeZero), Times.Once);
        Assert.That(cut.Markup, Does.Contain(Resources.CashShopNoGrants));
    }
}
