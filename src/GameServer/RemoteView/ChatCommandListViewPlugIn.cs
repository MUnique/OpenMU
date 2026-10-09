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
using PacketValueReference = MUnique.OpenMU.Network.Packets.ServerToClient.ChatCommandValueReference;

/// <summary>
/// The default implementation of the <see cref="IChatCommandListViewPlugIn"/> which sends
/// one message per available chat command to the game client. A command with parameters
/// is followed by a message with hints about the values of its parameters.
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

    private static async ValueTask SendCommandAsync(IConnection connection, ChatCommandInfo command, byte index, byte count)
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

        await connection.SendAsync(Write).ConfigureAwait(false);

        if (parameters.Count > 0)
        {
            await SendParameterHintsAsync(connection, parameters, index).ConfigureAwait(false);
        }
    }

    private static ValueTask SendParameterHintsAsync(IConnection connection, List<ChatCommandParameterInfo> parameters, byte index)
    {
        int Write()
        {
            var size = AvailableChatCommandParameterHintsRef.GetRequiredSize(parameters.Count);
            var span = connection.Output.GetSpan(size)[..size];

            // Not every field is written for every parameter, e.g. the range of a text.
            span.Clear();
            var packet = new AvailableChatCommandParameterHintsRef(span)
            {
                Index = index,
                ParameterCount = (byte)parameters.Count,
            };

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var target = packet[i];

                // The numeric values of both enums are the same, and new kinds are only appended to them.
                target.ValueReference = (PacketValueReference)parameter.ValueReference;
                target.GroupWithIndex = GetGroupWithIndex(parameters, parameter);
                if (parameter is { Minimum: { } minimum, Maximum: { } maximum })
                {
                    target.HasRange = true;
                    target.Minimum = unchecked((ulong)minimum);
                    target.Maximum = unchecked((ulong)maximum);
                }
            }

            return size;
        }

        return connection.SendAsync(Write);
    }

    private static byte GetGroupWithIndex(List<ChatCommandParameterInfo> parameters, ChatCommandParameterInfo parameter)
    {
        if (parameter.ValueReferenceGroupWith is not { } groupWith)
        {
            return byte.MaxValue;
        }

        var groupWithIndex = parameters.FindIndex(other => other.Name == groupWith);
        return groupWithIndex is >= 0 and < byte.MaxValue ? (byte)groupWithIndex : byte.MaxValue;
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
