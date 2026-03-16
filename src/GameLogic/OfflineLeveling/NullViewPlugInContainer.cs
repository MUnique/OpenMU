// <copyright file="NullViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A view plugin container that returns null, suppressing all network traffic for players.
/// </summary>
internal sealed class NullViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    /// <inheritdoc/>
    public T? GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        return null;
    }
}