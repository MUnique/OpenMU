// <copyright file="LuckyCoinRegistrationRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameServer.MessageHandler.MuHelper;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for the request to register a lucky coin at the NPC Delgado.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.LuckyCoinRegistrationRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.LuckyCoinRegistrationRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("14052A86-62E5-4037-9E17-6775A05E2613")]
[MinimumClient(6, 0, ClientLanguage.Invariant)]
[BelongsToGroup(MuHelperGroupHandler.GroupKey)]
internal class LuckyCoinRegistrationRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly LuckyCoinActions _actions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => LuckyCoinRegistrationRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length < LuckyCoinRegistrationRequest.Length)
        {
            return;
        }

        await this._actions.RegisterAsync(player).ConfigureAwait(false);
    }
}
