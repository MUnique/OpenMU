// <copyright file="ReEnterWorldAfterLogoutTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Tests that a player who went back to the character selection is shown to itself again
/// when it enters the world with a character.
/// </summary>
[TestFixture]
public class ReEnterWorldAfterLogoutTest
{
    /// <summary>
    /// The player always observes itself, so it was never removed from its own list of observed
    /// objects when it left the map. After a logout back to the character selection, entering the
    /// world again didn't send the player to itself as "new player in scope", and clients which
    /// create their own character from that message (e.g. the web client) showed no character.
    /// </summary>
    [Test]
    public async ValueTask PlayerIsShownToItselfAfterReEnteringTheWorldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        var view = Mock.Get(player.ViewPlugIns.GetPlugIn<INewPlayersInScopePlugIn>()!);
        view.Verify(v => v.NewPlayersInScopeAsync(It.Is<IEnumerable<Player>>(p => p.Contains(player)), It.IsAny<bool>()), Times.Once);

        await new LogoutAction().LogoutAsync(player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        view.Verify(v => v.NewPlayersInScopeAsync(It.Is<IEnumerable<Player>>(p => p.Contains(player)), It.IsAny<bool>()), Times.Exactly(2));
    }
}
