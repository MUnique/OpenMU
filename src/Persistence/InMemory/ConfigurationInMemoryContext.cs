// <copyright file="ConfigurationInMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// A context which is used to access the configuration data in-memory.
/// </summary>
public class ConfigurationInMemoryContext : InMemoryContext, IConfigurationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationInMemoryContext"/> class.
    /// </summary>
    /// <param name="provider">The manager which holds the memory repositories.</param>
    public ConfigurationInMemoryContext(InMemoryRepositoryProvider provider)
        : base(provider)
    {
    }

    /// <inheritdoc />
    public async ValueTask<Guid?> GetDefaultGameConfigurationIdAsync(CancellationToken cancellationToken)
    {
        var allConfigs = await this.Provider.GetRepository<GameConfiguration>().GetAllAsync(cancellationToken).ConfigureAwait(false);
        return allConfigs.FirstOrDefault()?.Id;
    }
}