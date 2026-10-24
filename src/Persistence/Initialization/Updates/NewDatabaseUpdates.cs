// <copyright file="NewDatabaseUpdates.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Installs the updates of a version on a database which was just initialized.
/// </summary>
internal static class NewDatabaseUpdates
{
    /// <summary>
    /// Marks the updates as installed, because the initialized data contains their changes already, and applies the
    /// mandatory ones whose changes it doesn't contain (<see cref="IConfigurationUpdatePlugIn.RunsOnNewDatabase"/>),
    /// in the order of their dependencies (<see cref="DataUpdateService.OrderByDependencies"/>).
    /// </summary>
    /// <remarks>
    /// An optional update whose changes the data doesn't contain stays pending, as on an existing database, so the
    /// operator decides whether to apply it; so do the ones to apply which depend on it. An update whose changes the
    /// data contains is installed anyway.
    /// </remarks>
    /// <param name="context">The context of the initialization.</param>
    /// <param name="gameConfiguration">The game configuration which was initialized.</param>
    /// <param name="updates">The updates of the initialized version.</param>
    /// <exception cref="MissingUpdateDependencyException">An update depends on one which isn't among them, or on a higher version of it.</exception>
    /// <exception cref="CircularUpdateDependencyException">The updates depend on each other in a circle.</exception>
    public static async ValueTask InstallAsync(IContext context, GameConfiguration gameConfiguration, IEnumerable<IConfigurationUpdatePlugIn> updates)
    {
        // Nothing is installed yet, so every dependency has to be among the updates.
        var ordered = DataUpdateService.OrderByDependencies(updates.ToList(), new Dictionary<Guid, int>());
        var pending = new HashSet<Guid>();
        foreach (var update in ordered)
        {
            if (update.RunsOnNewDatabase
                && (!update.IsMandatory || update.DependsOn.Any(dependency => pending.Contains(dependency.Key))))
            {
                pending.Add(update.Key);
                continue;
            }

            if (update.RunsOnNewDatabase)
            {
                // It adds its own entry.
                await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
                continue;
            }

            var entry = context.CreateNew<ConfigurationUpdate>();
            entry.Key = update.Key;
            entry.Version = update.Version;
            entry.Name = update.Name;
            entry.Description = update.Description;
            entry.CreatedAt = update.CreatedAt;
            entry.UpdatedAt = update.UpdatedAt;
            entry.InstalledAt = DateTime.UtcNow;
        }
    }
}
