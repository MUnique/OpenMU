// <copyright file="AddDuelConfigurationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This updates adds the data for the duel system.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("5DC5638E-581E-4ACC-81E4-D565C625649B")]
public class AddDuelConfigurationPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add duel configuration";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds data for the duel configuration";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddDuelConfiguration;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2024, 07, 11, 20, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.DuelConfiguration is not null)
        {
            return;
        }

        var invisibleEffect = new InvisibleEffectInitializer(context, gameConfiguration);
        invisibleEffect.Initialize();

        var duelMap = gameConfiguration.Maps.First(m => m.Number == 64);
        var mapGates = duelMap.ExitGates.ToDictionary(g => (g.X1, g.Y1), g => g);
        var targetGates = new Dictionary<short, ExitGate>
        {
            { 294, gameConfiguration.Maps.First(m => m.Number == 63).ExitGates.First() },
            { 295, mapGates[(101, 64)] },
            { 296, mapGates[(101, 75)] },
            { 297, mapGates[(101, 113)] },
            { 298, mapGates[(101, 124)] },
            { 299, mapGates[(154, 64)] },
            { 300, mapGates[(154, 75)] },
            { 301, mapGates[(154, 113)] },
            { 302, mapGates[(154, 124)] },
            { 303, mapGates[(100, 70)] },
            { 304, mapGates[(100, 120)] },
            { 305, mapGates[(150, 70)] },
            { 306, mapGates[(150, 120)] },
        };

        gameConfiguration.DuelConfiguration = this.CreateDuelConfiguration(context, targetGates);

        var doorkeeper = gameConfiguration.Monsters.First(m => m.Number == 479);
        doorkeeper.NpcWindow = NpcWindow.DoorkeeperTitusDuelWatch;
    }

    private DuelConfiguration CreateDuelConfiguration(IContext context, IDictionary<short, ExitGate> targetGates)
    {
        var duelConfig = context.CreateNew<DuelConfiguration>();
        duelConfig.MaximumScore = 10;
        duelConfig.MinimumCharacterLevel = 30;
        duelConfig.EntranceFee = 30000;
        duelConfig.Exit = targetGates[294]; // Vulcanus, see above

        List<(short FirstPlayerGate, short SecondPlayerGate, short SpectatorGate)> duelGateNumbers =
        [
            (295, 296, 303),
            (297, 298, 304),
            (299, 300, 305),
            (301, 302, 306),
        ];

        for (short i = 0; i < duelGateNumbers.Count; i++)
        {
            var indices = duelGateNumbers[i];
            var duelArea = context.CreateNew<DuelArea>();
            duelArea.Index = i;
            duelArea.FirstPlayerGate = targetGates[indices.FirstPlayerGate];
            duelArea.SecondPlayerGate = targetGates[indices.SecondPlayerGate];
            duelArea.SpectatorsGate = targetGates[indices.SpectatorGate];
            duelConfig.DuelAreas.Add(duelArea);
        }

        return duelConfig;
    }
}