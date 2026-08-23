// <copyright file="MiniGameSpawnGatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which spawns a player of a running mini game at the spot the game assigns to him - for
/// example his team's chamber in the illusion temple - instead of the safezone.
/// </summary>
[PlugIn]
[Display(Name = nameof(MiniGameSpawnGatePlugIn), Description = "Spawns a player of a running mini game at the spot the game assigns to him.")]
[Guid("A3F5C7D9-1B4E-4A28-9C6D-0E8B2F5A7C41")]
public class MiniGameSpawnGatePlugIn : IPlayerSpawnGateSelectionPlugIn
{
    /// <inheritdoc />
    public ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args)
    {
        if (args.Gate is not null)
        {
            return ValueTask.CompletedTask;
        }

        if (player.CurrentMiniGame is { State: MiniGameState.Playing } miniGame
            && miniGame.GetSpawnGate(player) is { } miniGameGate)
        {
            args.Gate = miniGameGate;
        }

        return ValueTask.CompletedTask;
    }
}
