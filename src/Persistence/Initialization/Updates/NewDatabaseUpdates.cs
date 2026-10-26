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
    /// operator decides whether to apply it; so do the ones to apply which depend on it. Their dependencies are checked
    /// when they're applied, not here, so a broken optional plugin doesn't stop the initialization. An update whose
    /// changes the data contains is installed anyway.
    /// </remarks>
    /// <param name="context">The context of the initialization.</param>
    /// <param name="gameConfiguration">The game configuration which was initialized.</param>
    /// <param name="updates">The updates of the initialized version.</param>
    /// <exception cref="MissingUpdateDependencyException">An update to apply depends on one which isn't there, or on a higher version of it.</exception>
    /// <exception cref="CircularUpdateDependencyException">Updates to apply depend on each other in a circle.</exception>
    public static async ValueTask InstallAsync(IContext context, GameConfiguration gameConfiguration, IEnumerable<IConfigurationUpdatePlugIn> updates)
    {
        var all = updates.ToList();
        var pending = DeterminePending(all);

        var initialized = all.Where(update => !update.RunsOnNewDatabase).ToList();
        foreach (var update in initialized)
        {
            var entry = context.CreateNew<ConfigurationUpdate>();
            entry.Key = update.Key;
            entry.Version = update.Version;
            entry.Name = update.Name;
            entry.Description = update.Description;
            entry.CreatedAt = update.CreatedAt;
            entry.UpdatedAt = update.UpdatedAt;
            entry.InstalledAt = DateTime.UtcNow;
        }

        // The updates which are marked count as installed for the dependencies of the ones to apply.
        var installedVersions = initialized
            .GroupBy(update => update.Key)
            .ToDictionary(group => group.Key, group => group.Max(update => update.Version));
        var toApply = all.Where(update => update.RunsOnNewDatabase && !pending.Contains(update.Key)).ToList();
        foreach (var update in DataUpdateService.OrderByDependencies(toApply, installedVersions))
        {
            // It adds its own entry.
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Determines the updates which stay pending: the optional ones which would run on a new database, and the ones
    /// which would run and depend on a pending one.
    /// </summary>
    private static HashSet<Guid> DeterminePending(IReadOnlyList<IConfigurationUpdatePlugIn> updates)
    {
        var pending = updates
            .Where(update => update.RunsOnNewDatabase && !update.IsMandatory)
            .Select(update => update.Key)
            .ToHashSet();
        bool added;
        do
        {
            added = false;
            foreach (var update in updates.Where(update => update.RunsOnNewDatabase && !pending.Contains(update.Key)))
            {
                if (update.DependsOn.Any(dependency => pending.Contains(dependency.Key)))
                {
                    pending.Add(update.Key);
                    added = true;
                }
            }
        }
        while (added);

        return pending;
    }
}
