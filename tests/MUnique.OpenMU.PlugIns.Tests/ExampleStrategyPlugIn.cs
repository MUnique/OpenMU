// <copyright file="ExampleStrategyPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A test strategy plugin.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.PlugIns.Tests.IExampleStrategyPlugIn" />
    [Guid("69A6FCD1-E828-4841-BE91-E064231ED7B9")]
    [PlugIn(nameof(ExampleStrategyPlugIn), "A test strategy plugin.")]
    public class ExampleStrategyPlugIn : IExampleStrategyPlugIn
    {
        /// <summary>
        /// Gets the command key which is handled by this strategy plugin type.
        /// </summary>
        public static string CommandKey => "/mytest";

        /// <inheritdoc />
        public string Key => CommandKey;

        /// <summary>
        /// Gets the handled command.
        /// </summary>
        /// <value>
        /// The handled command.
        /// </value>
        public string HandledCommand { get; private set; }

        /// <inheritdoc/>
        public void HandleCommand(string command)
        {
            this.HandledCommand = command;
        }
    }
}