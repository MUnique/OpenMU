// <copyright file="KanturuStartConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// The Kanturu event start configuration.
/// </summary>
public class KanturuStartConfiguration : MiniGameStartConfiguration
{
    /// <summary>
    /// Gets the default configuration for the Kanturu event.
    /// The event runs every 6 hours: each tick opens a silent entry lobby while no
    /// fight runs and the tower is closed. Entry is additionally always open on demand,
    /// and the tower stays open for <see cref="TowerOpenDuration"/> after Nightmare dies.
    /// There are no entrance announcements; the gateway dialog shows the live state.
    /// </summary>
    public static KanturuStartConfiguration Default =>
        new()
        {
            PreStartMessageDelay = TimeSpan.Zero,
            EntranceOpenedMessage = "Kanturu Refinery Tower entrance is open and closes in {0} minute(s).",
            EntranceClosedMessage = "Kanturu Refinery Tower entrance closed.",
            TaskDuration = TimeSpan.FromHours(6),
            Timetable = GenerateTimeSequence(TimeSpan.FromHours(6)).ToList(),
            TowerOpenDuration = TimeSpan.FromHours(23),
        };

    /// <summary>
    /// Gets or sets how long the Tower of Refinement stays open after the Nightmare boss
    /// has been defeated. The window is tracked persistently, so it survives server
    /// restarts: players can still re-enter the tower while it lasts.
    /// </summary>
    /// <remarks>
    /// This takes precedence over <see cref="KanturuEventDefinition.TowerOfRefinementDuration"/>
    /// whenever the event runs through this plug-in.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.KanturuStartConfiguration_TowerOpenDuration_Name), Description = nameof(PlugInResources.KanturuStartConfiguration_TowerOpenDuration_Description), Order = 6)]
    public TimeSpan TowerOpenDuration { get; set; } = TimeSpan.FromHours(23);

    /// <summary>
    /// Gets or sets the UTC time until which the Tower of Refinement is open.
    /// It's set when the Nightmare boss is defeated and cleared when the tower closes
    /// or a new event run starts. Persisted with the configuration, so the open window
    /// survives server restarts.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.KanturuStartConfiguration_TowerOpenUntilUtc_Name), Description = nameof(PlugInResources.KanturuStartConfiguration_TowerOpenUntilUtc_Description), Order = 7)]
    public DateTime? TowerOpenUntilUtc { get; set; }

    /// <summary>
    /// Gets or sets the definition of the event run itself: its phases, the monsters which
    /// have to be killed in each of them, the boss fight and the Tower of Refinement.
    /// </summary>
    /// <remarks>
    /// It's <see langword="null"/> until it's either seeded by the data initialization or
    /// filled in by an administrator, because the monsters can only be referenced when the
    /// game configuration is known. The <see cref="MiniGames.Kanturu.KanturuContext"/> falls
    /// back to <see cref="KanturuEventDefinition.CreateDefault"/> in that case.
    /// </remarks>
    public KanturuEventDefinition? EventDefinition { get; set; }
}
