// <copyright file="Counter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

/// <summary>
/// Used as C3/C4 Packet Counter, and Skill Count.
/// When the maximum count is getting exceeded,
/// the counter will jump back to the minimum counter value.
/// </summary>
public class Counter
{
    private readonly byte _maxCount;
    private readonly byte _minCount;
    private readonly object _lockObject = new ();
    private int _counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class.
    /// </summary>
    public Counter()
    {
        this._maxCount = 255;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class.
    /// </summary>
    /// <param name="min">The minimum counter value.</param>
    /// <param name="max">The maximum counter value.</param>
    public Counter(byte min, byte max)
    {
        this._minCount = min;
        this._maxCount = max;
    }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>
    /// The count.
    /// </value>
    public int Count
    {
        get
        {
            lock (this._lockObject)
            {
                return this._counter;
            }
        }

        set
        {
            lock (this._lockObject)
            {
                this._counter = value;
            }
        }
    }

    /// <summary>
    /// Increases the counter value.
    /// When the maximum count is getting exceeded,
    /// the counter will jump back to the minimum counter value.
    /// </summary>
    public void Increase()
    {
        lock (this._lockObject)
        {
            if (this._counter == this._maxCount)
            {
                this._counter = this._minCount;
            }
            else
            {
                this._counter++;
            }
        }
    }

    /// <summary>
    /// Resets the count to the minimum value.
    /// </summary>
    public void Reset()
    {
        this.Count = this._minCount;
    }
}