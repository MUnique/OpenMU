// <copyright file="CashShopControllerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Api;

using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.AdminPanel.API;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Tests for the <see cref="CashShopController"/>, which grants cash shop coins through the public API.
/// </summary>
[TestFixture]
public class CashShopControllerTests
{
    private InMemoryPersistenceContextProvider _persistenceContextProvider = null!;

    private CashShopController _controller = null!;

    /// <summary>
    /// Sets up an in-memory database with two accounts, and the controller.
    /// </summary>
    /// <returns>The task.</returns>
    [SetUp]
    public async Task SetUpAsync()
    {
        this._persistenceContextProvider = new InMemoryPersistenceContextProvider();
        var gameConfigurationSource = new Mock<IDataSource<GameConfiguration>>();
        gameConfigurationSource
            .Setup(s => s.GetOwnerAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GameConfiguration());
        this._controller = new CashShopController(this._persistenceContextProvider, gameConfigurationSource.Object, Mock.Of<ILogger<CashShopController>>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        using var context = this._persistenceContextProvider.CreateNewPlayerContext(new GameConfiguration());
        foreach (var loginName in new[] { "buyer", "other" })
        {
            var account = context.CreateNew<Account>();
            account.LoginName = loginName;
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// A grant is created as pending, and shows up in the summary of the account.
    /// </summary>
    [Test]
    public async Task GrantCreatesPendingGrantAsync()
    {
        var result = await this._controller.GrantAsync("buyer", CreateRequest(100, "payment-1"), CancellationToken.None).ConfigureAwait(false);

        Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        var grant = (CashShopCoinGrantInfo)((ObjectResult)result).Value!;
        Assert.That(grant.Amount, Is.EqualTo(100));
        Assert.That(grant.CoinType, Is.EqualTo(CashShopCoinType.WCoinC));
        Assert.That(grant.AppliedAt, Is.Null);

        var summary = (CashShopCoinSummary)((OkObjectResult)await this._controller.GetAsync("buyer", CancellationToken.None).ConfigureAwait(false)).Value!;
        Assert.That(summary.WCoinC, Is.Zero, "The game server applies the grant, not the API.");
        Assert.That(summary.Grants.Select(g => g.Id), Is.EqualTo(new[] { grant.Id }));
    }

    /// <summary>
    /// A repeated request with the same reference doesn't grant the coins twice, but answers with the existing grant.
    /// </summary>
    [Test]
    public async Task RepeatedReferenceIsGrantedOnceAsync()
    {
        var first = (CashShopCoinGrantInfo)((ObjectResult)await this._controller.GrantAsync("buyer", CreateRequest(100, "payment-1"), CancellationToken.None).ConfigureAwait(false)).Value!;

        var result = await this._controller.GrantAsync("buyer", CreateRequest(100, "payment-1"), CancellationToken.None).ConfigureAwait(false);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        Assert.That(((CashShopCoinGrantInfo)((OkObjectResult)result).Value!).Id, Is.EqualTo(first.Id));
        var summary = (CashShopCoinSummary)((OkObjectResult)await this._controller.GetAsync("buyer", CancellationToken.None).ConfigureAwait(false)).Value!;
        Assert.That(summary.Grants, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// A reference which another account already got is a conflict.
    /// </summary>
    [Test]
    public async Task ReferenceOfOtherAccountIsConflictAsync()
    {
        await this._controller.GrantAsync("other", CreateRequest(100, "payment-1"), CancellationToken.None).ConfigureAwait(false);

        var result = await this._controller.GrantAsync("buyer", CreateRequest(100, "payment-1"), CancellationToken.None).ConfigureAwait(false);

        Assert.That(result, Is.InstanceOf<ConflictResult>());
    }

    /// <summary>
    /// Grants without reference are always created.
    /// </summary>
    [Test]
    public async Task GrantsWithoutReferenceAreCreatedAsync()
    {
        await this._controller.GrantAsync("buyer", CreateRequest(100, null), CancellationToken.None).ConfigureAwait(false);
        var result = await this._controller.GrantAsync("buyer", CreateRequest(-30, " "), CancellationToken.None).ConfigureAwait(false);

        Assert.That(((ObjectResult)result).StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        var summary = (CashShopCoinSummary)((OkObjectResult)await this._controller.GetAsync("buyer", CancellationToken.None).ConfigureAwait(false)).Value!;
        Assert.That(summary.Grants.Select(g => g.Amount), Is.EquivalentTo(new[] { 100, -30 }));
    }

    /// <summary>
    /// An unknown account is answered with not found, and a zero amount or an unknown coin type is invalid.
    /// A grant of an unknown coin type couldn't be applied by the game server.
    /// </summary>
    [Test]
    public async Task InvalidRequestsAsync()
    {
        var unknownCoinType = CreateRequest(100, null);
        unknownCoinType.CoinType = (CashShopCoinType)7;

        Assert.That(await this._controller.GrantAsync("unknown", CreateRequest(100, null), CancellationToken.None).ConfigureAwait(false), Is.InstanceOf<NotFoundResult>());
        Assert.That(await this._controller.GetAsync("unknown", CancellationToken.None).ConfigureAwait(false), Is.InstanceOf<NotFoundResult>());
        Assert.That(await this._controller.GrantAsync("buyer", CreateRequest(0, null), CancellationToken.None).ConfigureAwait(false), Is.InstanceOf<ObjectResult>().With.Property(nameof(ObjectResult.StatusCode)).EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(await this._controller.GrantAsync("buyer", unknownCoinType, CancellationToken.None).ConfigureAwait(false), Is.InstanceOf<ObjectResult>().With.Property(nameof(ObjectResult.StatusCode)).EqualTo(StatusCodes.Status400BadRequest));
    }

    private static CashShopCoinGrantRequest CreateRequest(int amount, string? reference)
    {
        return new CashShopCoinGrantRequest
        {
            CoinType = CashShopCoinType.WCoinC,
            Amount = amount,
            Reason = "Test",
            Reference = reference,
        };
    }
}
