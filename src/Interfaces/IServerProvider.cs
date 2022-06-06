// <copyright file="IServerProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.ComponentModel;

/// <summary>
/// An interface for an object which provides a list of <see cref="IManageableServer"/>s.
/// </summary>
public interface IServerProvider : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the servers.
    /// </summary>
    IList<IManageableServer> Servers { get; }
}