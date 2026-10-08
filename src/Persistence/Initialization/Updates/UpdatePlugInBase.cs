// <copyright file="UpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Abstract base class for a <see cref="IConfigurationUpdatePlugIn"/>.
/// </summary>
public abstract class UpdatePlugInBase : IConfigurationUpdatePlugIn
{
    /// <inheritdoc />
    public Guid Key => this.GetType().GUID;

    /// <inheritdoc />
    public abstract string DataInitializationKey { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract DateTime CreatedAt { get; }

    /// <inheritdoc />
    public abstract bool IsMandatory { get; }

    /// <inheritdoc />
    public virtual int Version => 1;

    /// <inheritdoc />
    public virtual DateTime UpdatedAt => this.CreatedAt;

    /// <inheritdoc />
    public virtual IEnumerable<UpdateDependency> DependsOn => [];

    /// <inheritdoc />
    public async ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var existing = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false))
            .FirstOrDefault(entry => entry.Key == this.Key);
        await this.ApplyAsync(context, gameConfiguration, existing?.Version).ConfigureAwait(false);
        this.AddOrUpdateEntry(context, existing);
    }

    /// <summary>
    /// Applies this update on the given persistence context.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration which can be updated.</param>
    /// <param name="installedVersion">The installed version of this update, or <c>null</c> if it's not installed yet.</param>
    /// <remarks>
    /// Calling <see cref="IContext.SaveChangesAsync"/> is not required in this implementation.
    /// It will be called by <see cref="DataUpdateService"/>.
    /// The default implementation ignores <paramref name="installedVersion"/> and ensures the
    /// final state cumulatively. Override it only to apply a delta between versions.
    /// </remarks>
    protected virtual ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration, int? installedVersion)
    {
        return this.ApplyAsync(context, gameConfiguration);
    }

    /// <summary>
    /// Applies this update on the given persistence context.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration which can be updated.</param>
    /// <remarks>
    /// Calling <see cref="IContext.SaveChangesAsync" /> is not required in this implementation.
    /// It will be called by <see cref="DataUpdateService" />.
    /// </remarks>
    protected abstract ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration);

    /// <summary>
    /// Adds a stat attribute if it does not already exist in the game configuration.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="attribute">The attribute to add.</param>
    /// <returns><c>true</c> if the attribute was added; otherwise, <c>false</c>.</returns>
    protected bool AddStatIfNotExists(IContext context, GameConfiguration gameConfiguration, AttributeDefinition attribute)
    {
        if (gameConfiguration.Attributes.Contains(attribute))
        {
            return false;
        }

        var persistent = context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
        persistent.MaximumValue = attribute.MaximumValue;
        gameConfiguration.Attributes.Add(persistent);
        return true;
    }

    private void AddOrUpdateEntry(IContext context, ConfigurationUpdate? existing)
    {
        var entry = existing ?? context.CreateNew<ConfigurationUpdate>();
        entry.Key = this.Key;
        entry.Version = this.Version;
        entry.Name = this.Name;
        entry.Description = this.Description;
        entry.CreatedAt = this.CreatedAt;
        entry.UpdatedAt = this.UpdatedAt;
        entry.InstalledAt = DateTime.UtcNow;
    }
}