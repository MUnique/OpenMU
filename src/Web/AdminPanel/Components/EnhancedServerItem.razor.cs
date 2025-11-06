// <copyright file="EnhancedServerItem.razor.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components;

using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Enhanced server item component with improved UI and functionality.
/// </summary>
public partial class EnhancedServerItem : ComponentBase, IDisposable
{
    private bool _isDeleted;
    private bool _disposed;

    /// <summary>
    /// Gets or sets the server instance.
    /// </summary>
    [Parameter]
    public IManageableServer Server { get; set; } = null!;

    /// <summary>
    /// Gets or sets the delete action.
    /// </summary>
    [Parameter]
    public Action<IManageableServer>? DeleteAction { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (this.Server is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += this.OnServerPropertyChanged;
        }
    }

    /// <summary>
    /// Gets the CSS class for the server card based on its state.
    /// </summary>
    /// <returns>The CSS class name.</returns>
    protected string GetServerCardClass()
    {
        return this.Server.ServerState switch
        {
            ServerState.Started => "server-card-running",
            ServerState.Stopped => "server-card-stopped",
            ServerState.Timeout => "server-card-timeout",
            _ => "server-card-unknown",
        };
    }

    /// <summary>
    /// Gets the server type display text.
    /// </summary>
    /// <returns>The server type display text.</returns>
    protected string GetServerTypeDisplay()
    {
        return this.Server.Type switch
        {
            ServerType.GameServer => "Game Server",
            ServerType.ConnectServer => "Connect Server",
            ServerType.ChatServer => "Chat Server",
            _ => "Unknown Server",
        };
    }

    /// <summary>
    /// Gets the CSS class for the status indicator.
    /// </summary>
    /// <returns>The CSS class name.</returns>
    protected string GetStatusIndicatorClass()
    {
        return this.Server.ServerState switch
        {
            ServerState.Started => "running",
            ServerState.Stopped => "stopped",
            ServerState.Timeout => "timeout",
            _ => "unknown",
        };
    }

    /// <summary>
    /// Gets the CSS class for the status badge.
    /// </summary>
    /// <returns>The CSS class name.</returns>
    protected string GetStatusBadgeClass()
    {
        return this.Server.ServerState switch
        {
            ServerState.Started => "badge-success",
            ServerState.Stopped => "badge-secondary",
            ServerState.Timeout => "badge-warning",
            _ => "badge-light",
        };
    }

    /// <summary>
    /// Gets the CSS class for the progress bar.
    /// </summary>
    /// <returns>The CSS class name.</returns>
    protected string GetProgressBarClass()
    {
        if (this.Server.MaximumConnections <= 0)
        {
            return "bg-info";
        }

        var loadPercentage = (double)this.Server.CurrentConnections / this.Server.MaximumConnections * 100;
        return loadPercentage switch
        {
            >= 90 => "bg-danger",
            >= 70 => "bg-warning",
            >= 50 => "bg-info",
            _ => "bg-success",
        };
    }

    /// <summary>
    /// Gets the state caption for display.
    /// </summary>
    /// <returns>The state caption.</returns>
    protected string GetStateCaption()
    {
        return this.Server.ServerState switch
        {
            ServerState.Started => "Running",
            ServerState.Stopped => "Stopped",
            ServerState.Timeout => "Timeout",
            _ => "Unknown",
        };
    }

    /// <summary>
    /// Handles the start server action.
    /// </summary>
    protected async Task OnStartClickAsync()
    {
        try
        {
            await this.Server.StartAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Handle error - could show toast notification
        }
    }

    /// <summary>
    /// Handles the pause server action.
    /// </summary>
    protected async Task OnPauseClickAsync()
    {
        try
        {
            await this.Server.ShutdownAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Handle error - could show toast notification
        }
    }

    /// <summary>
    /// Handles the stop server action.
    /// </summary>
    protected async Task OnStopClickAsync()
    {
        try
        {
            await this.Server.ShutdownAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Handle error - could show toast notification
        }
    }

    /// <summary>
    /// Handles the delete server action.
    /// </summary>
    protected async Task OnDeleteClickAsync()
    {
        try
        {
            this.DeleteAction?.Invoke(this.Server);
            this._isDeleted = true;
            await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Handle error - could show toast notification
        }
    }

    /// <summary>
    /// Disposes the component resources.
    /// </summary>
    /// <param name="disposing">Whether the component is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed && disposing)
        {
            if (this.Server is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= this.OnServerPropertyChanged;
            }

            this._disposed = true;
        }
    }

    /// <summary>
    /// Handles server property changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnServerPropertyChanged(object? sender, PropertyChangedEventArgs e)
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