// <copyright file="ShowItemDropEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ShowItemDropEffectPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowItemDropEffectPlugIn), "The default implementation of the ShowItemDropEffectPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("AA949D5E-0CC6-424E-8412-DE12EA294E33")]
public class ShowItemDropEffectPlugIn : IShowItemDropEffectPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowItemDropEffectPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowItemDropEffectPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void ShowEffect(ItemDropEffect effect, Point targetCoordinates)
    {
        var (x, y) = targetCoordinates;
        switch (effect)
        {
            case ItemDropEffect.Fireworks:
                this._player.Connection?.SendShowFireworks(x, y);
                break;
            case ItemDropEffect.ChristmasFireworks:
                this._player.Connection?.SendShowChristmasFireworks(x, y);
                break;
            case ItemDropEffect.FanfareSound:
                this._player.Connection?.SendPlayFanfareSound(x, y);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effect));
        }
    }
}