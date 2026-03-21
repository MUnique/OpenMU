// <copyright file="PathFinderPoolingPolicy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using Microsoft.Extensions.ObjectPool;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The <see cref="PooledObjectPolicy{IPathFinder}"/> which implements the creation
/// of the <see cref="IPathFinder"/> with an <see cref="ScopedGridNetwork"/>.
/// </summary>
public class PathFinderPoolingPolicy : PooledObjectPolicy<IPathFinder>
{
    /// <inheritdoc />
    public override IPathFinder Create()
    {
        return new PathFinder(new ScopedGridNetwork());
    }

    /// <inheritdoc />
    public override bool Return(IPathFinder obj)
    {
        return true;
    }
}