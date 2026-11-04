// <copyright file="MockViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Concurrent;
using Moq;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A view plugin container which automatically create mocks for requested view plugins.
/// </summary>
public class MockViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    // Concurrent, because the game logic may request view plugins of the same player from multiple threads.
    private readonly ConcurrentDictionary<Type, IViewPlugIn> _mocks = new();

    /// <inheritdoc />
    public T GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        return (T)this._mocks.GetOrAdd(typeof(T), _ => new Mock<T>().Object);
    }
}