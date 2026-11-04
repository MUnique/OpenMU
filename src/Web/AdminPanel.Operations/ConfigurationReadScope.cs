// <copyright file="ConfigurationReadScope.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>Reads persisted configuration independently when the panel has pending edits.</summary>
internal sealed class ConfigurationReadScope : IDisposable
{
    private IContext? _context;

    /// <summary>Gets the configuration snapshot for the lifetime of this scope.</summary>
    internal GameConfiguration Configuration { get; private set; } = null!;

    /// <inheritdoc />
    public void Dispose() => this._context?.Dispose();

    /// <summary>Loads the active configuration while preserving a dirty graphical editor.</summary>
    /// <param name="persistence">The persistence provider.</param>
    /// <param name="source">The active configuration source.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async ValueTask LoadAsync(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> source, CancellationToken cancellationToken)
    {
        var owner = await source.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var shared = await source.GetContextAsync(cancellationToken).ConfigureAwait(false);
        if (shared?.HasChanges is true)
        {
            this._context = persistence.CreateNewContext();
            this.Configuration = await this._context.GetByIdAsync<GameConfiguration>(owner.GetId(), cancellationToken).ConfigureAwait(false)
                ?? throw new AdminOperationException("The active configuration could not be loaded.");
        }
        else
        {
            this.Configuration = owner;
        }
    }
}
