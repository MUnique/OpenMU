// <copyright file="DevilSquareStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the devil square.
/// </summary>
[PlugIn]
[Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.DevilSquareStartPlugIn_Display1_Name), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.DevilSquareStartPlugIn_Display1_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
[Guid("61C61A58-211E-4D6A-9EA1-D25E0C4A47C5")]
public sealed class DevilSquareStartPlugIn : MiniGameStartBasePlugIn<DevilSquareStartConfiguration, DevilSquareGameServerState>
{
    /// <summary>
    /// Gets the key under which the strategy is getting registered.
    /// </summary>
    public override MiniGameType Key => MiniGameType.DevilSquare;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return DevilSquareStartConfiguration.Default;
    }

    /// <inheritdoc />
    protected override DevilSquareGameServerState CreateState(IGameContext gameContext)
    {
        return new DevilSquareGameServerState(gameContext);
    }
}