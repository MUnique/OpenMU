// <copyright file="IContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The context for repository actions.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Saves the changes of the context.
        /// </summary>
        /// <returns><c>True</c>, if the saving was successful; <c>false</c>, otherwise.</returns>
        bool SaveChanges();

        /// <summary>
        /// Detaches the specified item from the context, if required.
        /// All reachable navigation properties are recursively detached from the context, too.
        /// </summary>
        /// <param name="item">The item which should be detached.</param>
        /// <returns><c>true</c>, if the object was persisted.</returns>
        /// <remarks>
        /// When calling this method, be sure to clear a back reference property before. Otherwise you might detach more than you intended.
        /// </remarks>
        bool Detach(object item);

        /// <summary>
        /// Attaches the specified item to the context in an unmodified state.
        /// All reachable navigations are recursively attached to the context, too.
        /// </summary>
        /// <param name="item">The item which should be attached.</param>
        /// <remarks>
        /// When calling this method, be sure to clear a previous back reference property before. Otherwise you might attach more than you intended.
        /// </remarks>
        void Attach(object item);

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// Attention: This operation needs a currently used context in the current thread!.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        T CreateNew<T>(params object[] args)
            where T : class;

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns><c>True</c>, if successful; Otherwise, false.</returns>
        bool Delete<T>(T obj)
            where T : class;

        /// <summary>
        /// Gets the object of the specified type by its identifier.
        /// </summary>
        /// <typeparam name="T">The type of the requested object.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>The object of the specified type by its identifier.</returns>
        T GetById<T>(Guid id)
            where T : class;

        /// <summary>
        /// Gets all objects of the specified type. Use with caution!.
        /// </summary>
        /// <typeparam name="T">The type of the requested objects.</typeparam>
        /// <returns>All objects of the specified type.</returns>
        IEnumerable<T> Get<T>()
            where T : class;
    }
}