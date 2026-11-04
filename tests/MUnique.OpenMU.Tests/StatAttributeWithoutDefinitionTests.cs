// <copyright file="StatAttributeWithoutDefinitionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for stat attributes without a definition. A running server which applies a data update can
/// resolve a new attribute of a character class to <see langword="null"/>, and a character which
/// entered the world then got a stat attribute without a definition. Such a character must still be
/// able to enter the game.
/// </summary>
[TestFixture]
public class StatAttributeWithoutDefinitionTests
{
    /// <summary>
    /// Tests that a stat attribute without a definition is removed when the character enters the world,
    /// instead of failing to create the attribute system.
    /// </summary>
    [Test]
    public async ValueTask StatAttributeWithoutDefinitionIsRemovedWhenEnteringWorldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        character.Attributes.Add(new StatAttribute(null!, 0));

        await EnterWorldAsync(player, character).ConfigureAwait(false);

        Assert.That(character.Attributes.Where(a => a.Definition is null), Is.Empty);
    }

    /// <summary>
    /// Tests that a stat attribute of the character class without an attribute doesn't add
    /// a stat attribute without a definition to the character.
    /// </summary>
    [Test]
    public async ValueTask ClassStatAttributeWithoutAttributeIsNotAddedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        character.CharacterClass!.StatAttributes.Add(new StatAttributeDefinition { BaseValue = 0 });

        await EnterWorldAsync(player, character).ConfigureAwait(false);

        Assert.That(character.Attributes.Where(a => a.Definition is null), Is.Empty);
    }

    private static async ValueTask EnterWorldAsync(Player player, Character character)
    {
        var secondPlayer = new OfflinePlayer(player.GameContext) { Account = player.Account };
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await secondPlayer.SetSelectedCharacterAsync(character).ConfigureAwait(false);
    }
}
