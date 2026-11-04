<<<<<<< HEAD
﻿// <copyright file="MockViewPlugInContainer.cs" company="MUnique">
=======
// <copyright file="MockViewPlugInContainer.cs" company="MUnique">
>>>>>>> origin/slave
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
<<<<<<< HEAD
public class MockViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    // Concurrent, because the game logic may request view plugins of the same player from multiple threads.
=======
/// <remarks>
/// The real container (<see cref="CustomPlugInContainerBase{TPlugIn}"/>) keeps its plugins in a
/// <see cref="ConcurrentDictionary{TKey,TValue}"/> and only ever reads from it here, so a player can
/// be asked for the same view plugin from several tasks at once. This one creates its mocks on
/// demand, so it has to be safe for that too - otherwise two threads requesting the same view plugin
/// for the first time both try to add it, and one of them fails.
/// </remarks>
public class MockViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
>>>>>>> origin/slave
    private readonly ConcurrentDictionary<Type, IViewPlugIn> _mocks = new();

    /// <inheritdoc />
    public T GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        return (T)this._mocks.GetOrAdd(typeof(T), _ => new Mock<T>().Object);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> origin/slave
