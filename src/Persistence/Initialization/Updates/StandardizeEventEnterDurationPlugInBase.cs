// <copyright file="StandardizeEventEnterDurationPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// This update raises the entrance duration of all events with a shared entrance to the five minutes which every event entrance announces.
/// </summary>
/// <remarks>
/// Only seeded durations are changed, so owner tweaks are preserved. Events without a shared map are created per party and have no entrance announcement.
/// </remarks>
public abstract class StandardizeEventEnterDurationPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Standardize event entrance duration";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update raises the entrance duration of the events with a shared entrance, like Blood Castle, Devil Square and Kanturu, to the five minutes which every event entrance announces.";

    /// <summary>
    /// The entrance durations which the initializers seeded before the standardization.
    /// </summary>
    private static readonly TimeSpan[] SeededEnterDurations =
    [
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(3),
    ];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 19, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var definition in gameConfiguration.MiniGameDefinitions
            .Where(definition => definition.MapCreationPolicy == MiniGameMapCreationPolicy.Shared)
            .Where(definition => SeededEnterDurations.Contains(definition.EnterDuration)))
        {
            definition.EnterDuration = MiniGameConstants.EnterDuration;
        }

        return ValueTask.CompletedTask;
    }
}
