// <copyright file="JsonObjectLoader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json.Serialization;

/// <summary>
/// A class which allows to load objects from the database by using a query provided by the <see cref="JsonQueryBuilder"/>
/// which retrieves a whole object graph by using json functions of postgres.
/// </summary>
public class JsonObjectLoader
{
    private readonly JsonQueryBuilder _queryBuilder;
    private readonly JsonObjectDeserializer _deserializer;
    private readonly IReferenceResolver _referenceResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectLoader"/> class.
    /// </summary>
    /// <param name="queryBuilder">The query builder.</param>
    /// <param name="deserializer">The deserializer.</param>
    /// <param name="referenceResolver">The reference resolver.</param>
    public JsonObjectLoader(JsonQueryBuilder queryBuilder, JsonObjectDeserializer deserializer, IReferenceResolver referenceResolver)
    {
        this._queryBuilder = queryBuilder;
        this._deserializer = deserializer;
        this._referenceResolver = referenceResolver;
    }

    /// <summary>
    /// Loads all objects of the specified type <typeparamref name="T"/>.
    /// Please note, that it's expected that the database connection is already established when calling this method.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IIdentifiable"/> object.</typeparam>
    /// <param name="context">The context.</param>
    /// <returns>All objects of <typeparamref name="T"/>.</returns>
    public IEnumerable<T> LoadAllObjects<T>(DbContext context)
        where T : class, IIdentifiable
    {
        var result = new List<T>();
        var type = context.Model.FindEntityType(typeof(T)) ?? throw new ArgumentException($"{typeof(T)} is not included in the model of the context.");
        var queryString = this._queryBuilder.BuildJsonQueryForEntity(type);
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                if (this._deserializer.Deserialize<T>(reader.GetTextReader(2), this._referenceResolver) is { } item)
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
    /// <returns>The loaded object, if available; Otherwise, null.</returns>
    public T? LoadObject<T>(Guid id, DbContext context)
        where T : class, IIdentifiable
    {
        IEntityType type;
        using (var completeContext = new EntityDataContext())
        {
            type = completeContext.Model.FindEntityType(typeof(T)) ?? throw new ArgumentException($"{typeof(T)} is not included in the model of the context.");
        }

        var queryString = this._queryBuilder.BuildJsonQueryForEntity(type);
        queryString += " where result.\"Id\" = @id;";
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        var idParameter = command.CreateParameter();
        idParameter.ParameterName = "id";
        idParameter.Value = id;

        command.Parameters.Add(idParameter);
        using var reader = command.ExecuteReader();
        if (reader.HasRows && reader.Read())
        {
            return this._deserializer.Deserialize<T>(reader.GetTextReader(2), this._referenceResolver);
        }

        return default;
    }
}