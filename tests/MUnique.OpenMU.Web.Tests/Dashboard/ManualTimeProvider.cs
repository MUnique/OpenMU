// <copyright file="ManualTimeProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.Dashboard;

/// <summary>
/// A <see cref="TimeProvider"/> whose time is set by the test.
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
    /// <summary>
    /// Gets or sets the current time.
    /// </summary>
    public DateTimeOffset UtcNow { get; set; } = new(2026, 10, 18, 12, 0, 0, TimeSpan.Zero);

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow() => this.UtcNow;
}
