// <copyright file="UpdateVaultStatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Vault;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Vault;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IUpdateVaultStatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(UpdateVaultStatePlugIn), "The default implementation of the IUpdateVaultStatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("1B0A5F5B-85A0-4228-9392-584E89FB8D88")]
public class UpdateVaultStatePlugIn : IUpdateVaultStatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateVaultStatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public UpdateVaultStatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask UpdateStateAsync()
    {
        await this._player.Connection.SendVaultProtectionInformationAsync(this.GetVaultState()).ConfigureAwait(false);
    }

    private VaultProtectionInformation.VaultProtectionState GetVaultState()
    {
        return this._player.IsVaultLocked switch
        {
            true => VaultProtectionInformation.VaultProtectionState.Locked,
            false when string.IsNullOrWhiteSpace(this._player.Account?.VaultPassword) => VaultProtectionInformation.VaultProtectionState.Unprotected,
            _ => VaultProtectionInformation.VaultProtectionState.Unlocked,
        };
    }
}