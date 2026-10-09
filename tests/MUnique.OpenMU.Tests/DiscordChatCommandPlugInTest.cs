// <copyright file="DiscordChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.AccountLinking;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Tests for the <see cref="DiscordChatCommandPlugIn"/>.
/// </summary>
[TestFixture]
public class DiscordChatCommandPlugInTest
{
    /// <summary>
    /// Tests that <c>/discord link</c> creates a pending link with the selected character,
    /// and that <c>/discord unlink</c> removes it.
    /// </summary>
    [Test]
    public async Task LinkCreatesCodeAndUnlinkRemovesItAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Hero";
        var plugIn = new DiscordChatCommandPlugIn();

        await plugIn.HandleCommandAsync(player, "/discord link").ConfigureAwait(false);
        var pendingLink = await this.GetLinkAsync(player).ConfigureAwait(false);
        await plugIn.HandleCommandAsync(player, "/discord unlink").ConfigureAwait(false);

        Assert.That(pendingLink, Is.Not.Null);
        Assert.That(pendingLink!.CodeHash, Is.Not.Null);
        Assert.That(pendingLink.ExternalUserId, Is.Null);
        Assert.That(pendingLink.CharacterName, Is.EqualTo("Hero"));
        Assert.That(await this.GetLinkAsync(player).ConfigureAwait(false), Is.Null);
    }

    private async Task<DataModel.Entities.AccountExternalLink?> GetLinkAsync(Player player)
    {
        using var context = player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration);
        return await context.GetAccountExternalLinkAsync(player.Account!.GetId(), AccountLinkService.DiscordProvider).ConfigureAwait(false);
    }
}
