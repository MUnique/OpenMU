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
    /// ones whose changes it doesn't contain (<see cref="IConfigurationUpdatePlugIn.RunsOnNewDatabase"/>), in the order
    /// of their dates.
    /// </summary>
    /// <param name="context">The context of the initialization.</param>
    /// <param name="gameConfiguration">The game configuration which was initialized.</param>
    /// <param name="updates">The updates of the initialized version.</param>
    public static async ValueTask InstallAsync(IContext context, GameConfiguration gameConfiguration, IEnumerable<IConfigurationUpdatePlugIn> updates)
    {
        foreach (var update in updates.OrderBy(up => up.UpdatedAt))
        {
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
