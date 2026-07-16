// <copyright file="BotServerPartition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Splits the bot population over the game servers of the deployment, so a server does not animate the
/// whole population by itself: bots count towards the player count of their server, and a server which
/// is full turns real clients away - the bots would lock the players out of the game.
/// <para>
/// Which accounts a server animates is a pure function of the account index and the SET of configured
/// game servers, so every server computes the same answer without asking the others - no coordination,
/// no shared state, and it holds in a deployment where each game server is its own process. The share
/// of a server is proportional to its capacity, and only a part of that capacity
/// (<see cref="BotConfiguration.BotCapacityPercent"/>) is handed to the bots: the rest stays reserved
/// for real players, who must never be denied a slot by a bot.
/// </para>
/// <para>
/// The split is computed when the server starts. Adding a game server to a running deployment therefore
/// takes a restart before the population spreads onto it; that is deliberate. Moving the ownership of a
/// bot between two RUNNING servers would mean one server animating an account the other one is still
/// animating - the very cross-context situation which corrupts a character.
/// </para>
/// </summary>
internal sealed class BotServerPartition
{
    private BotServerPartition(int firstAccount, int accountCount, bool isGenerator)
    {
        this.FirstAccount = firstAccount;
        this.AccountCount = accountCount;
        this.IsGenerator = isGenerator;
    }

    /// <summary>
    /// Gets the one-based index of the first bot account this server animates.
    /// </summary>
    public int FirstAccount { get; }

    /// <summary>
    /// Gets the number of bot accounts this server animates.
    /// </summary>
    public int AccountCount { get; }

    /// <summary>
    /// Gets a value indicating whether this server generates the bot population. Exactly one server
    /// does it (the one which animates the first account), so the generation of the accounts - and of
    /// their unique character names - never runs twice at the same time. The other servers simply find
    /// their accounts once they exist; until then, their spawns are retried by the maintenance pass.
    /// </summary>
    public bool IsGenerator { get; }

    /// <summary>
    /// Determines the share of the bot population which the given game server animates.
    /// </summary>
    /// <param name="gameContext">The context of the game server which asks.</param>
    /// <param name="configuration">The bot configuration.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The share of this server.</returns>
    public static async ValueTask<BotServerPartition> CreateAsync(IGameContext gameContext, BotConfiguration configuration, ILogger logger)
    {
        var requestedAccounts = Math.Max(configuration.NumberOfAccounts, 0);
        var charactersPerAccount = configuration.GetEffectiveCharactersPerAccount();
        var capacities = await GetAccountCapacitiesAsync(gameContext, configuration, charactersPerAccount, logger).ConfigureAwait(false);
        if (gameContext is not IGameServerContext serverContext || capacities.Count == 0)
        {
            // A deployment we cannot split (no server definitions readable, or a context which is not a
            // game server, e.g. in tests): behave exactly like before - this server animates everything.
            return new BotServerPartition(1, requestedAccounts, true);
        }

        var (partition, assignedAccounts) = Split(capacities, serverContext.Id, requestedAccounts);
        if (assignedAccounts < requestedAccounts)
        {
            logger.LogWarning(
                "The bot population does not fit: {Requested} account(s) configured, but only {Fitting} fit into {Percent}% of the game servers' capacity. {Dropped} account(s) stay offline - raise the servers' maximum player count, the bot capacity share, or lower the number of accounts.",
                requestedAccounts,
                assignedAccounts,
                configuration.GetEffectiveBotCapacityPercent(),
                requestedAccounts - assignedAccounts);
        }

        logger.LogInformation(
            "This game server ({ServerId}) animates {Count} bot account(s) ({First}..{Last}) of {Requested}.",
            serverContext.Id,
            partition.AccountCount,
            partition.AccountCount == 0 ? 0 : partition.FirstAccount,
            partition.AccountCount == 0 ? 0 : partition.FirstAccount + partition.AccountCount - 1,
            requestedAccounts);

        return partition;
    }

