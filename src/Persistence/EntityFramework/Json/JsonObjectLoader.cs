// <copyright file="JsonObjectLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading;

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using System.Data;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

/// <summary>
/// A class which allows to load objects from the database by using a query provided by the <see cref="JsonQueryBuilder"/>
/// which retrieves a whole object graph by using json functions of postgres.
/// </summary>
public class JsonObjectLoader
{
    private readonly JsonQueryBuilder _queryBuilder;
    private readonly JsonObjectDeserializer _deserializer;
    private readonly ReferenceHandler _referenceHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectLoader" /> class.
    /// </summary>
    /// <param name="queryBuilder">The query builder.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <param name="referenceHandler">The reference handler.</param>
    public JsonObjectLoader(JsonQueryBuilder queryBuilder, JsonObjectDeserializer deserializer, ReferenceHandler referenceHandler)
    {
        this._queryBuilder = queryBuilder;
        this._deserializer = deserializer;
        this._referenceHandler = referenceHandler;
    }

    /// <summary>
    /// Loads all objects of the specified type <typeparamref name="T"/>.
    /// Please note, that it's expected that the database connection is already established when calling this method.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IIdentifiable"/> object.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>All objects of <typeparamref name="T"/>.</returns>
    public async ValueTask<IEnumerable<T>> LoadAllObjectsAsync<T>(DbContext context, CancellationToken cancellationToken = default)
        where T : class, IIdentifiable
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = new List<T>();
        var type = context.Model.FindEntityType(typeof(T)) ?? throw new ArgumentException($"{typeof(T)} is not included in the model of the context.");
        var queryString = this._queryBuilder.BuildJsonQueryForEntity(type);
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);
        if (reader.HasRows)
        {
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                await using var textReader = reader.GetStream(2);
                cancellationToken.ThrowIfCancellationRequested();
                if (this._deserializer.Deserialize<T>(textReader, this._referenceHandler) is { } item)
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Loads the object with the specified id from the context.
    /// Please note, that it's expected that the database connection is already established when calling this method.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IIdentifiable"/> object.</typeparam>
    /// <param name="id">The identifier of the object which should be loaded.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded object, if available; Otherwise, null.</returns>
    public async ValueTask<T?> LoadObjectAsync<T>(Guid id, DbContext context, CancellationToken cancellationToken = default)
        where T : class, IIdentifiable
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEntityType type;
        await using (var completeContext = new EntityDataContext())
        {
            type = completeContext.Model.FindEntityType(typeof(T)) ?? throw new ArgumentException($"{typeof(T)} is not included in the model of the context.");
        }

        var queryString = this._queryBuilder.BuildJsonQueryForEntity(type);
        queryString += " where result.\"Id\" = @id;";
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        var idParameter = command.CreateParameter();
        idParameter.ParameterName = "id";
        idParameter.Value = id;

        command.Parameters.Add(idParameter);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess | CommandBehavior.SingleResult, cancellationToken).ConfigureAwait(false);
        if (reader.HasRows && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            await using var textReader = reader.GetStream(2);
            return this._deserializer.Deserialize<T>(textReader, this._referenceHandler);
        }

        return default;
    }
}