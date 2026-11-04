// <copyright file="GameConfigurationLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Loading;

using System.Data.Common;
using System.Threading;

/// <summary>
/// Loads the game configurations with one select statement per table, which are sent to the database as one batch.
/// The rows are materialized and the objects are connected by generated code.
/// </summary>
/// <remarks>
/// It creates the same object graph as the <see cref="Json.GameConfigurationJsonObjectLoader"/>, but the database doesn't
/// need to build the json, and the client doesn't need to parse it. The generated part is created by the
/// <c>GameConfigurationLoaderGenerator</c>, see Readme.md.
/// An instance can only be used once, because it keeps the loaded objects.
/// </remarks>
internal sealed partial class GameConfigurationLoader
{
    /// <summary>
    /// The name of the <see cref="AppContext"/> switch which disables this loader.
    /// When it's set, the game configuration is loaded by the <see cref="Json.GameConfigurationJsonObjectLoader"/>.
    /// </summary>
    public const string DisableSwitch = "MUnique.OpenMU.Persistence.DisableGameConfigurationLoader";

    /// <summary>
    /// Gets a value indicating whether this loader is enabled.
    /// </summary>
    public static bool IsEnabled => !AppContext.TryGetSwitch(DisableSwitch, out var isDisabled) || !isDisabled;

    /// <summary>
    /// Loads all game configurations with their object graphs.
    /// </summary>
    /// <param name="connection">The open database connection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded game configurations.</returns>
    public async ValueTask<IReadOnlyList<Model.GameConfiguration>> LoadAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        await using var batch = connection.CreateBatch();
        foreach (var statement in Statements)
        {
            var command = batch.CreateBatchCommand();
            command.CommandText = statement;
            batch.BatchCommands.Add(command);
        }

        await using (var reader = await batch.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
            for (var statementIndex = 0; statementIndex < Statements.Length; statementIndex++)
            {
                if (statementIndex > 0 && !await reader.NextResultAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new InvalidOperationException($"The result of statement {statementIndex} is missing.");
                }

                await this.ReadResultAsync(statementIndex, reader, cancellationToken).ConfigureAwait(false);
            }
        }

        this.Fixup();
        return this.LoadedGameConfigurations.ToList();
    }

    private static async ValueTask ReadJoinAsync(DbDataReader reader, System.Collections.Generic.List<(Guid Key, Guid Other)> rows, CancellationToken cancellationToken)
    {
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            rows.Add((reader.GetGuid(0), reader.GetGuid(1)));
        }
    }
}
