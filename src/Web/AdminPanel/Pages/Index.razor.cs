// <copyright file="Index.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Code-behind for the Index page with enhanced dashboard functionality.
/// </summary>
public partial class Index : ComponentBase, IDisposable
{
    private IList<IManageableServer>? _servers;
    private bool _disposed;

    /// <summary>
    /// Gets or sets the server provider.
    /// </summary>
    [Inject]
    public IServerProvider? ServerProvider { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync().ConfigureAwait(false);
        await this.LoadServersAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the CSS class for server status indication.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <returns>The CSS class name.</returns>
    protected string GetServerStatusClass(IManageableServer server)
    {
        return server.ServerState switch
        {
            ServerState.Started => "server-running",
            ServerState.Stopped => "server-stopped",
            ServerState.Timeout => "server-timeout",
            _ => "server-unknown",
        };
    }

    /// <summary>
    /// Gets the CSS class for server status badge.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <returns>The badge CSS class name.</returns>
    protected string GetServerStatusBadgeClass(IManageableServer server)
    {
        return server.ServerState switch
        {
            ServerState.Started => "badge badge-success",
            ServerState.Stopped => "badge badge-secondary",
            ServerState.Timeout => "badge badge-warning",
            _ => "badge badge-light",
        };
    }

    /// <summary>
    /// Gets the server status display text.
    /// </summary>
    /// <param name="server">The server.</param>
    /// <returns>The status text.</returns>
    protected string GetServerStatusText(IManageableServer server)
    {
        return server.ServerState switch
        {
            ServerState.Started => "Running",
            ServerState.Stopped => "Stopped",
            ServerState.Timeout => "Timeout",
            _ => "Unknown",
        };
    }

    /// <summary>
    /// Gets the system uptime as a formatted string.
    /// </summary>
    /// <returns>The formatted uptime string.</returns>
    protected string GetSystemUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount);
        if (uptime.Days > 0)
        {
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        }

        if (uptime.Hours > 0)
        {
            return $"{uptime.Hours}h {uptime.Minutes}m";
        }

        return $"{uptime.Minutes}m {uptime.Seconds}s";
    }

    /// <summary>
    /// Disposes the component.
    /// </summary>
    /// <param name="disposing">Whether the component is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed && disposing)
        {
            if (this._servers != null)
            {
                foreach (var server in this._servers.OfType<INotifyPropertyChanged>())
                {
                    server.PropertyChanged -= this.ServerPropertyChanged;
                }
            }

            this._disposed = true;
        }
    }

    /// <summary>
    /// Loads the servers from the server provider.
    /// </summary>
    private async Task LoadServersAsync()
    {
        if (this.ServerProvider != null)
        {
            this._servers = this.ServerProvider.Servers?.ToList();
            if (this._servers != null)
            {
                foreach (var server in this._servers.OfType<INotifyPropertyChanged>())
                {
                    server.PropertyChanged += this.ServerPropertyChanged;
                }
            }
        }

        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles server property changes to update the UI.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private void ServerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            _ = this.InvokeAsync(this.StateHasChanged);
        }
        catch (InvalidOperationException)
        {
            // Component might be disposed, ignore the error
        }
    }
}