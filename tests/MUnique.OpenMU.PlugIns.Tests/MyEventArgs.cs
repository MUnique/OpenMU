// <copyright file="MyEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.ComponentModel;

    /// <summary>
    /// Event args for <see cref="IExamplePlugIn.DoStuff"/> which tell us, if the plugin got executed.
    /// </summary>
    /// <seealso cref="System.ComponentModel.CancelEventArgs" />
    public class MyEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin instance was executed in the test.
        /// </summary>
        public bool WasExecuted { get; set; }

        /// <summary>
        /// Gets or sets a text.
        /// </summary>
        public string Text { get; set; }
    }
}