// <copyright file="PlayerAttributeReleaseTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.Login;

/// <summary>
/// Tests that the attribute system of a character is released when it's no longer used.
/// </summary>
/// <remarks>
/// The power-ups of items wrap elements of the game configuration (e.g. the value of an item option), which
/// live as long as the server. A power-up subscribes to its element, so an attribute system whose power-ups
/// were never disposed keeps the whole player alive - which happened to every player who ever logged out.
/// </remarks>
[TestFixture]
public class PlayerAttributeReleaseTest
{
    /// <summary>
    /// Tests if a disposed player isn't kept alive by the configuration element of an equipped item's option.
    /// </summary>
    [Test]
    public async ValueTask DisposedPlayerIsNotKeptAliveByItemConfigurationAsync()
    {
        var configurationElement = new SimpleElement(10, AggregateType.AddRaw);

        var playerReference = await CreateAndDisposePlayerWithItemPowerUpAsync(configurationElement).ConfigureAwait(false);

        CollectibleAssert.IsCollected(playerReference, "disposed player");
        GC.KeepAlive(configurationElement);
    }

    /// <summary>
    /// Tests if selecting a character again releases the attribute system of the previously selected one.
    /// </summary>
    [Test]
    public async ValueTask CharacterSwitchReleasesPreviousAttributeSystemAsync()
    {
        var configurationElement = new SimpleElement(10, AggregateType.AddRaw);
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var previousAttributesReference = AddItemPowerUp(player, configurationElement);

        var character = player.SelectedCharacter!;
        await new LogoutAction().LogoutAsync(player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(character).ConfigureAwait(false);

        CollectibleAssert.IsCollected(previousAttributesReference, "attribute system of the previously selected character");
        GC.KeepAlive(configurationElement);
        GC.KeepAlive(player);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async ValueTask<WeakReference> CreateAndDisposePlayerWithItemPowerUpAsync(IElement configurationElement)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        AddItemPowerUp(player, configurationElement);
        await player.DisposeAsync().ConfigureAwait(false);
        return new WeakReference(player);
    }

    /// <summary>
    /// Adds a power-up for an equipped item to the player, just like the inventory does when an item gets equipped.
    /// </summary>
    /// <returns>The weak reference to the attribute system which got the power-up.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference AddItemPowerUp(Player player, IElement configurationElement)
    {
        var attributes = player.Attributes!;
        var powerUp = new PowerUpWrapper(configurationElement, Stats.DefenseBase, attributes);
        attributes.ItemPowerUps.Add(new Item(), [powerUp]);
        Assert.That(attributes[Stats.DefenseBase], Is.EqualTo(configurationElement.Value), "The power-up should have been applied.");
        return new WeakReference(attributes);
    }
}
