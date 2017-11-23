// <copyright file="IAppearanceSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Serializer of <see cref="IAppearanceData"/> objects.
    /// </summary>
    public interface IAppearanceSerializer
    {
        /// <summary>
        /// Gets the serialized appearance data.
        /// </summary>
        /// <param name="appearance">The appearance.</param>
        /// <returns>The serialized appearance data.</returns>
        byte[] GetAppearanceData(IAppearanceData appearance);
    }
}
