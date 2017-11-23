// <copyright file="BucketMap{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The type of the range calculation type.
    /// </summary>
    public enum RangeType
    {
        /// <summary>
        /// An object is in range, when its in the quadrat of (x-range, y-range) and (x+range, y+range).
        /// </summary>
        Quadratic,

        /// <summary>
        /// An object is in range, when it is exactly in range (a²+b²=c²).
        /// </summary>
        Exact
    }

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
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The <see cref="Bucket{T}"/> at the specified coordinates.</returns>
        public Bucket<T> this[int x, int y]
        {
            get
            {
                return this.list[(x / this.BucketSideLength) + ((y / this.BucketSideLength) * this.SideLength)];
            }

            set
            {
                this.list[(x / this.BucketSideLength) + (y / this.BucketSideLength * this.SideLength)] = value;
            }
        }

        /// <summary>
        /// Moves the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="xOld">The old x coordinate.</param>
        /// <param name="yOld">The old y coordinate.</param>
        /// <param name="xNew">The new x coordinate.</param>
        /// <param name="yNew">The new y coordinate.</param>
        public void Move(T element, int xOld, int yOld, int xNew, int yNew)
        {
            if (xOld / this.BucketSideLength == xNew / this.BucketSideLength && yNew / this.BucketSideLength == yOld / this.BucketSideLength)
            {
                return;
            }

            this[xOld, yOld].Remove(element);
            this[xNew, yNew].Add(element);
        }

        /// <summary>
        /// Gets the items which are in range of the specified coordinate and range.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="range">The maximum range.</param>
        /// <param name="rangeType">Type of the range.</param>
        /// <returns>The items which are in range of the specified coordinate and range.</returns>
        public IEnumerable<T> GetInRange(int x, int y, int range, RangeType rangeType)
        {
            var retList = new List<T>();

            int maxX = Math.Min(x + range, this.SideLength * this.BucketSideLength) / this.BucketSideLength;
            int maxY = Math.Min(y + range, this.SideLength * this.BucketSideLength) / this.BucketSideLength;
            int minX = Math.Max(x - range, 0) / this.BucketSideLength;
            int minY = Math.Max(y - range, 0) / this.BucketSideLength;
            for (int i = minX; maxX > i; ++i)
            {
                for (int j = minY; maxY > j; ++j)
                {
                    if (rangeType == RangeType.Quadratic)
                    {
                        retList.AddRange(this.list[i + (j * this.SideLength)]);
                    }
                    else
                    {
                        if (IsInRangeExact(i, j, x / this.BucketSideLength, y / this.BucketSideLength, range))
                        {
                            retList.AddRange(this.list[i + (j * this.SideLength)]);
                        }
                    }
                }
            }

            return retList;
        }

        /// <summary>
        /// Gets the buckets in the specified range of the specified coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="range">The range.</param>
        /// <param name="rangeType">Type of the range.</param>
        /// <returns>The buckets in the specified range of the specified coordinate.</returns>
        public IEnumerable<Bucket<T>> GetBucketsInRange(int x, int y, int range, RangeType rangeType)
        {
            int maxX = Math.Min(x + range, this.SideLength * this.BucketSideLength) / this.BucketSideLength;
            int maxY = Math.Min(y + range, this.SideLength * this.BucketSideLength) / this.BucketSideLength;
            int minX = Math.Max(x - range, 0) / this.BucketSideLength;
            int minY = Math.Max(y - range, 0) / this.BucketSideLength;
            for (int i = minX; maxX > i; ++i)
            {
                for (int j = minY; maxY > j; ++j)
                {
                    if (rangeType == RangeType.Quadratic)
                    {
                        yield return this.list[i + (j * this.SideLength)];
                    }
                    else
                    {
                        if (IsInRangeExact(i, j, x / this.BucketSideLength, y / this.BucketSideLength, range))
                        {
                            yield return this.list[i + (j * this.SideLength)];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the points x1/y1 and x2/y2 are in range.
        /// </summary>
        /// <param name="x1">The x coordinate of the first position.</param>
        /// <param name="y1">The y coordinate of the first position.</param>
        /// <param name="x2">The x coordinate of the second position.</param>
        /// <param name="y2">The y coordinate of the second position.</param>
        /// <param name="maxrange">The maximum range.</param>
        /// <returns>True, if the points are in range.</returns>
        private static bool IsInRangeExact(int x1, int y1, int x2, int y2, double maxrange)
        {
            int xdiff;
            int ydiff;
            if (x1 < x2)
            {
                xdiff = x2 - x1;
            }
            else if (x1 > x2)
            {
                xdiff = x1 - x2;
            }
            else
            {
                xdiff = 0;
            }

            if (y1 < y2)
            {
                ydiff = y2 - y1;
            }
            else if (y1 > y2)
            {
                ydiff = y1 - y2;
            }
            else
            {
                ydiff = 0;
            }

            xdiff *= xdiff;
            ydiff *= ydiff;
            double distance = Math.Sqrt(xdiff + ydiff);
            return distance <= maxrange;
        }

        private void InitList(int listCapacity)
        {
            for (int i = 0; i < this.list.Length; ++i)
            {
                this.list[i] = new Bucket<T>(listCapacity);
            }
        }
    }
}
