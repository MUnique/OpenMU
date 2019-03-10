// <copyright file="ICustomTestPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A common interface for all plugins managed by the <see cref="CustomTestPlugInContainer"/>.
    /// </summary>
    [CustomPlugInContainer("test custom container interface", "")]
    [Guid("AD127356-FF4D-47EE-9E36-52DB0C2881B6")]
    public interface ICustomTestPlugInContainer
    {
    }
}