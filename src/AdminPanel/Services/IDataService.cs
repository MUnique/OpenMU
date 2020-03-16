// <copyright file="IDataService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a service which provides data of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the data objects.</typeparam>
    public interface IDataService<T>
    {
        /// <summary>
        /// Gets the data objects of the specified offset in the specified amount.
        /// </summary>
        /// <param name="offset">The offset, which defines how many items of the original data source should be skipped.</param>
        /// <param name="count">The maximum number of items which should be returned.</param>
        /// <returns>The list of available data objects.</returns>
        Task<List<T>> Get(int offset, int count);
    }
}