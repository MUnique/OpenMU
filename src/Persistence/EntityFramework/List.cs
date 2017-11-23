// <copyright file="List.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections.Generic;

    /// <summary>
    /// A generic list which implements additional required interfaces for the usage as list and collection in the object model.
    /// </summary>
    /// <typeparam name="T">The type of the items of the list.</typeparam>
    /// <seealso cref="System.Collections.Generic.List{T}" />
    internal class List<T> : System.Collections.Generic.List<T>, ILoadingStatusAwareList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that
        ///  is empty and has the default initial capacity.
        /// </summary>
        public List()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that
        /// is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        public List(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="List{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        public List(IEnumerable<T> collection)
            : base(collection)
        {
            this.LoadingStatus = LoadingStatus.Loaded;
        }

        /// <summary>
        /// Gets or sets the loading status.
        /// </summary>
        /// <value>
        /// The loading status.
        /// </value>
        public LoadingStatus LoadingStatus { get; set; } = LoadingStatus.Unloaded;
    }
}
