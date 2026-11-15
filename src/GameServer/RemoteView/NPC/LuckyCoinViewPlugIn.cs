// <copyright file="LuckyCoinViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.NPC;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using LuckyCoinExchangeResult = MUnique.OpenMU.GameLogic.Views.NPC.LuckyCoinExchangeResult;
using LuckyCoinRegistrationResult = MUnique.OpenMU.GameLogic.Views.NPC.LuckyCoinRegistrationResult;

/// <summary>
/// The default implementation of the <see cref="ILuckyCoinViewPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LuckyCoinViewPlugIn_Name), Description = nameof(PlugInResources.LuckyCoinViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("E7D90509-EBD1-4685-8C5A-015316C2AF4B")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
public class LuckyCoinViewPlugIn : ILuckyCoinViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="LuckyCoinViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public LuckyCoinViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowRegisteredCountAsync(int count)
    {
        await this._player.Connection.SendLuckyCoinCountResponseAsync((uint)Math.Max(count, 0)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowRegistrationResultAsync(LuckyCoinRegistrationResult result, int count)
    {
        var packetResult = result == LuckyCoinRegistrationResult.Registered
            ? LuckyCoinRegistrationResponse.LuckyCoinRegistrationResult.Registered
            : LuckyCoinRegistrationResponse.LuckyCoinRegistrationResult.NotEnoughItems;
        await this._player.Connection.SendLuckyCoinRegistrationResponseAsync(packetResult, (uint)Math.Max(count, 0)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowExchangeResultAsync(LuckyCoinExchangeResult result)
    {
        var packetResult = result switch
        {
            LuckyCoinExchangeResult.Exchanged => LuckyCoinExchangeResponse.LuckyCoinExchangeResult.Exchanged,
            LuckyCoinExchangeResult.InventoryFull => LuckyCoinExchangeResponse.LuckyCoinExchangeResult.InventoryFull,
            _ => LuckyCoinExchangeResponse.LuckyCoinExchangeResult.NotEnoughItems,
        };
        await this._player.Connection.SendLuckyCoinExchangeResponseAsync(packetResult).ConfigureAwait(false);
    }
}
