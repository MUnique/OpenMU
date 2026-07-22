// <copyright file="KanturuStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the periodic start of the Kanturu Refinery Tower event.
/// </summary>
[PlugIn]
[Display(Name = nameof(KanturuStartPlugIn), Description = "Kanturu Refinery Tower event")]
[Guid("A8F3C2D1-9E74-4ECB-8963-08A3697278C4")]
public sealed class KanturuStartPlugIn : MiniGameStartBasePlugIn<KanturuStartConfiguration, KanturuGameServerState>
{
    /// <inheritdoc />
    public override MiniGameType Key => MiniGameType.Kanturu;

    /// <inheritdoc />
    public override object CreateDefaultConfig()
    {
        return KanturuStartConfiguration.Default;
    }

    /// <inheritdoc />
    protected override KanturuGameServerState CreateState(IGameContext gameContext)
    {
        return new KanturuGameServerState(gameContext);
    }
}
