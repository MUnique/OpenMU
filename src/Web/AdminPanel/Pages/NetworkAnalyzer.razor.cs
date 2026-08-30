// <copyright file="NetworkAnalyzer.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.Threading;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Properties;
using MUnique.OpenMU.Web.Shared;
using MUnique.OpenMU.Web.Shared.Components.Modal;

/// <summary>
/// The page which shows the network traffic of the connections of our servers.
/// </summary>
public partial class NetworkAnalyzer : IAsyncDisposable
{
    /// <summary>
    /// The route which downloads an archived session, with its identifier appended.
    /// </summary>
    internal const string ArchiveDownloadRoute = "api/network-archive/";

    /// <summary>
    /// The maximum number of packets which are loaded from an archived session. Only the
    /// newest ones are shown when it contains more.
    /// </summary>
    private const int MaximumArchivedPacketCount = 5000;

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

    private IPacketArchive? _archive;

    private IReadOnlyList<ArchivedSessionInfo> _archivedSessions = [];

    private ArchivedSessionInfo? _selectedSession;

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
    /// Gets or sets the identifier of the archived session which should be opened initially.
    /// </summary>
    [Parameter]
    public string? SessionId { get; set; }

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

    /// <summary>
    /// Gets or sets the modal service, used to confirm the deletion of an archived session.
    /// </summary>
    [Inject]
    public IModalService ModalService { get; set; } = null!;

    private IPacketCaptureService? CaptureService => this._captureService;

    private bool IsArchiveAvailable => this._archive is not null;

    /// <summary>
    /// Gets the client version which applies to the shown packets - either the one of the
    /// captured connection, or the one which was recorded with the archived session.
    /// </summary>
    private ClientVersion CurrentClientVersion =>
        this._capture?.ConnectionInfo.ClientVersion ?? this._selectedSession?.Metadata.ClientVersion ?? default;

    private string TableColClass => this._isSidebarCollapsed ? "col-12" : "col-10";

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
        this._archive = this.ServiceProvider.GetService(typeof(IPacketArchive)) as IPacketArchive;
        if (this._captureService is null)
        {
            return;
        }

        _ = await this.RefreshConnectionsAsync().ConfigureAwait(true);
        _ = await this.RefreshArchiveAsync().ConfigureAwait(true);
        if (this.ConnectionId is { } connectionId
            && this._connections.FirstOrDefault(connection => connection.Id == connectionId) is { } preselected)
        {
            await this.OnConnectionSelectedAsync(preselected).ConfigureAwait(true);
        }
        else if (!string.IsNullOrEmpty(this.SessionId)
                 && this._archivedSessions.FirstOrDefault(session => session.Id == this.SessionId) is { } preselectedSession)
        {
            await this.OnArchivedSessionSelectedAsync(preselectedSession).ConfigureAwait(true);
        }

        _ = this.RefreshPeriodicallyAsync();
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

    private static bool HasChanged(IReadOnlyList<ArchivedSessionInfo> current, IReadOnlyList<ArchivedSessionInfo> updated)
    {
        if (current.Count != updated.Count)
        {
            return true;
        }

        for (int i = 0; i < current.Count; i++)
        {
            if (current[i].Id != updated[i].Id
                || current[i].SizeInBytes != updated[i].SizeInBytes
                || current[i].IsRunning != updated[i].IsRunning)
            {
                return true;
            }
        }

        return false;
    }

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

    private async Task OnConnectionSelectedAsync(ICapturedConnectionInfo connection)
    {
        if (this._captureService is not { } captureService || this._selectedConnection?.Id == connection.Id)
        {
            return;
        }

        this.StopCapture();
        this._selectedSession = null;

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

    /// <summary>
    /// Refreshes the list of the archived sessions.
    /// </summary>
    /// <returns><see langword="true"/>, if the listed sessions changed.</returns>
    private async Task<bool> RefreshArchiveAsync()
    {
        if (this._archive is not { } archive)
        {
            return false;
        }

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(true);
        if (!HasChanged(this._archivedSessions, sessions))
        {
            return false;
        }

        this._archivedSessions = sessions;
        return true;
    }

    /// <summary>
    /// Opens an archived session, which stops a running capture of the page.
    /// </summary>
    /// <param name="session">The session which should be shown.</param>
    /// <returns>The async task.</returns>
    private async Task OnArchivedSessionSelectedAsync(ArchivedSessionInfo session)
    {
        this.StopCapture();

        this._selectedSession = session;
        this._selectedPacket = null;
        this._isFollowing = true;
        this._analyzer = this.AnalyzerProvider.GetAnalyzer(PacketDefinitionSet.GameServer);
        await this.LoadArchivedPacketsAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Deletes an archived session, after the user confirmed it.
    /// </summary>
    /// <param name="session">The session which should be deleted.</param>
    /// <returns>The async task.</returns>
    private async Task OnDeleteArchivedSessionAsync(ArchivedSessionInfo session)
    {
        if (this._archive is not { } archive)
        {
            return;
        }

        var isConfirmed = await this.ModalService
            .ShowQuestionAsync(Resources.DeleteArchivedSession, string.Format(Resources.DeleteArchivedSessionQuestion, session.DisplayName))
            .ConfigureAwait(true);
        if (!isConfirmed)
        {
            return;
        }

        if (await archive.DeleteSessionAsync(session.Id).ConfigureAwait(true)
            && this._selectedSession?.Id == session.Id)
        {
            this._selectedSession = null;
            this._packets = [];
            this._selectedPacket = null;
            this.UpdateFilteredPackets();
        }

        _ = await this.RefreshArchiveAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Toggles the observation of the account of the selected connection.
    /// </summary>
    /// <returns>The async task.</returns>
    private async Task ToggleObservationAsync()
    {
        if (this._captureService is not { } captureService || this._capture is not { } capture)
        {
            return;
        }

        await captureService.SetObservationAsync(capture.ConnectionInfo.Id, !capture.ConnectionInfo.IsObserved).ConfigureAwait(true);
        _ = await this.RefreshArchiveAsync().ConfigureAwait(true);
    }

    /// <summary>
    /// Loads the packets of the opened archived session.
    /// </summary>
    /// <returns><see langword="true"/>, if the shown packets changed.</returns>
    private async Task<bool> LoadArchivedPacketsAsync()
    {
        if (this._selectedSession is not { } selectedSession || this._archive is not { } archive)
        {
            return false;
        }

        // A running session grows while it's shown, so its file is read again - the newest
        // packets are the interesting ones, and their number is capped anyway.
        var session = await archive.GetSessionAsync(selectedSession.Id).ConfigureAwait(true) ?? selectedSession;
        var loaded = await ArchivedSession.LoadAsync(session, MaximumArchivedPacketCount).ConfigureAwait(true);
        if (loaded.PacketList.Count == this._packets.Count && this._packets.Count > 0)
        {
            return false;
        }

        this._selectedSession = session;
        this._packets = loaded.PacketList.ToList();
        this.UpdateFilteredPackets();
        return true;
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
        if (this._analyzer is not { } analyzer)
        {
            return string.Empty;
        }

        try
        {
            return analyzer.ExtractShortInformation(packet, this.CurrentClientVersion).Data;
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
                        hasChanged |= await this.RefreshArchiveAsync().ConfigureAwait(true);
                    }

                    hasChanged |= this.UpdatePackets();
                    if (this._selectedSession is { IsRunning: true } && this._isFollowing)
                    {
                        hasChanged |= await this.LoadArchivedPacketsAsync().ConfigureAwait(true);
                    }

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
