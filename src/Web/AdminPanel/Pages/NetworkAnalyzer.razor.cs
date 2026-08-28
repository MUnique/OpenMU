// <copyright file="NetworkAnalyzer.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Network.Analyzer;

/// <summary>
/// The page which shows the network traffic of the connections of our servers.
/// </summary>
public partial class NetworkAnalyzer : IAsyncDisposable
{
    /// <summary>
    /// The interval in which the list of the connections is refreshed.
    /// </summary>
    private static readonly TimeSpan ConnectionRefreshInterval = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The interval in which the view of the captured packets is refreshed. The packets are
    /// captured on the network threads, so the view is updated with a fixed rate instead of
    /// rendering on each packet.
    /// </summary>
    private static readonly TimeSpan PacketRefreshInterval = TimeSpan.FromMilliseconds(250);

    private readonly CancellationTokenSource _disposeCts = new();

    private IReadOnlyList<ICapturedConnectionInfo> _connections = [];

    private ICapturedConnectionInfo? _selectedConnection;

    private ILiveCapturedConnection? _capture;

    private PacketAnalyzer? _analyzer;

    private Packet? _selectedPacket;

    private IReadOnlyList<Packet> _packets = [];

    private IReadOnlyList<Packet> _filteredPackets = [];

    private string? _packetFilter;

    private DirectionFilter _directionFilter = DirectionFilter.All;

    private bool _isFollowing = true;

    private bool _isSidebarCollapsed;

    private IPacketCaptureService? _captureService;

    /// <summary>
    /// The direction of the packets which should be shown.
    /// </summary>
    private enum DirectionFilter
    {
        /// <summary>
        /// All packets are shown.
        /// </summary>
        All,

        /// <summary>
        /// Only the packets which were sent to the server are shown.
        /// </summary>
        ToServer,

        /// <summary>
        /// Only the packets which were sent to the client are shown.
        /// </summary>
        ToClient,
    }

    /// <summary>
    /// Gets or sets the identifier of the connection which should be selected initially.
    /// </summary>
    [Parameter]
    public Guid? ConnectionId { get; set; }

    /// <summary>
    /// Gets or sets the service provider, used to resolve the capture service optionally:
    /// it's only registered in the all-in-one deployment, because it needs the servers in the
    /// same process.
    /// </summary>
    [Inject]
    public IServiceProvider ServiceProvider { get; set; } = null!;

    /// <summary>
    /// Gets or sets the provider of the packet analyzers.
    /// </summary>
    [Inject]
    public PacketAnalyzerProvider AnalyzerProvider { get; set; } = null!;

    private IPacketCaptureService? CaptureService => this._captureService;

    private string TableColClass => this._isSidebarCollapsed ? "col-12" : "col-10";

