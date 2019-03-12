// <copyright file="IAppearanceSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Serializer of <see cref="IAppearanceData"/> objects.
    /// </summary>
    public interface IAppearanceSerializer : IViewPlugIn
    {
        /// <summary>
        /// Gets the needed space for a serialized <see cref="IAppearanceData"/>.
        /// </summary>
        int NeededSpace { get; }

        /// <summary>
        /// Serializes the appearance data into the target span.
        /// </summary>
        /// <param name="target">The target which should be at least as big as <see cref="NeededSpace"/>.</param>
        /// <param name="appearance">The appearance which should be serialized.</param>
        /// <param name="useCache">If set to <c>true</c>, the result is cached and used in subsequent calls.</param>
        void WriteAppearanceData(Span<byte> target, IAppearanceData appearance, bool useCache);

        /// <summary>
        /// Invalidates the cache for the given appearance.
        /// </summary>
        /// <param name="appearance">The appearance.</param>
        void InvalidateCache(IAppearanceData appearance);
    }
}
