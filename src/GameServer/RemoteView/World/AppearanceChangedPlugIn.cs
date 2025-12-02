// <copyright file="AppearanceChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IAppearanceChangedPlugIn"/> which is forwarding appearance changes of other players to the game client with specific data packets.
/// </summary>
[PlugIn("Appearance changed", "The default implementation of the IAppearanceChangedPlugIn which is forwarding appearance changes of other players to the game client with specific data packets.")]
[Guid("1d097399-d5af-40de-a97d-a812f13c2f20")]
public class AppearanceChangedPlugIn : IAppearanceChangedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppearanceChangedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AppearanceChangedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AppearanceChangedAsync(Player changedPlayer, Item item, bool isEquipped)
    {
        var connection = this._player.Connection;
        if (connection is null || changedPlayer.Inventory is not { } inventory)
        {
            return;
        }

        int Write()
        {
            var itemSerializer = this._player.ItemSerializer;
            var size = AppearanceChanged.GetRequiredSize(itemSerializer.NeededSpace);
            var span = connection!.Output.GetSpan(size)[..size];
            var packet = new AppearanceChangedRef(span)
            {
                ChangedPlayerId = changedPlayer.GetId(this._player),
            };

            if (inventory!.EquippedItems.Contains(item))
            {
                itemSerializer.SerializeItem(packet.ItemData, item);
            }
            else
            {
                packet.ItemData.Fill(0xFF);
            }

            // The byte with index 1 usually now holds the item level and one part of the item option level.
            // This full information is irrelevant. For this message, we just need the "glow" level, which means the one of the appearance serializer.
            // In the available space, the item position is serialized.
            // To summarize: The 4 higher bits hold the item position, the 4 lower bits hold the "glow" level
            packet.ItemData[1] = (byte)(item.ItemSlot << 4);
            packet.ItemData[1] |= item.GetGlowLevel();

            // We could also continue to dumb down information here as this packet reveals all of the options of an item to
            // other players - something which is probably not in interest of the players.
            // However, for now we keep this logic close to the original server, which doesn't do a thing about it.

            // Additionally, we could think of ignoring changes of rings and pendants, as they are usually not visible in the game client, except
            // maybe transformation rings. So we'll leave it as it is, too.
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}

/// <summary>
/// The extended implementation of the <see cref="IAppearanceChangedPlugIn"/> which is forwarding appearance changes of other players to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(AppearanceChangedExtendedPlugIn), "The extended implementation of the IAppearanceChangedPlugIn which is forwarding appearance changes of other players to the game client with specific data packets.")]
[Guid("A2F298E4-9F48-402A-B30D-9BC2BA8DEB2E")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class AppearanceChangedExtendedPlugIn : IAppearanceChangedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppearanceChangedExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AppearanceChangedExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AppearanceChangedAsync(Player changedPlayer, Item item, bool isEquipped)
    {
        var connection = this._player.Connection;
        if (connection is null || changedPlayer.Inventory is null)
        {
            return;
        }

        await connection.SendAppearanceChangedExtendedAsync(
            changedPlayer.GetId(this._player),
            item.ItemSlot,
            (byte)((isEquipped? item.Definition?.Group : 0xFF) ?? 0xFF),
            (ushort)(item.Definition?.Number ?? 0xFFFF),
            item.Level,
            (byte)(ItemSerializerHelper.GetExcellentByte(item) | ItemSerializerHelper.GetFenrirByte(item)),
            (byte)(item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator != 0)?.AncientSetDiscriminator ?? 0),
            changedPlayer.SelectedCharacter?.HasFullAncientSetEquipped() is true)
            .ConfigureAwait(false);
    }
}