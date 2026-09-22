// <copyright file="AdminPanelContextFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;

using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Design-time factory for <see cref="AdminPanelContext"/>.
/// </summary>
public class AdminPanelContextFactory : IDesignTimeDbContextFactory<AdminPanelContext>
{
    /// <inheritdoc />
    public AdminPanelContext CreateDbContext(string[] args)
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }

        return new AdminPanelContext();
    }
}
