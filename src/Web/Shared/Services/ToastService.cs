// <copyright file="ToastService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MUnique.OpenMU.Web.Shared.Components.Toast;

/// <summary>
/// Default implementation of <see cref="IToastService"/>.
/// </summary>
public sealed class ToastService : IToastService, IDisposable
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ClosingDuration = TimeSpan.FromMilliseconds(300);

    private readonly object _lock = new();
    private readonly List<ToastInstance> _toasts = new();
    private readonly List<CancellationTokenSource> _cancellations = new();

    /// <inheritdoc />
    public event Action? StateChanged;

    /// <inheritdoc />
    public IReadOnlyList<ToastInstance> Toasts
    {
        get
        {
            lock (this._lock)
            {
                return this._toasts.ToArray();
            }
        }
    }

    /// <inheritdoc />
    public void ShowSuccess(string message, string? heading = null)
    {
        this.Show(ToastLevel.Success, message, heading);
    }

    /// <inheritdoc />
    public void ShowInfo(string message, string? heading = null)
    {
        this.Show(ToastLevel.Info, message, heading);
    }

    /// <inheritdoc />
    public void ShowWarning(string message, string? heading = null)
    {
        this.Show(ToastLevel.Warning, message, heading);
    }

    /// <inheritdoc />
    public void ShowError(string message, string? heading = null)
    {
        this.Show(ToastLevel.Error, message, heading);
    }

    /// <inheritdoc />
    public void Close(ToastInstance toast)
    {
        this.StartClosing(toast);
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (this._lock)
        {
            foreach (var cts in this._cancellations)
            {
                cts.Cancel();
            }

            this._cancellations.Clear();
            this._toasts.Clear();
        }

        this.StateChanged?.Invoke();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (this._lock)
        {
            foreach (var cts in this._cancellations)
            {
                cts.Dispose();
            }

            this._cancellations.Clear();
            this._toasts.Clear();
        }
    }

    private void Show(ToastLevel level, string message, string? heading)
    {
        var toast = new ToastInstance(level, message, heading);
        var cts = new CancellationTokenSource();
        lock (this._lock)
        {
            this._toasts.Add(toast);
            this._cancellations.Add(cts);
        }

        this.StateChanged?.Invoke();

        _ = this.AutoCloseAsync(toast, cts);
    }

    private async Task AutoCloseAsync(ToastInstance toast, CancellationTokenSource cts)
    {
        try
        {
            await Task.Delay(DefaultDuration, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        this.StartClosing(toast);
    }

    private void StartClosing(ToastInstance toast)
    {
        lock (this._lock)
        {
            var index = this._toasts.IndexOf(toast);
            if (index < 0 || toast.IsClosing)
            {
                return;
            }

            toast.IsClosing = true;
            this._cancellations[index].Cancel();
        }

        this.StateChanged?.Invoke();

        _ = this.FinishClosingAsync(toast);
    }

    private Task FinishClosingAsync(ToastInstance toast)
    {
        return Task.Run(async () =>
        {
            await Task.Delay(ClosingDuration).ConfigureAwait(false);

            lock (this._lock)
            {
                var index = this._toasts.IndexOf(toast);
                if (index < 0)
                {
                    return;
                }

                this._cancellations[index].Dispose();
                this._cancellations.RemoveAt(index);
                this._toasts.RemoveAt(index);
            }

            this.StateChanged?.Invoke();
        });
    }
}