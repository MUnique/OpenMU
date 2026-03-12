// <copyright file="NullViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Reflection;

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A view plugin container that returns proxy objects which do nothing,
/// effectively suppressing all network traffic for the offline leveling ghost player.
/// </summary>
internal sealed class NullViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    /// <inheritdoc/>
    public T? GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        return DispatchProxy.Create<T, NullViewProxy>();
    }
}