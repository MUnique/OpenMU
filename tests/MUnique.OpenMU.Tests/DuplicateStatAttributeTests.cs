// <copyright file="DuplicateStatAttributeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;

/// <summary>
/// Tests for characters which hold the same stat attribute more than once.
/// Such data shouldn't exist, but it must never keep a character from entering the game.
/// </summary>
[TestFixture]
public class DuplicateStatAttributeTests
{
    /// <summary>
    /// Tests that an attribute system can be created for a character which holds the same stat
    /// attribute twice, instead of failing with an "item with the same key" exception.
    /// </summary>
    [Test]
    public async ValueTask AttributeSystemIsCreatedForDuplicatedStatAttributeAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        character.Attributes.Add(new StatAttribute(Stats.BaseStrength, 99));

        using var attributeSystem = new ItemAwareAttributeSystem(player.Account!, character, player.GameContext.Configuration);

        Assert.That(attributeSystem[Stats.BaseStrength], Is.Not.EqualTo(0));
    }

    /// <summary>
    /// Tests that duplicated stat attributes of a character are removed when it enters the world,
    /// keeping the attribute with the highest value.
    /// </summary>
    [Test]
    public async ValueTask DuplicatedStatAttributesAreRemovedWhenEnteringWorldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        var baseStrength = character.Attributes.First(a => a.Definition == Stats.BaseStrength);
        baseStrength.Value = 28;
        character.Attributes.Add(new StatAttribute(Stats.BaseStrength, 55));

        var secondPlayer = new OfflinePlayer(player.GameContext) { Account = player.Account };
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await secondPlayer.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await secondPlayer.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        var remaining = character.Attributes.Where(a => a.Definition == Stats.BaseStrength).ToList();
        Assert.That(remaining, Has.Count.EqualTo(1));
        Assert.That(remaining[0].Value, Is.EqualTo(55));
    }
}
