// <copyright file="IExampleStrategyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Interface for an example strategy plugin.
    /// </summary>
    [Guid("1E68B14C-9156-448A-A6AB-90E423A8E91C")]
    [PlugInPoint("Strategy Plugin Test Interface", "A strategy plugin test interface")]
    public interface IExampleStrategyPlugIn : IStrategyPlugIn<string>
    {
        /// <summary>
        /// Handles the command.
        /// </summary>
        /// <param name="command">The command.</param>
        void HandleCommand(string command);
    }
}
