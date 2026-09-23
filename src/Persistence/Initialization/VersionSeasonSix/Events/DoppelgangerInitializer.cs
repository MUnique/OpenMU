// <copyright file="DoppelgangerInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the doppelganger event.
/// </summary>
/// <remarks>
/// There is one definition for each of the four event maps. They don't differ in their
/// requirements, because the event map is chosen randomly when a party enters.
/// Each party plays in its own instance of the map.
/// The definitions don't define a ticket item, because the event accepts two different tickets,
/// which are checked by the EnterDoppelgangerAction.
/// </remarks>
internal class DoppelgangerInitializer : InitializerBase
{
    private static readonly byte[] MapNumbers = [Doppelgaenger1.Number, Doppelgaenger2.Number, Doppelgaenger3.Number, Doppelgaenger4.Number];

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DoppelgangerInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        if (!this.GameConfiguration.Monsters.Any(monster => monster.Number == DoppelgangerMonsters.FirstMonsterNumber))
        {
            new DoppelgangerMonsters(this.Context, this.GameConfiguration).Initialize();
        }

        for (var i = 0; i < MapNumbers.Length; i++)
        {
            var gameLevel = i + 1;
            var doppelganger = this.Context.CreateNew<MiniGameDefinition>();
            doppelganger.SetGuid((short)MiniGameType.Doppelganger, (short)gameLevel);
            this.GameConfiguration.MiniGameDefinitions.Add(doppelganger);
            doppelganger.Name = $"Doppelganger {gameLevel}";
            doppelganger.Description = $"Event definition for the doppelganger event on map {MapNumbers[i]}.";
            doppelganger.EnterDuration = TimeSpan.FromSeconds(30);
            doppelganger.GameDuration = TimeSpan.FromMinutes(10);
            doppelganger.ExitDuration = TimeSpan.FromMinutes(1);
            doppelganger.MaximumPlayerCount = 5;
            doppelganger.MinimumCharacterLevel = 1;
            doppelganger.MaximumCharacterLevel = 400;
            doppelganger.MinimumSpecialCharacterLevel = 1;
            doppelganger.MaximumSpecialCharacterLevel = 400;
            doppelganger.Entrance = this.GameConfiguration.Maps.First(m => m.Number == MapNumbers[i]).ExitGates.Single(g => g.IsSpawnGate);
            doppelganger.Type = MiniGameType.Doppelganger;
            doppelganger.TicketItem = null;
            doppelganger.GameLevel = (byte)gameLevel;
            doppelganger.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerParty;
            doppelganger.SaveRankingStatistics = false;
            doppelganger.AllowParty = true;
        }
    }
}
