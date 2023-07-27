// <copyright file="UpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Abstract base class for a <see cref="IConfigurationUpdatePlugIn"/>.
/// </summary>
public abstract class UpdatePlugInBase : IConfigurationUpdatePlugIn
{
    /// <inheritdoc />
    public int Key => (int)this.Version;

    /// <inheritdoc />
    public abstract UpdateVersion Version { get; }

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
    public async ValueTask ApplyUpdateAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await this.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);
        this.AddUpdateEntry(context);
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

    private void AddUpdateEntry(IContext context)
    {
        var entry = context.CreateNew<ConfigurationUpdate>();
        entry.Version = (int)this.Version;
        entry.Name = this.Name;
        entry.Description = this.Description;
        entry.CreatedAt = this.CreatedAt;
        entry.InstalledAt = DateTime.UtcNow;
    }
}