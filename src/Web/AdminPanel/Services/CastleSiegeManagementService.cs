// <copyright file="CastleSiegeManagementService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Provides the administration panel with safe access to in-process Castle Siege game servers.
/// </summary>
/// <remarks>
/// Runtime Castle Siege state is not transported over the public administration API. Therefore this service
/// intentionally only exposes game servers that provide their local <see cref="IGameServerContext"/>.
/// </remarks>
public sealed class CastleSiegeManagementService
{
    private readonly IServerProvider _serverProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeManagementService"/> class.
    /// </summary>
    /// <param name="serverProvider">The provider of manageable servers.</param>
    public CastleSiegeManagementService(IServerProvider serverProvider)
    {
        this._serverProvider = serverProvider;
    }

    /// <summary>
    /// Gets the game servers which can provide direct Castle Siege runtime access.
    /// </summary>
    /// <returns>The available game servers.</returns>
    public IReadOnlyList<CastleSiegeManagementGameServer> AvailableGameServers =>
        this._serverProvider.Servers
            .OfType<IGameServer>()
            .Where(server => server is IGameServerContextProvider)
            .OrderBy(server => server.Id)
            .Select(server => new CastleSiegeManagementGameServer(server.Id, server.Description))
            .ToList();

    /// <summary>
    /// Gets the current Castle Siege snapshot for a game server.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <returns>The status result.</returns>
    public async ValueTask<CastleSiegeManagementSnapshotResult> GetSnapshotAsync(int gameServerId)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        if (administration is null || context is null)
        {
            return new(null, error);
        }

        var snapshot = await administration.GetSnapshotAsync(context).ConfigureAwait(false);
        return snapshot is null
            ? new(null, CastleSiegeAdministrationError.NotInitialized)
            : new(snapshot, CastleSiegeAdministrationError.None);
    }

    /// <summary>
    /// Requests a Castle Siege state transition.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <param name="state">The requested state.</param>
    /// <returns>The operation result.</returns>
    public ValueTask<CastleSiegeAdministrationResult> ForceStateAsync(int gameServerId, CastleSiegeState state)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return ValueTask.FromResult(administration is null || context is null
            ? CastleSiegeAdministrationResult.Failed(error)
            : administration.ForceState(context, state));
    }

    /// <summary>
    /// Assigns the castle to a guild.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask<CastleSiegeAdministrationResult> SetOwnerAsync(int gameServerId, string guildName)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return administration is null || context is null
            ? ValueTask.FromResult(CastleSiegeAdministrationResult.Failed(error))
            : administration.SetOwnerAsync(context, guildName);
    }

    /// <summary>
    /// Clears registrations and resets the current Castle Siege cycle.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask<CastleSiegeAdministrationResult> ResetCycleAsync(int gameServerId)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return administration is null || context is null
            ? ValueTask.FromResult(CastleSiegeAdministrationResult.Failed(error))
            : administration.ResetCycleAsync(context);
    }

    /// <summary>
    /// Updates the Castle Siege tax rates.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <param name="chaosTax">The Chaos Machine tax percentage.</param>
    /// <param name="storeTax">The personal-store tax percentage.</param>
    /// <param name="huntTax">The Land of Trials entrance fee.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask<CastleSiegeAdministrationResult> SetTaxesAsync(
        int gameServerId,
        byte chaosTax,
        byte storeTax,
        int huntTax)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return administration is null || context is null
            ? ValueTask.FromResult(CastleSiegeAdministrationResult.Failed(error))
            : administration.SetTaxesAsync(context, chaosTax, storeTax, huntTax);
    }

    /// <summary>
    /// Clears the accumulated Castle Siege tribute.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask<CastleSiegeAdministrationResult> ClearTributeAsync(int gameServerId)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return administration is null || context is null
            ? ValueTask.FromResult(CastleSiegeAdministrationResult.Failed(error))
            : administration.ClearTributeAsync(context);
    }

    /// <summary>
    /// Removes one Castle Siege guild registration.
    /// </summary>
    /// <param name="gameServerId">The game-server identifier.</param>
    /// <param name="guildId">The persistent guild identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask<CastleSiegeAdministrationResult> RemoveRegistrationAsync(int gameServerId, Guid guildId)
    {
        var (administration, context, error) = this.Resolve(gameServerId);
        return administration is null || context is null
            ? ValueTask.FromResult(CastleSiegeAdministrationResult.Failed(error))
            : administration.RemoveRegistrationAsync(context, guildId);
    }

    private (CastleSiegeAdministration? Administration, IGameServerContext? Context, CastleSiegeAdministrationError Error) Resolve(int gameServerId)
    {
        var server = this._serverProvider.Servers
            .OfType<IGameServer>()
            .FirstOrDefault(candidate => candidate.Id == gameServerId);
        if (server is null)
        {
            return (null, null, CastleSiegeAdministrationError.GameServerUnavailable);
        }

        if (server is not IGameServerContextProvider contextProvider)
        {
            return (null, null, CastleSiegeAdministrationError.AllInOneDeploymentRequired);
        }

        var plugIn = contextProvider.Context.PlugInManager
            .GetActivePlugInsOf<IPeriodicTaskPlugIn>()
            .OfType<CastleSiegePlugIn>()
            .FirstOrDefault();
        return plugIn is null
            ? (null, contextProvider.Context, CastleSiegeAdministrationError.PlugInInactive)
            : (new CastleSiegeAdministration(plugIn), contextProvider.Context, CastleSiegeAdministrationError.None);
    }
}
