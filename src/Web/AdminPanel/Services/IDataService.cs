// <copyright file="IDataService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Interface for a service which provides data of the specified type.
/// </summary>
/// <typeparam name="T">The type of the data objects.</typeparam>
public interface IDataService<T>
    where T : class
{
    /// <summary>
    /// Gets the data objects of the specified offset in the specified amount.
    /// </summary>
    /// <param name="offset">The offset, which defines how many items of the original data source should be skipped.</param>
    /// <param name="count">The maximum number of items which should be returned.</param>
    /// <returns>The list of available data objects.</returns>
    Task<List<T>> GetAsync(int offset, int count);
}