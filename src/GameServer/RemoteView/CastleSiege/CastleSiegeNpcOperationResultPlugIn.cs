// <copyright file="CastleSiegeNpcOperationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICastleSiegeNpcOperationResultPlugIn"/>
/// which forwards defense-structure operation results to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeNpcOperationResultPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeNpcOperationResultPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("5B2289ED-05A9-49C0-90C0-33554E043E5F")]
public class CastleSiegeNpcOperationResultPlugIn : ICastleSiegeNpcOperationResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeNpcOperationResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CastleSiegeNpcOperationResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public ValueTask ShowBuyResultAsync(
        CastleSiegeNpcOperationResult result,
        uint npcNumber,
        uint npcIndex)
        => this._player.Connection.SendCastleSiegeDefenseBuyResponseAsync(
            (byte)result,
            npcNumber,
            npcIndex);

    /// <inheritdoc />
    public ValueTask ShowRepairResultAsync(
        CastleSiegeNpcOperationResult result,
        uint npcNumber,
        uint npcIndex,
        int currentHealth,
        int maximumHealth)
        => this._player.Connection.SendCastleSiegeDefenseRepairResponseAsync(
            (byte)result,
            npcNumber,
            npcIndex,
            checked((uint)currentHealth),
            checked((uint)maximumHealth));

    /// <inheritdoc />
    public ValueTask ShowUpgradeResultAsync(
        CastleSiegeNpcOperationResult result,
        uint npcNumber,
        uint npcIndex,
        CastleSiegeUpgradeType upgradeType,
        byte upgradeLevel)
        => this._player.Connection.SendCastleSiegeDefenseUpgradeResponseAsync(
            (byte)result,
            npcNumber,
            npcIndex,
            (uint)upgradeType,
            upgradeLevel);

    /// <inheritdoc />
    public ValueTask ShowGateInterfaceAsync(
        CastleSiegeNpcOperationResult result,
        ushort gateIndex)
        => this._player.Connection.SendCastleSiegeGateInterfaceResponseAsync(
            (byte)result,
            gateIndex);

    /// <inheritdoc />
    public ValueTask ShowGateOperationResultAsync(
        CastleSiegeNpcOperationResult result,
        bool isOpen,
        ushort gateIndex)
        => this._player.Connection.SendCastleSiegeGateOperateResponseAsync(
            (byte)result,
            isOpen,
            gateIndex);

    /// <inheritdoc />
    public ValueTask ShowGateStateAsync(bool isOpen, ushort gateIndex)
        => this._player.Connection.SendCastleSiegeGateStateNotificationAsync(
            isOpen,
            gateIndex);
}
