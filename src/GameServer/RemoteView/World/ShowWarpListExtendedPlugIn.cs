// <copyright file="ShowWarpListExtendedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The extended implementation of the <see cref="IShowWarpListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowWarpListExtendedPlugIn_Name), Description = nameof(PlugInResources.ShowWarpListExtendedPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("D679BFFC-2096-40B8-8BD9-1DA7771B9152")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class ShowWarpListExtendedPlugIn : IShowWarpListPlugIn
{
    /// <summary>
    /// The length of the name field of an entry of the WarpListExtended packet.
    /// </summary>
    internal const int NameLength = 32;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowWarpListExtendedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowWarpListExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowWarpListAsync(IReadOnlyCollection<WarpInfo> warps)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        // The count field is one byte. Real warp lists are far below that.
        var entries = warps.Take(byte.MaxValue).ToList();
        var culture = this._player.Culture;

        int Write()
        {
            var size = WarpListExtendedRef.GetRequiredSize(entries.Count);
            var packet = new WarpListExtendedRef(connection.Output.GetSpan(size)[..size])
            {
                Count = (byte)entries.Count,
            };

            for (var i = 0; i < entries.Count; i++)
            {
                var warp = entries[i];
                var entry = packet[i];
                entry.Index = (ushort)warp.Index;
                entry.MapNumber = (ushort)(warp.Gate?.Map?.Number ?? 0);
                entry.LevelRequirement = (ushort)Math.Clamp(warp.LevelRequirement, 0, ushort.MaxValue);
                entry.Costs = (uint)Math.Max(warp.Costs, 0);
                entry.Name = FitIntoNameField(warp.Name.GetTranslation(culture) ?? string.Empty);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <summary>
    /// Shortens the name to the whole characters which fit into the name field as UTF-8.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The name, or its beginning which fits into the name field.</returns>
    internal static string FitIntoNameField(string name)
    {
        if (Encoding.UTF8.GetByteCount(name) <= NameLength)
        {
            return name;
        }

        var result = new StringBuilder();
        var byteCount = 0;
        var elements = StringInfo.GetTextElementEnumerator(name);
        while (elements.MoveNext())
        {
            var element = elements.GetTextElement();
            byteCount += Encoding.UTF8.GetByteCount(element);
            if (byteCount > NameLength)
            {
                break;
            }

            result.Append(element);
        }

        return result.ToString();
    }
}
