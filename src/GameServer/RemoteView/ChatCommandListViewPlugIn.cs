// <copyright file="ChatCommandListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer.RemoteView.Character;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IChatCommandListViewPlugIn"/> which sends
/// one message per available chat command to the game client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ChatCommandListViewPlugIn_Name), Description = nameof(PlugInResources.ChatCommandListViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6E9E4C1E-9C2A-4C7E-9F5B-0B0A2E2E51D7")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ChatCommandListViewPlugIn : IChatCommandListViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatCommandListViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChatCommandListViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowChatCommandListAsync(IReadOnlyCollection<ChatCommandInfo> commands)
    {
        if (this._player.Connection is not { Connected: true } connection)
        {
            return;
        }

        var index = 0;
        foreach (var command in commands)
        {
            await SendCommandAsync(connection, command, (byte)index, (byte)commands.Count).ConfigureAwait(false);
            index++;
        }
    }

    private static ValueTask SendCommandAsync(IConnection connection, ChatCommandInfo command, byte index, byte count)
    {
        // The client can't show more parameters than fit into one message, and a command
        // with that many parameters wouldn't be usable anyway.
        var parameters = command.Parameters.Take(byte.MaxValue).ToList();

        int Write()
        {
            var size = AvailableChatCommandRef.GetRequiredSize(parameters.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AvailableChatCommandRef(span)
            {
                Index = index,
                Count = count,
                MinimumCharacterStatus = command.MinimumCharacterStatus.Convert(),
                ParameterCount = (byte)parameters.Count,
                Command = command.Command,
                Name = command.Name,
                Description = command.Description,
            };

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var target = packet[i];
                target.IsRequired = parameter.IsRequired;
                target.Type = GetParameterType(parameter.TypeName);
                target.Name = parameter.Name;
                target.ShortName = parameter.ShortName ?? string.Empty;
                target.ValidValues = string.Join('|', parameter.ValidValues);
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    private static ChatCommandParameterType GetParameterType(string typeName)
    {
        return typeName switch
        {
            nameof(Boolean) => ChatCommandParameterType.Boolean,
            nameof(Byte) or nameof(SByte)
                or nameof(Int16) or nameof(UInt16)
                or nameof(Int32) or nameof(UInt32)
                or nameof(Int64) or nameof(UInt64) => ChatCommandParameterType.Number,
            _ => ChatCommandParameterType.Text,
        };
    }
}
