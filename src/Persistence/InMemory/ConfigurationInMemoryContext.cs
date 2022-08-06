// <copyright file="ConfigurationInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using MUnique.OpenMU.Persistence.BasicModel;

public class ConfigurationInMemoryContext : InMemoryContext, IConfigurationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationInMemoryContext"/> class.
    /// </summary>
    /// <param name="manager">The manager which holds the memory repositories.</param>
    public ConfigurationInMemoryContext(InMemoryRepositoryManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public async ValueTask<Guid?> GetDefaultGameConfigurationIdAsync()
    {
        var allConfigs = await this.Manager.GetRepository<GameConfiguration>().GetAllAsync().ConfigureAwait(false);
        return allConfigs.FirstOrDefault()?.Id;
    }
}