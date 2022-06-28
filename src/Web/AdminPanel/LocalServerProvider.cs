// <copyright file="LocalServerProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.ComponentModel;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of a <see cref="IServerProvider"/> which just forwards an available (local) list of <see cref="IManageableServer"/>s.
/// </summary>
public sealed class LocalServerProvider : IServerProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalServerProvider"/> class.
    /// </summary>
    /// <param name="servers">The servers.</param>
    public LocalServerProvider(IList<IManageableServer> servers)
    {
        this.Servers = servers;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public IList<IManageableServer> Servers { get; }
}