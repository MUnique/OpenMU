// <copyright file="LazyExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for <see cref="Lazy{T}"/>.
/// </summary>
public static class LazyExtensions
{
    /// <summary>
    /// Disposes the value of the lazy instance, if it was created, asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IAsyncDisposable"/>.</typeparam>
    /// <param name="lazy">The lazy instance.</param>
    public static async ValueTask DisposeIfCreatedAsync<T>(this Lazy<T> lazy)
        where T : IAsyncDisposable
    {
        if (lazy.IsValueCreated)
        {
            await lazy.Value.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Disposes the value of the lazy instance, if created.
    /// </summary>
    /// <typeparam name="T">The <see cref="IDisposable"/>.</typeparam>
    /// <param name="lazy">The lazy instance.</param>
    public static void DisposeIfCreated<T>(this Lazy<T> lazy)
        where T : IDisposable
    {
        if (lazy.IsValueCreated)
        {
            lazy.Value.Dispose();
        }
    }
}