// <copyright file="ByDataSourceReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Text.Json.Serialization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A reference resolver which uses a <see cref="IDataSource{T}"/> to resolve references.
/// </summary>
public class ByDataSourceReferenceResolver : ReferenceResolver
{
    /// <summary>
    /// The data source.
    /// </summary>
    private readonly IDataSource<GameConfiguration> _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByDataSourceReferenceResolver"/> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    public ByDataSourceReferenceResolver(IDataSource<GameConfiguration> dataSource)
    {
        this._dataSource = dataSource;
    }

    /// <inheritdoc />
    public override void AddReference(string referenceId, object value)
    {
        // do nothing here, because the data source is the source of truth.
    }

    /// <inheritdoc />
    public override string GetReference(object value, out bool alreadyExists)
    {
        if (value is IIdentifiable identifiable)
        {
            alreadyExists = this._dataSource.Get(identifiable.Id) is { };
            return identifiable.Id.ToString();
        }

        alreadyExists = false;
        return string.Empty;
    }

    /// <inheritdoc />
    public override object ResolveReference(string referenceId)
    {
        var id = Guid.Parse(referenceId);
        return this._dataSource.Get(id) ?? throw new KeyNotFoundException($"Reference with id '{referenceId}' not found.");
    }
}