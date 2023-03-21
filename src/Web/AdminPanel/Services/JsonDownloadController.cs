// <copyright file="JsonDownloadController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Controller to retrieve data as json.
/// Supported types: <see cref="GenericControllerFeatureProvider.SupportedTypes"/>.
/// </summary>
/// <typeparam name="T">The source type of the data object.</typeparam>
/// <typeparam name="TSerializable">The type of the serializable.</typeparam>
[Route("download/[controller]")]
[GenericControllerName]
public class JsonDownloadController<T, TSerializable> : ControllerBase
    where T : class
    where TSerializable : class
{
    private readonly IDataSource<T> _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDownloadController{T,TSerializable}" /> class.
    /// </summary>
    /// <param name="_dataSource">The data source.</param>
    public JsonDownloadController(IDataSource<T> _dataSource)
    {
        this._dataSource = _dataSource;
    }

    /// <summary>
    /// Gets the configuration with the specified id as json string.
    /// </summary>
    /// <param name="objectId">The identifier of the requested object.</param>
    /// <returns>
    /// The requested configuration as json string.
    /// </returns>
    [HttpGet("[controller]_{objectId}.json")]
    public async ValueTask<IActionResult?> GetConfigurationById(Guid objectId)
    {
        await this._dataSource.DiscardChangesAsync();
        var owner = await this._dataSource.GetOwnerAsync(objectId);
        
        if (owner is IConvertibleTo<TSerializable> convertibleTo)
        {
            convertibleTo.Convert().ToJson(this.Response.Body);
            this.Response.ContentType = "application/json";
            return this.Ok();
        }

        return null;
    }
}