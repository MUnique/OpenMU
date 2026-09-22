// <copyright file="PacketDetail.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Shows the content of a selected data packet.
/// </summary>
public partial class PacketDetail
{
    /// <summary>
    /// Gets or sets the packet whose content should be shown.
    /// </summary>
    [Parameter]
    public Packet? Packet { get; set; }

    /// <summary>
    /// Gets or sets the analyzer which extracts the information of the packet.
    /// </summary>
    [Parameter]
    public PacketAnalyzer? Analyzer { get; set; }

    /// <summary>
    /// Gets or sets the client version which applies to the packet.
    /// </summary>
    [Parameter]
    public ClientVersion ClientVersion { get; set; }

    private string ExtractedInformation
    {
        get
        {
            if (this.Packet is not { } packet || this.Analyzer is not { } analyzer)
            {
                return string.Empty;
            }

            try
            {
                return analyzer.ExtractInformation(packet, this.ClientVersion);
            }
            catch (Exception ex)
            {
                return $"{ex.GetType().Name}: {ex.Message}";
            }
        }
    }
}
