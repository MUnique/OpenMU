// <copyright file="ExamplePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The implementation of the <see cref="IExamplePlugIn"/> which tells us if it got executed.
    /// </summary>
    /// <seealso cref="IExamplePlugIn" />
    [Guid("9FCA692F-2BD5-4310-8755-E20761F94180")]
    [PlugIn(nameof(ExamplePlugIn), "Just an example plugin.")]
    internal class ExamplePlugIn : IExamplePlugIn
    {
        /// <summary>
        /// Gets a value indicating whether the plugin instance was executed in the test.
        /// </summary>
        public bool WasExecuted { get; private set; }

        /// <inheritdoc />
        public void DoStuff(Player player, string text, MyEventArgs args)
        {
            this.WasExecuted = true;
            args.WasExecuted = true;
        }

        /// <summary>
        /// A plugin of a nested type.
        /// </summary>
        /// <seealso cref="IExamplePlugIn" />
        [Guid("B6D7E11D-E99D-4466-BAE1-87B043ED345D")]
        [PlugIn(nameof(NestedPlugIn), "A nested example plugin.")]
        internal class NestedPlugIn : IExamplePlugIn
        {
            /// <inheritdoc/>
            public void DoStuff(Player player, string text, MyEventArgs args)
            {
                // do nothing
            }
        }

        /// <summary>
        /// A plugin of a nested type which doesn't have a guid.
        /// </summary>
        /// <seealso cref="IExamplePlugIn" />
        [PlugIn(nameof(NestedPlugIn), "A nested example plugin without Guid.")]
        internal class NestedWithoutGuid : IExamplePlugIn
        {
            /// <inheritdoc/>
            public void DoStuff(Player player, string text, MyEventArgs args)
            {
                // does nothing, too
            }
        }
    }
}