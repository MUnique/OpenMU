// <copyright file="ByDataSourceReferenceHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Text.Json.Serialization;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A reference handler which uses a <see cref="IDataSource{T}"/> to resolve references.
/// </summary>
public class ByDataSourceReferenceHandler : ReferenceHandler
{
    /// <summary>
    /// The data source.
    /// </summary>
    private readonly IDataSource<GameConfiguration> _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByDataSourceReferenceHandler"/> class.
    /// </summary>
    /// <param name="dataSource">The data source.</param>
    public ByDataSourceReferenceHandler(IDataSource<GameConfiguration> dataSource)
    {
        this._dataSource = dataSource;
    }

    /// <summary>
    /// Gets the data source which is used to resolve the references.
    /// It's exposed, so that it can be reloaded when the underlying data changed,
    /// e.g. after the database has been (re-)initialized.
    /// </summary>
    public IDataSource<GameConfiguration> DataSource => this._dataSource;

    /// <inheritdoc />
    public override ReferenceResolver CreateResolver()
    {
        return new ByDataSourceReferenceResolver(this._dataSource);
    }
}