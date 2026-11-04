// <copyright file="CastleSiegeNpcListHandlerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;

/// <summary>
/// Base packet handler for Castle Siege defense-structure lists.
/// </summary>
internal abstract class CastleSiegeNpcListHandlerBase : ISubPacketHandlerPlugIn
{
    private const short SeniorNumber = 223;

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
        // Defense structures are managed in the Senior's window, like the taxes and the tribute.
        if (player.OpenedNpc?.Definition.Number != SeniorNumber)
        {
            return;
        }

        await CastleSiegeNpcListAction.ShowDefenseStructuresAsync(
                player,
                CastleSiegeHandlerContext.Get(player),
                this.MonsterNumber)
            .ConfigureAwait(false);
    }
}
