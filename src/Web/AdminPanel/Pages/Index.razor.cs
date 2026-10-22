// <copyright file="Index.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.AdminPanel.Properties;

/// <summary>
/// The start page of the admin panel, which shows a dashboard with statistics about the server.
/// </summary>
public partial class Index : IDisposable
{
    /// <summary>
    /// The period in which registered accounts are counted as new.
    /// </summary>
    private static readonly TimeSpan NewAccountPeriod = TimeSpan.FromDays(7);

    private IList<IManageableServer> _servers = [];

    private AccountStatistics? _accountStatistics;

    private GuildStatistics? _guildStatistics;

    private bool _isLoadingStatistics = true;

    private bool _isGlobalMessageAvailable;

    [Inject]
    private IServerProvider ServerProvider { get; set; } = null!;

    [Inject]
    private IDataSource<Account> AccountDataSource { get; set; } = null!;

    [Inject]
    private IPersistenceContextProvider PersistenceContextProvider { get; set; } = null!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose()
    {
        this.ServerProvider.PropertyChanged -= this.OnServerProviderPropertyChanged;
        this.UnsubscribeFromServers();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(true);
        this.ServerProvider.PropertyChanged += this.OnServerProviderPropertyChanged;
        this.SubscribeToServers();

        // The game servers are only available in the all-in-one deployment.
        this._isGlobalMessageAvailable = this.ServiceProvider.GetService<IDictionary<int, IGameServer>>() is not null;

        await this.LoadStatisticsAsync().ConfigureAwait(true);
    }

    private static string GetLoadText(IManageableServer server)
    {
        return server.Type == ServerType.GameServer && server.MaximumConnections < int.MaxValue
            ? $"{server.CurrentConnections:N0} / {server.MaximumConnections:N0}"
            : string.Format(Resources.DashboardConnections, server.CurrentConnections.ToString("N0"));
    }

    private static string? GetLoadPercentage(IManageableServer server)
    {
        if (server.MaximumConnections <= 0 || server.MaximumConnections == int.MaxValue)
        {
            return null;
        }

        var percentage = Math.Clamp(100.0 * server.CurrentConnections / server.MaximumConnections, 0, 100);
        return percentage.ToString("0.#", CultureInfo.InvariantCulture);
    }

    private static string GetStateBadgeClass(ServerState state)
    {
        return state switch
        {
            ServerState.Started => "text-bg-success",
            ServerState.Stopped => "text-bg-danger",
            ServerState.Timeout => "text-bg-danger",
            _ => "text-bg-warning",
        };
    }

    private string GetStatisticText(int? value)
    {
        if (value is { } notNull)
        {
            return notNull.ToString("N0");
        }

        return this._isLoadingStatistics ? "…" : Resources.DashboardNotAvailable;
    }

    private string GetGuildDetails()
    {
        var details = new List<string>(2);
        if (this._guildStatistics is { } guilds)
        {
            details.Add(string.Format(Resources.DashboardAlliances, guilds.AllianceCount.ToString("N0")));
        }

        if (this._accountStatistics is { } accounts)
        {
            details.Add(string.Format(Resources.DashboardCharacters, accounts.CharacterCount.ToString("N0")));
        }

        return string.Join(" · ", details);
    }

    private async Task LoadStatisticsAsync()
    {
        try
        {
            var playerContext = (IPlayerContext)await this.AccountDataSource.GetContextAsync().ConfigureAwait(true);
            this._accountStatistics = await playerContext.GetAccountStatisticsAsync(DateTime.UtcNow - NewAccountPeriod).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Couldn't load the account statistics.");
        }

        try
        {
            using var guildContext = this.PersistenceContextProvider.CreateNewGuildContext();
            this._guildStatistics = await guildContext.GetGuildStatisticsAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            this.Logger.LogWarning(ex, "Couldn't load the guild statistics.");
        }

        this._isLoadingStatistics = false;
    }

    private void OnServerProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IServerProvider.Servers))
        {
            return;
        }

        this.UnsubscribeFromServers();
        this.SubscribeToServers();
        _ = this.InvokeAsync(this.StateHasChanged);
    }

    private void OnServerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = this.InvokeAsync(this.StateHasChanged);
    }

    private void SubscribeToServers()
    {
        this._servers = this.ServerProvider.Servers;
        foreach (var server in this._servers)
        {
            server.PropertyChanged += this.OnServerPropertyChanged;
        }
    }

    private void UnsubscribeFromServers()
    {
        foreach (var server in this._servers)
        {
            server.PropertyChanged -= this.OnServerPropertyChanged;
        }
    }
}
