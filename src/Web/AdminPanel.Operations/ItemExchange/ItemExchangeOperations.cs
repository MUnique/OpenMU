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
public sealed class ItemExchangeOperations(IPersistenceContextProvider persistence)
{
    /// <summary>
    /// Writes the exchange file of the stored game configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The file as UTF-8 text.</returns>
    /// <remarks>
    /// It reads the configuration in a context of its own, so edits which the admin panel
    /// hasn't saved yet are not part of the file.
    /// </remarks>
    [AdminOperation(AdminRole.Viewer)]
    [NotExposedToAgents("The file is for the client editor's update and holds every item at once. Agents read items with the configuration tools.")]
    public async ValueTask<byte[]> ExportAsync(CancellationToken cancellationToken)
    {
        Guid? id;
        using (var configurationContext = persistence.CreateNewConfigurationContext())
        {
            id = await configurationContext.GetDefaultGameConfigurationIdAsync(cancellationToken).ConfigureAwait(false);
        }

        if (id is not { } configurationId)
        {
            throw new AdminOperationException("There is no game configuration yet.", AdminOperationErrorKind.Unavailable);
        }

        using var context = persistence.CreateNewContext();
        var configuration = await context.GetByIdAsync<GameConfiguration>(configurationId, cancellationToken).ConfigureAwait(false)
            ?? throw new AdminOperationException("The game configuration could not be loaded.", AdminOperationErrorKind.Unavailable);
        return ItemExchangeExport.Write(configuration);
    }
}
