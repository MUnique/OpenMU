// <copyright file="PacketGrid.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The grid which shows the captured data packets of a connection.
/// </summary>
public partial class PacketGrid
{
    /// <summary>
    /// Gets or sets the packets which should be shown.
    /// </summary>
    [Parameter]
    public IReadOnlyList<Packet> Packets { get; set; } = [];

    /// <summary>
    /// Gets or sets the analyzer which extracts the information of a packet.
    /// </summary>
    [Parameter]
    public PacketAnalyzer? Analyzer { get; set; }

    /// <summary>
    /// Gets or sets the client version which applies to the packets.
    /// </summary>
    [Parameter]
    public ClientVersion ClientVersion { get; set; }

    /// <summary>
    /// Gets or sets the currently selected packet.
    /// </summary>
    [Parameter]
    public Packet? SelectedPacket { get; set; }

    /// <summary>
    /// Gets or sets the callback which is invoked when a packet is selected.
    /// </summary>
    [Parameter]
    public EventCallback<Packet> OnSelect { get; set; }

    private string GetMessage(Packet packet)
    {
        if (this.Analyzer is not { } analyzer)
        {
            return packet.PacketData;
        }

        try
        {
            return analyzer.ExtractShortInformation(packet, this.ClientVersion).Data;
        }
        catch (Exception ex)
        {
            return $"{ex.GetType().Name}: {ex.Message}";
        }
    }
}
