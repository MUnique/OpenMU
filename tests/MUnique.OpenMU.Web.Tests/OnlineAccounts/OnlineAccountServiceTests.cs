// <copyright file="OnlineAccountServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.OnlineAccounts;

using Moq;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the online-accounts data services in a distributed deployment,
/// where no game server runs in the same process.
/// </summary>
[TestFixture]
public class OnlineAccountServiceTests
{
    /// <summary>
    /// Without in-process game servers no character can be resolved (distributed fallback),
    /// but accounts are still listed ordered by login name.
    /// </summary>
    [Test]
    public async Task LoggedInAccounts_WithoutInProcessServers_HaveNoCharacter()
    {
        var loginServer = new Mock<ILoginServer>();
        loginServer.Setup(s => s.GetSnapshotAsync()).Returns(
            new ValueTask<Dictionary<string, byte>>(new Dictionary<string, byte> { ["bUser"] = 1, ["aUser"] = 2 }));
        var service = new LoggedInAccountService(loginServer.Object, EmptyServerProvider());

        var result = await service.GetAsync(0, 20);

        Assert.That(result.Select(a => a.LoginName), Is.EqualTo(new[] { "aUser", "bUser" }));
        Assert.That(result.All(a => a.CharacterName is null), Is.True);
    }

    /// <summary>
    /// Pagination applies after ordering.
    /// </summary>
    [Test]
    public async Task LoggedInAccounts_PaginationAppliesAfterOrdering()
    {
        var loginServer = new Mock<ILoginServer>();
        loginServer.Setup(s => s.GetSnapshotAsync()).Returns(
            new ValueTask<Dictionary<string, byte>>(new Dictionary<string, byte> { ["cUser"] = 1, ["bUser"] = 1, ["aUser"] = 1 }));
        var service = new LoggedInAccountService(loginServer.Object, EmptyServerProvider());

        var result = await service.GetAsync(1, 1);

        Assert.That(result.Select(a => a.LoginName), Is.EqualTo(new[] { "bUser" }));
    }

    /// <summary>
    /// Without in-process game servers there are no offline sessions to list
    /// and the off-level tab stays hidden.
    /// </summary>
    [Test]
    public async Task OfflineAccounts_WithoutInProcessServers_AreEmpty()
    {
        var service = new OfflineAccountService(EmptyServerProvider());

        Assert.That(await service.GetAsync(0, 20), Is.Empty);
        Assert.That(service.IsOfflevelFeatureAvailable(), Is.False);
    }

    /// <summary>
    /// Without in-process game servers there are no bots to list
    /// and the bot tab stays hidden.
    /// </summary>
    [Test]
    public async Task BotAccounts_WithoutInProcessServers_AreEmpty()
    {
        var service = new BotAccountService(EmptyServerProvider());

        Assert.That(await service.GetAsync(0, 20), Is.Empty);
        Assert.That(service.IsBotFeatureAvailable(), Is.False);
    }

    private static IServerProvider EmptyServerProvider()
    {
        var provider = new Mock<IServerProvider>();
        provider.Setup(p => p.Servers).Returns(new List<IManageableServer>());
        return provider.Object;
    }
}
