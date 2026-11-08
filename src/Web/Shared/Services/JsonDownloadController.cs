// <copyright file="JsonDownloadController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Controller to retrieve data as json.
/// Supported types: <see cref="GenericControllerFeatureProvider.SupportedTypes"/>.
/// </summary>
/// <typeparam name="T">The source type of the data object.</typeparam>
/// <typeparam name="TSerializable">The type of the serializable.</typeparam>
/// <remarks>
/// The data is sensitive, e.g. an account contains its password hash. The attribute is required,
/// because not every host requires an authorization for all of its controllers, e.g. the admin
/// panel of the distributed deployment also offers endpoints which are called by dapr.
/// </remarks>
[Authorize]
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
    /// <param name="dataSource">The data source.</param>
    public JsonDownloadController(IDataSource<T> dataSource)
    {
        this._dataSource = dataSource;
    }

    /// <summary>
    /// Gets the configuration with the specified id as json string.
    /// </summary>
    /// <param name="objectId">The identifier of the requested object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The requested configuration as json string.
    /// </returns>
    [HttpGet("[controller]_{objectId}.json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> GetConfigurationByIdAsync(Guid objectId, CancellationToken cancellationToken)
    {
        await this._dataSource.DiscardChangesAsync().ConfigureAwait(false);
        var owner = await this._dataSource.GetOwnerAsync(objectId, cancellationToken).ConfigureAwait(false);

        if (owner is IConvertibleTo<TSerializable> convertibleTo)
        {
            var resultStream = new MemoryStream();
            await convertibleTo.Convert().ToJsonAsync(resultStream, cancellationToken).ConfigureAwait(false);
            resultStream.Position = 0;
            if (GetExcludedProperties() is { Length: > 0 } excludedProperties)
            {
                // The properties are removed from the json and not from the converted object, because the
                // conversion may return the original object, e.g. with the in-memory persistence.
                resultStream = await RemovePropertiesAsync(resultStream, excludedProperties, cancellationToken).ConfigureAwait(false);
            }

            return this.File(resultStream, "application/json");
        }

        return this.NotFound();
    }

    /// <summary>
    /// Gets the properties which are left out of the download.
    /// </summary>
    /// <remarks>
    /// The credentials of an account don't belong into a file which may end up in a bug report or a backup
    /// folder. The password hash allows to crack the password offline, the other codes are stored in plain text.
    /// </remarks>
    private static string[] GetExcludedProperties()
    {
        if (typeof(T) == typeof(Account))
        {
            return [nameof(Account.PasswordHash), nameof(Account.SecurityCode), nameof(Account.VaultPassword)];
        }

        return [];
    }

    private static async ValueTask<MemoryStream> RemovePropertiesAsync(MemoryStream json, IEnumerable<string> propertyNames, CancellationToken cancellationToken)
    {
        using (json)
        {
            var node = await JsonNode.ParseAsync(json, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (node is JsonObject jsonObject)
            {
                foreach (var propertyName in propertyNames)
                {
                    jsonObject.Remove(propertyName);
                }
            }

            var result = new MemoryStream();
            using (var writer = new Utf8JsonWriter(result, new JsonWriterOptions { Indented = true }))
            {
                node?.WriteTo(writer);
            }

            result.Position = 0;
            return result;
        }
    }
}
