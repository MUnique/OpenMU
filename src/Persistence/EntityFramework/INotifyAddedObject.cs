// <copyright file="INotifyAddedObject.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    /// <summary>
    /// Interface which can be implemented by a repository, if it wants to be notified
    /// that an object of its type got added (e.g. because it wants to cache it).
    /// </summary>
    internal interface INotifyAddedObject
    {
        /// <summary>
        /// Is getting called when a new object got added to the database context.
        /// </summary>
        /// <param name="addedObject">The added object.</param>
        void ObjectAdded(object addedObject);
    }
}