    private void UpdateFilteredPackets()
    {
        IEnumerable<Packet> packets = this._packets;
        packets = this._directionFilter switch
        {
            DirectionFilter.ToServer => packets.Where(packet => packet.ToServer),
            DirectionFilter.ToClient => packets.Where(packet => !packet.ToServer),
            _ => packets,
        };

        if (!string.IsNullOrWhiteSpace(this._packetFilter))
        {
            var filter = this._packetFilter;
            packets = packets.Where(packet => packet.PacketData.Contains(filter, StringComparison.OrdinalIgnoreCase)
                                              || packet.DisplayCode.ToString("X2").Contains(filter, StringComparison.OrdinalIgnoreCase)
                                              || this.GetMessageForFilter(packet).Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        this._filteredPackets = packets.ToList();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await this._disposeCts.CancelAsync().ConfigureAwait(false);
        this.StopCapture();
        this._disposeCts.Dispose();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        this._captureService = this.ServiceProvider.GetService(typeof(IPacketCaptureService)) as IPacketCaptureService;
        if (this._captureService is null)
        {
            return;
        }

        _ = await this.RefreshConnectionsAsync().ConfigureAwait(true);
        if (this.ConnectionId is { } connectionId
            && this._connections.FirstOrDefault(connection => connection.Id == connectionId) is { } preselected)
        {
            await this.OnConnectionSelectedAsync(preselected).ConfigureAwait(true);
        }

        _ = this.RefreshPeriodicallyAsync();
    }

    /// <summary>
    /// Refreshes the list of the connections.
    /// </summary>
    /// <returns><see langword="true"/>, if the listed connections changed.</returns>
    /// <remarks>
    /// The servers create the information about their connections on each request, so the
    /// list is only replaced when it actually differs - otherwise the sidebar would be
    /// rendered again and again.
    /// </remarks>
    private async Task<bool> RefreshConnectionsAsync()
    {
        if (this._captureService is not { } captureService)
        {
            return false;
        }

        var connections = await captureService.GetConnectionsAsync().ConfigureAwait(true);
        if (!HasChanged(this._connections, connections))
        {
            return false;
        }

        this._connections = connections;
        return true;
    }

    private static bool HasChanged(IReadOnlyList<ICapturedConnectionInfo> current, IReadOnlyList<ICapturedConnectionInfo> updated)
    {
        if (current.Count != updated.Count)
        {
            return true;
        }

        for (int i = 0; i < current.Count; i++)
        {
            if (current[i].Id != updated[i].Id
                || current[i].DisplayName != updated[i].DisplayName)
            {
                return true;
            }
        }

        return false;
    }

    private async Task OnConnectionSelectedAsync(ICapturedConnectionInfo connection)
    {
        if (this._captureService is not { } captureService || this._selectedConnection?.Id == connection.Id)
        {
            return;
        }

        this.StopCapture();

        this._selectedConnection = connection;
        this._capture = await captureService.StartCaptureAsync(connection.Id).ConfigureAwait(true);
        this._analyzer = this.AnalyzerProvider.GetAnalyzer(connection.DefinitionSet);
        this._packets = this._capture?.GetPackets() ?? [];
        this._selectedPacket = null;

        // The grid starts at the newest packet of the new connection, so it follows the
        // traffic again - even when the view of the previous connection was scrolled up.
        this._isFollowing = true;
        this.UpdateFilteredPackets();
    }

    private async Task OnDisconnectAsync(ICapturedConnectionInfo connection)
    {
        await connection.DisconnectAsync().ConfigureAwait(true);
        _ = await this.RefreshConnectionsAsync().ConfigureAwait(true);
    }

    private Task OnClearAsync()
    {
        this._capture?.Clear();
        this._packets = [];
        this._selectedPacket = null;

        // An empty view has nothing to scroll away from, so it follows the traffic again.
        this._isFollowing = true;
        this.UpdateFilteredPackets();
        return Task.CompletedTask;
    }

    private void StopCapture()
    {
        if (this._selectedConnection is { } connection && this._capture is not null)
        {
            this._captureService?.StopCapture(connection.Id);
        }

        this._capture = null;
        this._selectedConnection = null;
        this._packets = [];
        this.UpdateFilteredPackets();
    }

    /// <summary>
    /// Toggles the following of the newest packets. While it's turned off, the view keeps
    /// showing the packets which are currently in it, so that the user can look at them in
    /// peace.
    /// </summary>
    private void ToggleFollowing()
    {
        this._isFollowing = !this._isFollowing;
        if (this._isFollowing)
        {
            this.UpdatePackets();
        }
    }

    /// <summary>
    /// Is called when the packet grid got scrolled to its bottom, or away from it. Scrolling
    /// up is a good indication that the user wants to look at the packets which are currently
    /// shown, so the view stops taking new ones until it's scrolled down again.
    /// </summary>
    /// <param name="isAtBottom">If set to <c>true</c>, the grid is scrolled to its bottom.</param>
    private void OnAtBottomChanged(bool isAtBottom)
    {
        if (this._isFollowing == isAtBottom)
        {
            return;
        }

        this._isFollowing = isAtBottom;
        if (isAtBottom)
        {
            this.UpdatePackets();
        }
    }

    private void OnPacketFilterChanged(string? filter)
    {
        this._packetFilter = filter;
        this.UpdateFilteredPackets();
    }

    private void OnDirectionFilterChanged()
    {
        this.UpdateFilteredPackets();
    }

    private string GetMessageForFilter(Packet packet)
    {
        if (this._analyzer is not { } analyzer || this._capture is not { } capture)
        {
            return string.Empty;
        }

        try
        {
            return analyzer.ExtractShortInformation(packet, capture.ConnectionInfo.ClientVersion).Data;
        }
        catch
        {
            // A packet which can't be analyzed simply doesn't match the filter.
            return string.Empty;
        }
    }

    private async Task RefreshPeriodicallyAsync()
    {
        var token = this._disposeCts.Token;
        using var packetTimer = new PeriodicTimer(PacketRefreshInterval);
        var lastConnectionRefresh = DateTime.UtcNow;

        try
        {
            while (await packetTimer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                var refreshConnections = DateTime.UtcNow - lastConnectionRefresh >= ConnectionRefreshInterval;
                if (refreshConnections)
                {
                    lastConnectionRefresh = DateTime.UtcNow;
                }

                await this.InvokeAsync(async () =>
                {
                    var hasChanged = false;
                    if (refreshConnections)
                    {
                        hasChanged = await this.RefreshConnectionsAsync().ConfigureAwait(true);
                    }

                    hasChanged |= this.UpdatePackets();

                    // Rendering without a change would just make the grid flicker, which is
                    // especially annoying while the user scrolls through the packets.
                    if (hasChanged)
                    {
                        this.StateHasChanged();
                    }
                }).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // The page has been disposed.
        }
    }

    /// <summary>
    /// Takes the newly captured packets, as long as the view follows them.
    /// </summary>
    /// <returns><see langword="true"/>, if the shown packets changed.</returns>
    /// <remarks>
    /// The capture keeps running when the view doesn't follow it, so the missed packets are
    /// simply taken over as soon as it does again.
    /// </remarks>
    private bool UpdatePackets()
    {
        if (!this._isFollowing || this._capture is not { } capture)
        {
            return false;
        }

        var packets = capture.GetPackets();
        if (packets.Count == this._packets.Count)
        {
            return false;
        }

        this._packets = packets;
        this.UpdateFilteredPackets();
        return true;
    }
}
