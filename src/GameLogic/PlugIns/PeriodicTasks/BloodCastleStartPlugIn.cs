// <copyright file="BloodCastleStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the blood castle.
/// </summary>
[PlugIn(nameof(BloodCastleStartPlugIn), "Blood Castle event")]
[Guid("95E68C14-AD87-4B3C-AF46-45B8F1C3BC2A")]
public sealed class BloodCastleStartPlugIn : MiniGameStartBasePlugIn<BloodCastleStartConfiguration, BloodCastleGameServerState>
{
    /// <inheritdoc />
    public override MiniGameType Key => MiniGameType.BloodCastle;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return BloodCastleStartConfiguration.Default;
    }

    /// <inheritdoc />
    protected override BloodCastleGameServerState CreateState(IGameContext gameContext)
    {
        return new BloodCastleGameServerState(gameContext);
    }
}