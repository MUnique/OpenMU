// <copyright file="ConfigurationContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Context to access the game configuration (read-only).
/// </summary>
public class ConfigurationContext : EntityDataContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationContext"/> class.
    /// </summary>
    public ConfigurationContext()
    {
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The model of this context is the same as the one of the <see cref="EntityDataContext"/>,
    /// so we use its compiled model instead of generating another one.
    /// </remarks>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseModel(CompiledModels.ForEntityDataContext.EntityDataContextModel.Instance);
    }
}