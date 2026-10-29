// <copyright file="ItemExchangeOperations.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.ItemExchange;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;

/// <summary>
/// Provides the item exchange file of the stored game configuration.
/// </summary>
/// <param name="persistence">The persistence context provider.</param>
/// <param name="configurations">The game configuration which the admin panel edits.</param>
public sealed class ItemExchangeOperations(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations)
{
    /// <summary>
    /// The number of attempts to write the file while an edit in the admin panel changes the items.
    /// </summary>
    private const int MaximumWriteAttempts = 3;

    /// <summary>
    /// Writes the exchange file of the stored game configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file as UTF-8 text.</returns>
    /// <remarks>
    /// It reads the configuration through a <see cref="ConfigurationReadScope"/>: the one the admin
    /// panel has already loaded, or a copy of the stored one while the admin panel has unsaved
    /// changes. So edits which aren't saved yet are not part of the file, and the configuration
    /// is usually not loaded again for each request.
    /// </remarks>
    [AdminOperation(AdminRole.Viewer)]
    [NotExposedToAgents("The file is for the client editor's update and holds every item at once. Agents read items with the configuration tools.")]
    public async ValueTask<byte[]> ExportAsync(CancellationToken cancellationToken)
    {
        using (var configurationContext = persistence.CreateNewConfigurationContext())
        {
            if (await configurationContext.GetDefaultGameConfigurationIdAsync(cancellationToken).ConfigureAwait(false) is null)
            {
                throw new AdminOperationException("There is no game configuration yet.", AdminOperationErrorKind.Unavailable);
            }
        }

        using var read = new ConfigurationReadScope();
        await read.LoadAsync(persistence, configurations, cancellationToken).ConfigureAwait(false);
        for (var attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return ItemExchangeExport.Write(read.Configuration);
            }
            catch (InvalidOperationException) when (attempt < MaximumWriteAttempts)
            {
                // An edit in the admin panel changed the items while they were read, like in the
                // configuration reads of the agents. The file is written again from the start.
            }
        }
    }
}
