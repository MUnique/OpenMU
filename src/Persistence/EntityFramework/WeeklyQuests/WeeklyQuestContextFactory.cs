// <copyright file="WeeklyQuestContextFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.WeeklyQuests;

using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Design-time factory for <see cref="WeeklyQuestContext"/>.
/// </summary>
public class WeeklyQuestContextFactory : IDesignTimeDbContextFactory<WeeklyQuestContext>
{
    /// <inheritdoc />
    public WeeklyQuestContext CreateDbContext(string[] args)
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }

        return new WeeklyQuestContext();
    }
}