    /// <summary>
    /// Determines whether this server animates the bot account with the given one-based index.
    /// </summary>
    /// <param name="accountIndex">The one-based bot account index.</param>
    public bool Owns(int accountIndex)
        => accountIndex >= this.FirstAccount && accountIndex < this.FirstAccount + this.AccountCount;

    /// <summary>
    /// Hands the accounts to the servers, each getting a share PROPORTIONAL to its capacity: the pure
    /// decision behind <see cref="CreateAsync"/>. Every server runs it over the same list and gets the
    /// same answer, which is what makes the split need no coordination at all.
    /// <para>
    /// Proportional, not first-come: filling one server to the brim before using the next would leave the
    /// added server empty until the first one overflows - and a player on it would meet nobody. The bots
    /// are there to populate the world, so they spread over the servers the players can choose from.
    /// </para>
    /// </summary>
    /// <param name="capacities">How many accounts each game server may animate, ordered by server id.</param>
    /// <param name="serverId">The id of the server which asks.</param>
    /// <param name="requestedAccounts">The configured number of bot accounts.</param>
    /// <returns>The share of the asking server, and how many accounts fit into the deployment at all.</returns>
    internal static (BotServerPartition Partition, int AssignedAccounts) Split(
        IEnumerable<(byte ServerId, int Capacity)> capacities,
        byte serverId,
        int requestedAccounts)
    {
        var servers = capacities.Where(c => c.Capacity > 0).ToList();
        var totalCapacity = servers.Sum(server => (long)server.Capacity);
        if (totalCapacity == 0 || requestedAccounts <= 0)
        {
            return (new BotServerPartition(1, 0, false), 0);
        }

        // What does not fit into the servers' share stays offline; those accounts wake up as soon as the
        // deployment offers the room (another game server, a higher player limit or bot capacity share).
        var assignedAccounts = (int)Math.Min(requestedAccounts, totalCapacity);

        var partition = new BotServerPartition(1, 0, false);
        long capacitySoFar = 0;
        var accountsSoFar = 0;
        foreach (var (currentServer, capacity) in servers)
        {
            capacitySoFar += capacity;

            // Walk the cumulative capacity, so the rounding of one server's share is corrected by the
            // next one instead of adding up: the shares always sum up to the assigned accounts exactly.
            var accountsUpToHere = (int)(assignedAccounts * capacitySoFar / totalCapacity);
            var share = accountsUpToHere - accountsSoFar;
            if (currentServer == serverId && share > 0)
            {
                // The server which owns the first account generates the population.
                partition = new BotServerPartition(accountsSoFar + 1, share, accountsSoFar == 0);
            }

            accountsSoFar = accountsUpToHere;
        }

        return (partition, assignedAccounts);
    }

    /// <summary>
    /// Reads how many bot ACCOUNTS each configured game server may animate: its maximum player count,
    /// reduced to the bots' share of it, divided by the characters an account animates at once. The
    /// servers are ordered by their id, so every server walks the same list in the same order.
    /// </summary>
    private static async ValueTask<List<(byte ServerId, int Capacity)>> GetAccountCapacitiesAsync(
        IGameContext gameContext,
        BotConfiguration configuration,
        int charactersPerAccount,
        ILogger logger)
    {
        try
        {
            using var context = gameContext.PersistenceContextProvider.CreateNewConfigurationContext();
            var definitions = await context.GetAsync<GameServerDefinition>().ConfigureAwait(false);
            var capacityPercent = configuration.GetEffectiveBotCapacityPercent();
            return definitions
                .OrderBy(definition => definition.ServerID)
                .Select(definition => (
                    definition.ServerID,
                    Capacity: (definition.ServerConfiguration?.MaximumPlayers ?? 0) * capacityPercent / 100 / charactersPerAccount))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not read the game server definitions; this server animates the whole bot population.");
            return [];
        }
    }
}
