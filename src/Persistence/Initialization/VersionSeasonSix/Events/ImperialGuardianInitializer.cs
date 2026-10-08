// <copyright file="ImperialGuardianInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the imperial guardian event.
/// </summary>
/// <remarks>
/// Each day of the week has its own <see cref="MiniGameDefinition"/>, whose game level is the day
/// (1 = monday, ..., 7 = sunday). From monday to saturday, the event takes place on one of the first
/// three maps, on sunday on the fourth one. The event is entered by talking to Jerint in Devias.
/// </remarks>
internal class ImperialGuardianInitializer : InitializerBase
{
    /// <summary>
    /// The number of the NPC Jerint, at which the event is entered.
    /// </summary>
    internal const short JerintNumber = 522;

    /// <summary>
    /// The days of the event with their map and the coordinates of the entrance of the first zone.
    /// </summary>
    private static readonly (ImperialGuardianDay Day, byte MapNumber, byte EntranceX, byte EntranceY, LocalizedString Name, LocalizedString Description)[] Days =
    [
        (ImperialGuardianDay.Monday, FortressOfImperialGuardian1.Number, 231, 15, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianMonday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianMonday)),
        (ImperialGuardianDay.Tuesday, FortressOfImperialGuardian2.Number, 86, 63, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianTuesday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianTuesday)),
        (ImperialGuardianDay.Wednesday, FortressOfImperialGuardian3.Number, 154, 187, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianWednesday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianWednesday)),
        (ImperialGuardianDay.Thursday, FortressOfImperialGuardian1.Number, 231, 15, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianThursday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianThursday)),
        (ImperialGuardianDay.Friday, FortressOfImperialGuardian2.Number, 86, 63, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianFriday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianFriday)),
        (ImperialGuardianDay.Saturday, FortressOfImperialGuardian3.Number, 154, 187, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianSaturday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianSaturday)),
        (ImperialGuardianDay.Sunday, FortressOfImperialGuardian4.Number, 93, 66, LocalizedString.FromResource(() => MiniGameNames.ImperialGuardianSunday), LocalizedString.FromResource(() => MiniGameDescriptions.ImperialGuardianSunday)),
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ImperialGuardianInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateMiniGameDefinitions();
        this.ConfigureJerint();
    }

    /// <summary>
    /// Creates the mini game definitions of the days, which don't exist yet.
    /// </summary>
    internal void CreateMiniGameDefinitions()
    {
        foreach (var (day, mapNumber, entranceX, entranceY, name, description) in Days)
        {
            var id = GuidHelper.CreateGuid<MiniGameDefinition>((short)MiniGameType.ImperialGuardian, (short)day);
            if (this.GameConfiguration.MiniGameDefinitions.Any(definition => definition.GetId() == id))
            {
                continue;
            }

            var map = this.GameConfiguration.Maps.First(m => m.Number == mapNumber);
            var definition = this.Context.CreateNew<MiniGameDefinition>();
            definition.SetGuid((short)MiniGameType.ImperialGuardian, (short)day);
            this.GameConfiguration.MiniGameDefinitions.Add(definition);
            definition.Name = name;
            definition.Description = description;
            definition.Type = MiniGameType.ImperialGuardian;
            definition.GameLevel = (byte)day;
            definition.EnterDuration = TimeSpan.FromSeconds(60);
            definition.GameDuration = TimeSpan.FromMinutes(50);
            definition.ExitDuration = TimeSpan.FromMinutes(1);
            definition.MaximumPlayerCount = 5;
            definition.MinimumCharacterLevel = 150;
            definition.MaximumCharacterLevel = 400;
            definition.MinimumSpecialCharacterLevel = 150;
            definition.MaximumSpecialCharacterLevel = 400;
            definition.Entrance = map.ExitGates.First(gate => gate.X1 == entranceX && gate.Y1 == entranceY);
            definition.TicketItem = null;
            definition.MapCreationPolicy = MiniGameMapCreationPolicy.OnePerParty;
            definition.SaveRankingStatistics = false;
            definition.AllowParty = true;
        }
    }

    /// <summary>
    /// Lets Jerint open the entrance window of the event.
    /// </summary>
    internal void ConfigureJerint()
    {
        if (this.GameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == JerintNumber) is { } jerint)
        {
            jerint.NpcWindow = NpcWindow.JerintGaionEvententry;
        }
    }
}
