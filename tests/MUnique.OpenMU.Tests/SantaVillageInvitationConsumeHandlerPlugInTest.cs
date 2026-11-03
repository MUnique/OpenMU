// <copyright file="SantaVillageInvitationConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="SantaVillageInvitationConsumeHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class SantaVillageInvitationConsumeHandlerPlugInTest
{
    private const byte InvitationSlot = 12;

    /// <summary>
    /// Tests that the invitation warps the player to the spawn gate of the santa village and uses up one of its usages.
    /// </summary>
    [Test]
    public async Task InvitationWarpsToSantaVillageAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var santaVillage = AddSantaVillage(gameContext.Configuration);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var invitation = await AddInvitationAsync(player).ConfigureAwait(false);

        var consumed = await new SantaVillageInvitationConsumeHandlerPlugIn().ConsumeItemAsync(player, invitation, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(invitation.Durability, Is.EqualTo(9));
        Assert.That(player.SelectedCharacter!.CurrentMap, Is.SameAs(santaVillage));
        var gate = santaVillage.ExitGates.Single();
        Assert.That(player.SelectedCharacter.PositionX, Is.InRange(gate.X1, gate.X2));
        Assert.That(player.SelectedCharacter.PositionY, Is.InRange(gate.Y1, gate.Y2));
    }

    /// <summary>
    /// Tests that the invitation isn't used up when the santa village isn't configured.
    /// </summary>
    [Test]
    public async Task InvitationIsNotConsumedWithoutSantaVillageAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var invitation = await AddInvitationAsync(player).ConfigureAwait(false);
        var map = player.SelectedCharacter!.CurrentMap;

        var consumed = await new SantaVillageInvitationConsumeHandlerPlugIn().ConsumeItemAsync(player, invitation, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(invitation.Durability, Is.EqualTo(10));
        Assert.That(player.SelectedCharacter.CurrentMap, Is.SameAs(map));
    }

    /// <summary>
    /// Tests that the invitation isn't used up when the santa village is deactivated.
    /// </summary>
    [Test]
    public async Task InvitationIsNotConsumedForInactiveSantaVillageAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var santaVillage = AddSantaVillage(gameContext.Configuration);
        santaVillage.IsActive = false;
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var invitation = await AddInvitationAsync(player).ConfigureAwait(false);
        var map = player.SelectedCharacter!.CurrentMap;

        var consumed = await new SantaVillageInvitationConsumeHandlerPlugIn().ConsumeItemAsync(player, invitation, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(invitation.Durability, Is.EqualTo(10));
        Assert.That(player.SelectedCharacter.CurrentMap, Is.SameAs(map));
    }

    /// <summary>
    /// Tests that the invitation warps the player to the map which is configured in the plugin.
    /// </summary>
    [Test]
    public async Task InvitationWarpsToConfiguredMapAsync()
    {
        const short configuredMapNumber = 63;
        var gameContext = GameContextTestHelper.CreateGameContext();
        var targetMap = AddSantaVillage(gameContext.Configuration, configuredMapNumber);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var invitation = await AddInvitationAsync(player).ConfigureAwait(false);
        var plugIn = new SantaVillageInvitationConsumeHandlerPlugIn
        {
            Configuration = new SantaVillageInvitationConsumeHandlerConfiguration { MapNumber = configuredMapNumber },
        };

        var consumed = await plugIn.ConsumeItemAsync(player, invitation, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(player.SelectedCharacter!.CurrentMap, Is.SameAs(targetMap));
    }

    private static GameMapDefinition AddSantaVillage(GameConfiguration configuration, short mapNumber = 62)
    {
        var santaVillage = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
        {
            Number = mapNumber,
            TerrainData = new byte[ushort.MaxValue + 3],
        };
        santaVillage.ExitGates.Add(new MUnique.OpenMU.Persistence.BasicModel.ExitGate
        {
            Id = Guid.NewGuid(),
            Map = santaVillage,
            X1 = 221,
            Y1 = 23,
            X2 = 225,
            Y2 = 27,
            IsSpawnGate = true,
        });
        configuration.Maps.Add(santaVillage);
        return santaVillage;
    }

    private static async ValueTask<Item> AddInvitationAsync(Player player)
    {
        var invitation = new Item
        {
            Definition = new ItemDefinition { Group = 13, Number = 66, Width = 1, Height = 1, Durability = 10 },
            Durability = 10,
            ItemSlot = InvitationSlot,
        };
        await player.Inventory!.AddItemAsync(InvitationSlot, invitation).ConfigureAwait(false);
        return invitation;
    }
}
