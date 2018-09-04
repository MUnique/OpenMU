// <copyright file="ListAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System.Collections.Generic;

    /// <summary>
    /// A collection adapter which adapts a <see cref="ICollection{TEfCore}"/> to an <see cref="ICollection{TClass}"/>,
    /// if <typeparamref name="TEfCore" /> inherits from <typeparamref name="TClass"/>.
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TEfCore">The type of the ef core.</typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{TClass}" />
    public class ListAdapter<TClass, TEfCore> : CollectionAdapter<TClass, TEfCore>, IList<TClass>
        where TEfCore : TClass
    {
        private readonly IList<TEfCore> rawList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListAdapter{TClass, TEfCore}"/> class.
        /// </summary>
        /// <param name="rawList">The raw list.</param>
        public ListAdapter(IList<TEfCore> rawList)
            : base(rawList)
        {
            this.rawList = rawList;
        }

        /// <inheritdoc />
        public TClass this[int index]
        {
            get => this.rawList[index];

            set => this.rawList[index] = (TEfCore)value;
        }

        /// <inheritdoc />
        public int IndexOf(TClass item)
        {
            return this.rawList.IndexOf((TEfCore)item);
        }

        /// <inheritdoc />
        public void Insert(int index, TClass item)
        {
            this.rawList.Insert(index, (TEfCore)item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            this.rawList.RemoveAt(index);
        }
    }
}
