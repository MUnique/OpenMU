// <copyright file="KanturuInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the Kanturu Refinery Tower event.
/// </summary>
internal class KanturuInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KanturuInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public KanturuInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var kanturu = this.Context.CreateNew<MiniGameDefinition>();
        kanturu.SetGuid((short)MiniGameType.Kanturu, 1);
        this.GameConfiguration.MiniGameDefinitions.Add(kanturu);
        kanturu.Name = "Kanturu Refinery Tower";
        kanturu.Description = "Event definition for the Kanturu Refinery Tower event.";
        kanturu.EnterDuration = TimeSpan.FromMinutes(3);
        kanturu.GameDuration = TimeSpan.FromMinutes(135);
        kanturu.ExitDuration = TimeSpan.FromMinutes(1);
        kanturu.MaximumPlayerCount = 10;
        kanturu.MinimumCharacterLevel = 350;
        kanturu.MaximumCharacterLevel = 400;
        kanturu.MinimumSpecialCharacterLevel = 350;
        kanturu.MaximumSpecialCharacterLevel = 400;
        kanturu.Entrance = this.GameConfiguration.Maps.First(m => m.Number == KanturuEvent.Number).ExitGates.Single(g => g.IsSpawnGate);
        kanturu.Type = MiniGameType.Kanturu;
        kanturu.TicketItem = null;
        kanturu.GameLevel = 1;
        kanturu.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        kanturu.SaveRankingStatistics = false;
        kanturu.AllowParty = true;
    }
}
