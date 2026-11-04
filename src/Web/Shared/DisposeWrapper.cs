// <copyright file="DisposeWrapper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

/// <summary>
/// A wrapper for an <see cref="IDisposable"/> which allows to execute additional code before disposing the target.
/// </summary>
public sealed class DisposeWrapper : IDisposable
{
    private readonly Action _disposeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeWrapper"/> class.
    /// </summary>
    /// <param name="disposeAction">The dispose action.</param>
    public DisposeWrapper(Action disposeAction)
    {
        this._disposeAction = disposeAction;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeAction();
    }
}