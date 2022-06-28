// <copyright file="IDataInitializationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// An interface for a plug in which provides initial data.
/// </summary>
[Guid("A8124680-8562-456C-9952-567E5A40F6D0")]
[PlugInPoint("Data initialization", "Provides an initialization for initial data.")]
public interface IDataInitializationPlugIn : IStrategyPlugIn<string>
{
    /// <summary>
    /// Gets the caption for this initialization plugin which will be visible on the setup page.
    /// </summary>
    string Caption { get; }

    /// <summary>
    /// Creates the initial data for a server.
    /// </summary>
    /// <param name="numberOfGameServers">The number of game servers.</param>
    /// <param name="createTestAccounts">If set to <c>true</c>, test accounts should be created.</param>
    void CreateInitialData(byte numberOfGameServers, bool createTestAccounts);
}