// <copyright file="CastleSiegeStateScheduleEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a scheduled transition to a specific <see cref="CastleSiegeState"/> at a given day and time.
/// </summary>
[Cloneable]
public partial class CastleSiegeStateScheduleEntry
{
    /// <summary>
    /// Gets or sets the siege state that becomes active at the scheduled time.
    /// </summary>
    public CastleSiegeState State { get; set; }

    /// <summary>
    /// Gets or sets the day of the week on which this state transition occurs.
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the hour (0–23) at which this state transition occurs.
    /// </summary>
    public byte Hour { get; set; }

    /// <summary>
    /// Gets or sets the minute (0–59) at which this state transition occurs.
    /// </summary>
    public byte Minute { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.State} on {this.DayOfWeek} at {this.Hour:D2}:{this.Minute:D2}";
    }
}
