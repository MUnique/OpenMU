// <copyright file="LoadingOverlayService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// A service which provides a loading overlay functionality.
/// </summary>
public sealed class LoadingOverlayService
{
    private int _count;

    /// <summary>
    /// Occurs when the visibility state of this overlay changed.
    /// </summary>
    public event Action? StateChanged;

    /// <summary>
    /// Gets a value indicating whether the loading overlay is visible.
    /// </summary>
    public bool IsVisible => this._count > 0;

    /// <summary>
    /// Shows the loading overlay.
    /// </summary>
    /// <returns>A disposable which hides the overlay when being disposed.</returns>
    public IDisposable ShowLoadingIndicator()
    {
        this._count++;
        this.StateChanged?.Invoke();
        return new DisposeAction(() =>
        {
            this._count = Math.Max(0, this._count - 1);
            this.StateChanged?.Invoke();
        });
    }

    private sealed class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction(Action action)
        {
            this._action = action;
        }

        public void Dispose()
        {
            this._action();
        }
    }
}
