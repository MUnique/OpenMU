// <copyright file="AsyncDisposable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Base class for classes which want to implement <see cref="IAsyncDisposable"/>.
/// </summary>
public class AsyncDisposable : Disposable, IAsyncDisposable
{
    private bool _isDisposing;

    /// <summary>
    /// Gets a value indicating whether this instance is disposing.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposing; otherwise, <c>false</c>.
    /// </value>
    public new bool IsDisposing
    {
        get => this._isDisposing || base.IsDisposing;
        private set => this._isDisposing = value;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!this.IsDisposed && !this.IsDisposing)
        {
            this.IsDisposing = true;
            try
            {
                await this.DisposeAsyncCore().ConfigureAwait(false);
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            finally
            {
                this.IsDisposing = false;
            }
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "This naming is suggested by microsoft itself.")]
    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }
}