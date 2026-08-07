// <copyright file="CastleSiegeNpcListHandlerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Base packet handler for Castle Siege defense-structure lists.
/// </summary>
internal abstract class CastleSiegeNpcListHandlerBase : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public abstract byte Key { get; }

    /// <summary>
    /// Gets the monster number returned by this handler.
    /// </summary>
    protected abstract short MonsterNumber { get; }

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var context = CastleSiegeHandlerContext.Get(player);
        var npcs = context is null
            ? []
            : await context.NpcController.GetDefenseStructureSnapshotAsync(this.MonsterNumber).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcListPlugIn>(
                view => view.ShowNpcListAsync(npcs))
            .ConfigureAwait(false);
    }
}
