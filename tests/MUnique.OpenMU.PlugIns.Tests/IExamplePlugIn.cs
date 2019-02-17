// <copyright file="IExamplePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Example interface for a plugin.
    /// </summary>
    [Guid("34AEED37-9D62-4AE1-9320-91BB620B39C2")]
    [PlugInPoint("Example PlugIn Point", "This plugin point is an example.")]
    public interface IExamplePlugIn
    {
        /// <summary>
        /// Does some stuff.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="text">The text.</param>
        /// <param name="args">The <see cref="MyEventArgs"/> instance containing the event data.</param>
        void DoStuff(Player player, string text, MyEventArgs args);
    }
}