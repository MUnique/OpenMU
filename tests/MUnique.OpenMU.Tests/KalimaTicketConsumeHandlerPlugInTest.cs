// <copyright file="KalimaTicketConsumeHandlerPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Tests the <see cref="KalimaTicketConsumeHandlerPlugIn"/>.
/// </summary>
[TestFixture]
public class KalimaTicketConsumeHandlerPlugInTest
{
    private const byte TicketSlot = 12;

    private const short FirstGateNpcNumber = 152;

    private static readonly short[] KalimaMapNumbers = [24, 25, 26, 27, 28, 29, 36];

    /// <summary>
    /// Tests that the ticket opens the gate to the highest level of Kalima which the character level allows,
    /// and that it uses up one of its usages.
    /// </summary>
    /// <param name="characterLevel">The character level.</param>
    /// <param name="expectedKalimaLevel">The expected level of Kalima.</param>
    [TestCase(40, 1)]
    [TestCase(150, 2)]
    [TestCase(349, 6)]
    [TestCase(400, 7)]
    public async Task TicketOpensGateOfKalimaLevelAsync(int characterLevel, int expectedKalimaLevel)
    {
        var player = await CreatePlayerAsync(characterLevel).ConfigureAwait(false);
        var ticket = await AddTicketAsync(player).ConfigureAwait(false);

        var consumed = await new KalimaTicketConsumeHandlerPlugIn().ConsumeItemAsync(player, ticket, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        Assert.That(ticket.Durability, Is.EqualTo(9));
        var gate = player.CurrentMap!.GetNpcsInRange(player.Position, 1).OfType<GateNpc>().Single();
        Assert.That(gate.Definition.Number, Is.EqualTo(FirstGateNpcNumber + expectedKalimaLevel - 1));
    }

    /// <summary>
    /// Tests that the ticket isn't used up when the character level is too low for the first level of Kalima.
    /// </summary>
    [Test]
    public async Task TicketIsNotConsumedBelowMinimumLevelAsync()
    {
        var player = await CreatePlayerAsync(39).ConfigureAwait(false);
        var ticket = await AddTicketAsync(player).ConfigureAwait(false);

        var consumed = await new KalimaTicketConsumeHandlerPlugIn().ConsumeItemAsync(player, ticket, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.False);
        Assert.That(ticket.Durability, Is.EqualTo(10));
        Assert.That(player.CurrentMap!.GetNpcsInRange(player.Position, 1).OfType<GateNpc>(), Is.Empty);
    }

    /// <summary>
    /// Tests that the ticket opens the gate to the highest level of Kalima whose minimum level the character reached,
    /// even when the configured minimum levels aren't ordered.
    /// </summary>
    [Test]
    public async Task TicketOpensHighestQualifyingKalimaLevelForUnorderedLevelsAsync()
    {
        var player = await CreatePlayerAsync(400).ConfigureAwait(false);
        var ticket = await AddTicketAsync(player).ConfigureAwait(false);
        var plugIn = new KalimaTicketConsumeHandlerPlugIn
        {
            Configuration = new KalimaTicketConsumeHandlerConfiguration
            {
                MinimumLevels = [40, 131, 181, 231, 281, 500, 350],
            },
        };

        var consumed = await plugIn.ConsumeItemAsync(player, ticket, null, FruitUsage.Undefined).ConfigureAwait(false);

        Assert.That(consumed, Is.True);
        var gate = player.CurrentMap!.GetNpcsInRange(player.Position, 1).OfType<GateNpc>().Single();
        Assert.That(gate.Definition.Number, Is.EqualTo(FirstGateNpcNumber + 6));
    }

    private static async ValueTask<Player> CreatePlayerAsync(int characterLevel)
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var configuration = gameContext.Configuration;
        for (var i = 0; i < KalimaMapNumbers.Length; i++)
        {
            configuration.Monsters.Add(new MUnique.OpenMU.Persistence.BasicModel.MonsterDefinition
            {
                Number = (short)(FirstGateNpcNumber + i),
                ObjectKind = NpcObjectKind.Gate,
            });

            var map = new MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition
            {
                Number = KalimaMapNumbers[i],
                TerrainData = new byte[ushort.MaxValue + 3],
            };
            map.ExitGates.Add(new MUnique.OpenMU.Persistence.BasicModel.ExitGate
            {
                Id = Guid.NewGuid(),
                Map = map,
                X1 = 10,
                Y1 = 10,
                X2 = 12,
                Y2 = 12,
            });
            configuration.Maps.Add(map);
        }

        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await player.ClientReadyAfterMapChangeAsync().ConfigureAwait(false);
        player.Attributes![Stats.Level] = characterLevel;
        return player;
    }

    private static async ValueTask<Item> AddTicketAsync(Player player)
    {
        var ticket = new Item
        {
            Definition = new ItemDefinition { Group = 13, Number = 48, Width = 1, Height = 1, Durability = 10 },
            Durability = 10,
            ItemSlot = TicketSlot,
        };
        await player.Inventory!.AddItemAsync(TicketSlot, ticket).ConfigureAwait(false);
        return ticket;
    }
}
