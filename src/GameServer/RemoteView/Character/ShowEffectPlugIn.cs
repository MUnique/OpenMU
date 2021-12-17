﻿// <copyright file="ShowEffectPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowEffectPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowEffectPlugIn", "The default implementation of the IShowEffectPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("EC433D54-F9CE-40FD-8848-F3515DDD43CF")]
public class ShowEffectPlugIn : IShowEffectPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowEffectPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowEffectPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public void ShowEffect(IIdentifiable target, IShowEffectPlugIn.EffectType effectType)
    {
        if (effectType == IShowEffectPlugIn.EffectType.Swirl)
        {
            this._player.Connection?.SendShowSwirl(target.GetId(this._player));
        }
        else
        {
            this._player.Connection?.SendShowEffect(target.GetId(this._player), Convert(effectType));
        }
    }

    private static ShowEffect.EffectType Convert(IShowEffectPlugIn.EffectType effectType)
    {
        return effectType switch
        {
            IShowEffectPlugIn.EffectType.LevelUp => Network.Packets.ServerToClient.ShowEffect.EffectType.LevelUp,
            IShowEffectPlugIn.EffectType.ShieldLost => Network.Packets.ServerToClient.ShowEffect.EffectType.ShieldLost,
            IShowEffectPlugIn.EffectType.ShieldPotion => Network.Packets.ServerToClient.ShowEffect.EffectType.ShieldPotion,
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), $"Unknown value {effectType}."),
        };
    }
}