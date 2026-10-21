// <copyright file="LoadingMeasurements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Measurements;

using System.Data;
using System.Data.Common;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Measures loading the game configuration and accounts through the JSON query.
/// </summary>
/// <remarks>
/// Besides the end-to-end time through the repositories, the loading is split into:
///   - planning and execution time on the database server (EXPLAIN ANALYZE, without transferring the result),
///   - executing the query and reading the whole JSON result into memory (server time + transfer),
///   - deserializing the JSON from memory.
/// </remarks>
internal static class LoadingMeasurements
{
    /// <summary>
    /// Measures the loading of the game configuration.
    /// </summary>
    /// <param name="iterations">The number of warm iterations.</param>
    public static async ValueTask MeasureConfigurationAsync(int iterations)
    {
        Measure.Section("Game configuration loading");

        var endToEnd = new List<Measure.Result>();
        for (var i = 0; i <= iterations; i++)
        {
            // A new provider for each iteration, because the configuration is cached by the provider.
            var (result, _) = await Measure.RunAsync(LoadConfigurationAsync).ConfigureAwait(false);
            endToEnd.Add(result);
        }

        Measure.ColdAndWarmRows("End-to-end: GameConfigurationContext.GetByIdAsync", endToEnd, "cold includes EF model building and JIT");

        await using var context = new EntityDataContext();
        var entityType = context.Model.FindEntityType(typeof(Persistence.EntityFramework.Model.GameConfiguration))!;
        var query = new GameConfigurationJsonQueryBuilder().BuildJsonQueryForEntity(entityType);
        await MeasureSplitAsync(
                context,
                query,
                null,
                iterations,
                stream => new Persistence.EntityFramework.Json.JsonObjectDeserializer().Deserialize<Persistence.EntityFramework.Model.GameConfiguration>(stream, new IdReferenceHandler()))
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Measures the steps of loading the game configuration, each step cold.
    /// </summary>
    /// <remarks>
    /// In the end-to-end measurement of <see cref="MeasureConfigurationAsync"/>, the cold costs of all steps
    /// add up into one number. Here, each step is measured on its own, so it's visible where the cold time goes.
    /// The end-to-end measurement at the end only contains the remaining cold costs, e.g. building the model
    /// of the <see cref="ConfigurationContext"/>.
    /// </remarks>
    public static async ValueTask MeasureConfigurationColdAsync()
    {
        Measure.Section("Game configuration loading, each step cold");

        await using var context = new EntityDataContext();
        Measure.Row("EF model of the EntityDataContext", Measure.Run(() => _ = context.Model), "required to build the query");

        var query = string.Empty;
        Measure.Row("Building the JSON query", Measure.Run(() =>
        {
            var entityType = context.Model.FindEntityType(typeof(Persistence.EntityFramework.Model.GameConfiguration))!;
            query = new GameConfigurationJsonQueryBuilder().BuildJsonQueryForEntity(entityType);
        }));

        var (open, _) = await Measure.RunAsync(async () =>
        {
            await context.Database.OpenConnectionAsync().ConfigureAwait(false);
            return true;
        }).ConfigureAwait(false);
        Measure.Row("Opening the database connection", open);

        using var buffer = new MemoryStream();
        var (fetch, _) = await Measure.RunAsync(async () =>
        {
            await FetchJsonAsync(context.Database.GetDbConnection(), query, null, buffer).ConfigureAwait(false);
            return true;
        }).ConfigureAwait(false);
        Measure.Row("Query execution + reading JSON", fetch, $"{buffer.Length / 1024.0 / 1024.0:0.00} MB of JSON");

        buffer.Position = 0;
        Measure.Row(
            "Deserialization from memory",
            Measure.Run(() => _ = new Persistence.EntityFramework.Json.JsonObjectDeserializer().Deserialize<Persistence.EntityFramework.Model.GameConfiguration>(buffer, new IdReferenceHandler())),
            "includes creating the converters");

        var (endToEnd, _) = await Measure.RunAsync(LoadConfigurationAsync).ConfigureAwait(false);
        Measure.Row("End-to-end: GameConfigurationContext.GetByIdAsync (after the steps above)", endToEnd, "remaining cold costs");
    }

    /// <summary>
    /// Measures the loading of accounts.
    /// </summary>
    /// <param name="iterations">The number of warm iterations.</param>
    /// <param name="loginNames">The login names of the accounts.</param>
    public static async ValueTask MeasureAccountsAsync(int iterations, IEnumerable<string> loginNames)
    {
        var provider = new PersistenceContextProvider(NullLoggerFactory.Instance, null);
        var configuration = await LoadConfigurationAsync(provider).ConfigureAwait(false);

        await using var context = new EntityDataContext();
        var entityType = context.Model.FindEntityType(typeof(Persistence.EntityFramework.Model.Account))!;
        var query = new JsonQueryBuilder().BuildJsonQueryForEntity(entityType) + " where result.\"Id\" = @id;";

        foreach (var loginName in loginNames)
        {
            Measure.Section($"Account loading: {loginName}");

            var endToEnd = new List<Measure.Result>();
            Guid accountId = default;
            for (var i = 0; i <= iterations; i++)
            {
                // A new player context for each iteration, like for each login.
                var (result, account) = await Measure.RunAsync(async () =>
                {
                    using var playerContext = provider.CreateNewPlayerContext(configuration);
                    return await playerContext.GetAccountByLoginNameAsync(loginName).ConfigureAwait(false);
                }).ConfigureAwait(false);

                accountId = (account as Persistence.IIdentifiable)?.Id ?? throw new InvalidOperationException($"Account {loginName} not found.");
                endToEnd.Add(result);
            }

            Measure.ColdAndWarmRows("End-to-end: IPlayerContext.GetAccountByLoginNameAsync", endToEnd, "includes login name lookup and attaching to the context");

            await MeasureSplitAsync(
                    context,
                    query,
                    accountId,
                    iterations,
                    stream => new Persistence.EntityFramework.Json.JsonObjectDeserializer().Deserialize<Persistence.EntityFramework.Model.Account>(stream, new CachingReferenceHandler()))
                .ConfigureAwait(false);
            Console.WriteLine();
        }
    }

    private static ValueTask<DataModel.Configuration.GameConfiguration> LoadConfigurationAsync()
    {
        return LoadConfigurationAsync(new PersistenceContextProvider(NullLoggerFactory.Instance, null));
    }

    private static async ValueTask<DataModel.Configuration.GameConfiguration> LoadConfigurationAsync(PersistenceContextProvider provider)
    {
        using var configurationContext = provider.CreateNewConfigurationContext();
        var id = await configurationContext.GetDefaultGameConfigurationIdAsync(default).ConfigureAwait(false)
                 ?? throw new InvalidOperationException("No game configuration found. Run the 'init' mode first.");
        return await configurationContext.GetByIdAsync<DataModel.Configuration.GameConfiguration>(id).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Game configuration couldn't be loaded.");
    }

    private static async ValueTask MeasureSplitAsync<T>(DbContext context, string query, Guid? id, int iterations, Func<Stream, T?> deserialize)
    {
        await context.Database.OpenConnectionAsync().ConfigureAwait(false);
        var connection = context.Database.GetDbConnection();

        var (planning, execution) = await ExplainAnalyzeAsync(connection, query, id).ConfigureAwait(false);
        Console.WriteLine($"| Database server: planning (EXPLAIN ANALYZE) | {planning:0.0} | | query has {query.Length:N0} characters |");
        Console.WriteLine($"| Database server: execution (EXPLAIN ANALYZE) | {execution:0.0} | | without transferring the result |");

        var fetches = new List<Measure.Result>();
        var deserializations = new List<Measure.Result>();
        long jsonBytes = 0;
        for (var i = 0; i <= iterations; i++)
        {
            using var buffer = new MemoryStream();
            fetches.Add((await Measure.RunAsync(async () =>
            {
                await FetchJsonAsync(connection, query, id, buffer).ConfigureAwait(false);
                return true;
            }).ConfigureAwait(false)).Result);
            jsonBytes = buffer.Length;

            buffer.Position = 0;
            deserializations.Add(Measure.Run(() => _ = deserialize(buffer)));
        }

        Measure.ColdAndWarmRows("Query execution + reading JSON", fetches, $"{jsonBytes / 1024.0 / 1024.0:0.00} MB of JSON");
        Measure.ColdAndWarmRows("Deserialization from memory", deserializations);
    }

    private static async ValueTask FetchJsonAsync(DbConnection connection, string query, Guid? id, Stream target)
    {
        await using var command = CreateCommand(connection, query, id);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            await using var stream = reader.GetStream(2);
            await stream.CopyToAsync(target).ConfigureAwait(false);
        }
    }

    private static async ValueTask<(double PlanningMs, double ExecutionMs)> ExplainAnalyzeAsync(DbConnection connection, string query, Guid? id)
    {
        await using var command = CreateCommand(connection, "EXPLAIN (ANALYZE, TIMING OFF, FORMAT JSON) " + query.TrimEnd().TrimEnd(';'), id);
        var json = (string)(await command.ExecuteScalarAsync().ConfigureAwait(false))!;
        using var document = System.Text.Json.JsonDocument.Parse(json);
        var root = document.RootElement[0];
        return (root.GetProperty("Planning Time").GetDouble(), root.GetProperty("Execution Time").GetDouble());
    }

    private static DbCommand CreateCommand(DbConnection connection, string query, Guid? id)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        if (id is { } value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = "id";
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        return command;
    }
}
