// <copyright file="PacketGrid.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

using System.Runtime.CompilerServices;
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
    /// The height of a row in pixels. It's used to calculate the height of the scrollable
    /// area for the requested number of visible rows.
    /// </summary>
    private const float RowHeight = 28;

    /// <summary>
    /// The height of the header row in pixels.
    /// </summary>
    private const float HeaderHeight = 40;

    private const string ModulePath = "./_content/MUnique.OpenMU.Web.AdminPanel/Components/NetworkAnalyzer/PacketGrid.razor.js";

    private IJSObjectReference? _module;

    private DotNetObjectReference<PacketGrid>? _componentReference;

    private bool _isObserving;

    private int _lastPacketCount;

    private bool _wasAutoScrolling;

    private IReadOnlyList<Packet> _visiblePackets = [];

    private IReadOnlyList<Packet>? _packetsOfVisiblePackets;

    private ClientVersion _versionOfMessages;

    private PacketAnalyzer? _analyzerOfMessages;

    private ConditionalWeakTable<Packet, string> _messages = new();

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
    /// Gets or sets the maximum number of rows which are rendered. Only the newest packets
    /// are shown when there are more of them.
    /// </summary>
    /// <remarks>
    /// The grid isn't virtualized: appending rows below the visible area doesn't move the
    /// scroll position, while a virtualized grid has to shift its content around when the
    /// number of items changes - which makes it jump while the traffic arrives. Instead, the
    /// number of rendered rows is limited, so that the browser and the diffing of the
    /// rendering don't have to deal with the whole capture.
    /// </remarks>
    [Parameter]
    public int MaximumRowCount { get; set; } = 500;

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
    /// Gets or sets the callback which is invoked when the view got scrolled to the bottom,
    /// or away from it.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnAtBottomChanged { get; set; }

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
        this._componentReference?.Dispose();
        this._componentReference = null;

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

    /// <summary>
    /// Is called by the javascript module when the view got scrolled to the bottom, or away
    /// from it.
    /// </summary>
    /// <param name="isAtBottom">If set to <c>true</c>, the view is scrolled to the bottom.</param>
    /// <returns>The async task.</returns>
    [JSInvokable]
    public async Task SetAtBottomAsync(bool isAtBottom)
    {
        await this.OnAtBottomChanged.InvokeAsync(isAtBottom).ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this._versionOfMessages != this.ClientVersion || !ReferenceEquals(this._analyzerOfMessages, this.Analyzer))
        {
            // The extracted messages depend on the analyzer and the client version, so they
            // can't be reused when one of them changed.
            this._versionOfMessages = this.ClientVersion;
            this._analyzerOfMessages = this.Analyzer;
            this._messages = new ConditionalWeakTable<Packet, string>();
        }

        // The visible packets are only determined when the packets actually changed. Doing it
        // on each render would make the grid treat it as a new data source.
        if (!ReferenceEquals(this._packetsOfVisiblePackets, this.Packets))
        {
            this._packetsOfVisiblePackets = this.Packets;
            this._visiblePackets = this.Packets.Count > this.MaximumRowCount
                ? this.Packets.Skip(this.Packets.Count - this.MaximumRowCount).ToList()
                : this.Packets;
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(true);

        var packetCount = this._visiblePackets.Count;
        var hasNewPackets = packetCount != this._lastPacketCount;
        this._lastPacketCount = packetCount;

        // When the auto scrolling is switched on again, the view may have been left somewhere
        // in the middle of the packets, so it's brought back to the newest one.
        var startedScrolling = this.AutoScroll && !this._wasAutoScrolling;
        this._wasAutoScrolling = this.AutoScroll;

        var shouldScroll = this.AutoScroll && (hasNewPackets || startedScrolling);
        if (packetCount == 0)
        {
            // The scrollable element isn't rendered without packets, so a new one has to be
            // observed as soon as they arrive again.
            this._isObserving = false;
            return;
        }

        if (this._isObserving && !shouldScroll)
        {
            return;
        }

        try
        {
            this._module ??= await this.JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath).ConfigureAwait(true);
            if (this._module is not { } module)
            {
                return;
            }

            if (!this._isObserving)
            {
                this._isObserving = true;
                this._componentReference ??= DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("observe", this.ScrollContainer, this._componentReference).ConfigureAwait(true);
            }

            if (shouldScroll)
            {
                await module.InvokeVoidAsync("scrollToBottom", this.ScrollContainer).ConfigureAwait(true);
            }
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so there is nothing to observe or to scroll anymore.
        }
    }

    /// <summary>
    /// Gets the message of the packet, which is extracted by the analyzer.
    /// </summary>
    /// <param name="packet">The packet.</param>
    /// <returns>The message of the packet.</returns>
    /// <remarks>
    /// The extraction is not for free and a row is rendered again with each arriving packet,
    /// so the message of a packet is only extracted once.
    /// </remarks>
    private string GetMessage(Packet packet)
    {
        if (this._messages.TryGetValue(packet, out var message))
        {
            return message;
        }

        message = this.ExtractMessage(packet);
        this._messages.AddOrUpdate(packet, message);
        return message;
    }

    private string ExtractMessage(Packet packet)
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
