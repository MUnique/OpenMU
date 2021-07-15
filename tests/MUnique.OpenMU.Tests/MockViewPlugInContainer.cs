// <copyright file="MockViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A view plugin container which automatically create mocks for requested view plugins.
    /// </summary>
    public class MockViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
    {
        private readonly Dictionary<Type, IViewPlugIn> mocks = new ();

        /// <inheritdoc />
        public T GetPlugIn<T>()
            where T : class, IViewPlugIn
        {
            if (!this.mocks.TryGetValue(typeof(T), out var mock))
            {
                mock = new Mock<T>().Object;
                this.mocks.Add(typeof(T), mock);
            }

            return (T)mock;
        }
    }
}