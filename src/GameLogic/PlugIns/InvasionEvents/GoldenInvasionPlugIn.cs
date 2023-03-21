﻿// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Golden Invasion feature.
/// </summary>
[PlugIn(nameof(GoldenInvasionPlugIn), "Handle Golden Invasion event")]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>, ISupportDefaultCustomConfiguration
{
    private const ushort AtlansId = 7;
    private const ushort TarkanId = 8;

    private const ushort GoldenBudgeDragonId = 43;
    private const ushort GoldenGoblinId = 78;
    private const ushort GoldenSoldierId = 54;
    private const ushort GoldenTitanId = 53;
    private const ushort GoldenDragonId = 79;
    private const ushort GoldenVeparId = 81;
    private const ushort GoldenLizardKingId = 80;
    private const ushort GoldenWheelId = 83;
    private const ushort GoldenTantallosId = 82;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenInvasionPlugIn"/> class.
    /// </summary>
    public GoldenInvasionPlugIn()
        : base(
            MapEventType.GoldenDragonInvasion,
            new (ushort MapId, ushort MonsterId, ushort Count)[]
            {
                (LorenciaId, GoldenBudgeDragonId, 20),
                (NoriaId, GoldenGoblinId, 20),
                (DeviasId, GoldenSoldierId, 20),
                (DeviasId, GoldenTitanId, 10),
                (AtlansId, GoldenVeparId, 20),
                (AtlansId, GoldenLizardKingId, 10),
                (TarkanId, GoldenWheelId, 20),
                (TarkanId, GoldenTantallosId, 10),
            },
            new (ushort MonsterId, ushort Count)[] { (GoldenDragonId, 10) })
    {
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => PeriodicInvasionConfiguration.DefaultGoldenInvasion;
}