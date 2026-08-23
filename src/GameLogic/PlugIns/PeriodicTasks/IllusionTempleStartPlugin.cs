// <copyright file="IllusionTempleStartPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the illusion temple.
/// </summary>
[PlugIn]
[Display(Name = nameof(IllusionTempleStartPlugin), Description = "Illusion Temple event")]
[Guid("3AD96A70-ED24-4979-80B8-169E464E545F")]
public sealed class IllusionTempleStartPlugin : MiniGameStartBasePlugIn<IllusionTempleStartConfiguration, IllusionTempleGameServerState>
{
    /// <inheritdoc />
    public override MiniGameType Key => MiniGameType.IllusionTemple;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return IllusionTempleStartConfiguration.Default;
    }

    /// <inheritdoc />
    protected override IllusionTempleGameServerState CreateState(IGameContext gameContext)
    {
        return new IllusionTempleGameServerState(gameContext);
    }
}