// <copyright file="IViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A marker interface for all view plugins, managed by a <see cref="IPlugInContainer{IViewPlugIn}"/>.
    /// </summary>
    [CustomPlugInContainer("View PlugIns", "This is a custom plugin point which collects all view plugins which are suitable for a specific game client version.")]
    public interface IViewPlugIn
    {
    }
}
