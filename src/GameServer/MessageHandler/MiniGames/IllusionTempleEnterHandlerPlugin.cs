// <copyright file="IllusionTempleEnterHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.MiniGames;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for illusion temple enter request packets.
/// </summary>
/// <remarks>
/// The packet belongs to the 0xBF group, which is dispatched by the <see cref="MuHelperGroupHandler"/>.
/// Therefore this is a sub packet handler which is selected by the sub code, and not a handler of its own.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.IllusionTempleEnterHandlerPlugIn_Name), Description = nameof(PlugInResources.IllusionTempleEnterHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D4F0076F-86D2-4712-B9FD-6B1C58B11969")]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class IllusionTempleEnterHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <summary>
    /// The game action which contains the logic to enter the mini game.
    /// </summary>
    private readonly EnterMiniGameAction _enterAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => IllusionTempleEnterRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < IllusionTempleEnterRequest.Length
            || player.SelectedCharacter?.CharacterClass is null)
        {
            return;
        }

        IllusionTempleEnterRequest request = packet;
        var definitions = player.GameContext.Configuration.MiniGameDefinitions
            .Where(def => def.Type == MiniGameType.IllusionTemple)
            .ToList();

        // Despite its name, the client sends the number of the temple (1 to 6) here, which corresponds
        // to the game level - not the number of the game map. The lookup by map number is kept as a
        // fallback, in case another client version sends the actual map number (45 to 50).
        var definition = definitions.FirstOrDefault(def => def.GameLevel == request.MapNumber)
                         ?? definitions.FirstOrDefault(def => def.Entrance?.Map?.Number == request.MapNumber);
        var ticketIndex = request.ItemSlot - InventoryConstants.EquippableSlotsCount;

        await this._enterAction.TryEnterMiniGameAsync(
            player,
            MiniGameType.IllusionTemple,
            definition?.GameLevel ?? request.MapNumber,
            (byte)ticketIndex).ConfigureAwait(false);
    }
}