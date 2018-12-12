// <copyright file="BucketMap{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// A two-dimensional map of buckets.
    /// </summary>
    /// <typeparam name="T">The type of objects which should be hold.</typeparam>
    internal class BucketMap<T>
    {
        private const int DefaultListCapacity = 4;

        private readonly Bucket<T>[] list;

        /// <summary>
        /// Initializes a new instance of the <see cref="BucketMap{T}"/> class.
        /// </summary>
        public BucketMap()
        {
            this.BucketSideLength = 4;
            this.list = new Bucket<T>[0x100 / this.BucketSideLength * 0x100 / this.BucketSideLength];
            this.InitList(DefaultListCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BucketMap{T}"/> class.
        /// </summary>
        /// <param name="sideLength">Length of the side.</param>
        /// <param name="createLists">if set to <c>true</c> the all lists are created now, and not by demand.</param>
        /// <param name="listCapacity">The list capacity.</param>
        /// <param name="bucketSideLength">Length of the bucket side.</param>
        /// <exception cref="System.ArgumentException">SideLength must be a multiple of BucketSideLength.</exception>
        public BucketMap(int sideLength, bool createLists, int listCapacity, int bucketSideLength)
        {
            if (sideLength % bucketSideLength != 0)
            {
                throw new ArgumentException($"SideLength ({sideLength}) must be a multiple of BucketSideLength ({bucketSideLength}).");
            }

            this.BucketSideLength = bucketSideLength;
            this.SideLength = sideLength / bucketSideLength;
            this.list = new Bucket<T>[this.SideLength * this.SideLength];
            if (createLists)
            {
                this.InitList(listCapacity);
            }
        }

        /// <summary>
        /// Gets or sets the length of a side of the map. This value should be a multiple of <see cref="BucketSideLength"/>.
        /// </summary>
        public int SideLength { get; set; }

        /// <summary>
        /// Gets or sets the length of the bucket side.
        /// </summary>
        /// <value>
        /// The length of the bucket side.
        /// </value>
        public int BucketSideLength { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Bucket{T}"/> at the specified coordinates.
        /// </summary>
        /// <value>
        /// The <see cref="Bucket{T}"/>.
        /// </value>
        /// <param name="point">The coordinates.</param>
        /// <returns>The <see cref="Bucket{T}"/> at the specified coordinates.</returns>
        public Bucket<T> this[Point point]
        {
            get => this.list[this.GetListIndex(point)];

            set => this.list[this.GetListIndex(point)] = value;
        }

        /// <summary>
        /// Gets the items which are in range of the specified coordinates and range.
        /// </summary>
        /// <param name="point">The coordinates.</param>
        /// <param name="range">The maximum range.</param>
        /// <returns>The items which are in range of the specified coordinate and range.</returns>
        public IEnumerable<ILocateable> GetInRange(Point point, int range)
        {
            var result = new List<ILocateable>();
            var buckets = this.GetBucketsInRange(point, range);
            foreach (var bucket in buckets)
            {
                result.AddRange(bucket.OfType<ILocateable>().Where(obj => obj.IsInRange(point, range)));
            }

            return result;
        }

        /// <summary>
        /// Gets the buckets in the specified range of the specified coordinates.
        /// </summary>
        /// <param name="point">The coordinates.</param>
        /// <param name="range">The range.</param>
        /// <returns>The buckets in the specified range of the specified coordinate.</returns>
        public IEnumerable<Bucket<T>> GetBucketsInRange(Point point, int range)
        {
            int maxX = Math.Min(point.X + range, (this.SideLength - 1) * this.BucketSideLength) / this.BucketSideLength;
            int maxY = Math.Min(point.Y + range, (this.SideLength - 1) * this.BucketSideLength) / this.BucketSideLength;
            int minX = Math.Max(point.X - range, 0) / this.BucketSideLength;
            int minY = Math.Max(point.Y - range, 0) / this.BucketSideLength;
            for (int i = minX; maxX >= i; ++i)
            {
                for (int j = minY; maxY >= j; ++j)
                {
                    yield return this.list[i + (j * this.SideLength)];
                }
            }
        }

        private int GetListIndex(Point point) => (point.X / this.BucketSideLength) + ((point.Y / this.BucketSideLength) * this.SideLength);

        private void InitList(int listCapacity)
        {
            for (int i = 0; i < this.list.Length; ++i)
            {
                this.list[i] = new Bucket<T>(listCapacity);
            }
        }
    }
}
