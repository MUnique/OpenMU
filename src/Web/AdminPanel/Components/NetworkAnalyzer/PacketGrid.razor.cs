// <copyright file="PacketGrid.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The grid which shows the captured data packets of a connection.
/// </summary>
public partial class PacketGrid : IAsyncDisposable
{
    /// <summary>
    /// The height of a row in pixels. It's required by the virtualization of the grid, which
    /// only renders the rows which are inside the visible area.
    /// </summary>
    private const float RowHeight = 28;

    /// <summary>
    /// The height of the header row in pixels.
    /// </summary>
    private const float HeaderHeight = 40;

    private const string ModulePath = "./_content/MUnique.OpenMU.Web.AdminPanel/Components/NetworkAnalyzer/PacketGrid.razor.js";

    private IJSObjectReference? _module;

    private int _lastPacketCount;

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
    /// Gets or sets the number of rows which are visible at once. The grid is scrollable, so
    /// the other rows are reachable by scrolling.
    /// </summary>
    [Parameter]
    public int VisibleRowCount { get; set; } = 15;

    /// <summary>
    /// Gets or sets a value indicating whether the grid scrolls to the newest packet when new
    /// packets arrive.
    /// </summary>
    [Parameter]
    public bool AutoScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rows should be virtualized, so that only
    /// the visible ones are rendered. It requires javascript to measure the viewport.
    /// </summary>
    [Parameter]
    public bool Virtualize { get; set; } = true;

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

    /// <summary>
    /// Gets or sets the javascript runtime.
    /// </summary>
    [Inject]
    public IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Gets the height of the scrollable area in pixels.
    /// </summary>
    private float GridHeight => (Math.Max(1, this.VisibleRowCount) * RowHeight) + HeaderHeight;

    /// <summary>
    /// Gets or sets the reference to the scrollable element which contains the grid.
    /// </summary>
    private ElementReference ScrollContainer { get; set; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._module is { } module)
        {
            this._module = null;
            try
            {
                await module.DisposeAsync().ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is already gone, so there is nothing to dispose anymore.
            }
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);

        var packetCount = this.Packets.Count;
        if (packetCount == this._lastPacketCount)
        {
            return;
        }

        this._lastPacketCount = packetCount;
        if (!this.AutoScroll || packetCount == 0)
        {
            return;
        }

        try
        {
            this._module ??= await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath).ConfigureAwait(true);
            if (this._module is { } module)
            {
                await module.InvokeVoidAsync("scrollToBottom", this.ScrollContainer).ConfigureAwait(true);
            }
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so we don't need to scroll anymore.
        }
    }

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
