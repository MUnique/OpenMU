// <copyright file="ByteArrayDownloadController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Persistence;

/// <summary>
/// API-Controller which returns data of a byte array of an object.
/// </summary>
[Route("download/{typeString}/{id:guid}/{propertyName}")]
public class ByteArrayDownloadController : Controller
{
    private readonly IPersistenceContextProvider _persistenceContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ByteArrayDownloadController"/> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    public ByteArrayDownloadController(IPersistenceContextProvider persistenceContextProvider)
    {
        this._persistenceContextProvider = persistenceContextProvider;
    }

    /// <summary>
    /// Gets the data of the specified property of the specified object.
    /// </summary>
    /// <param name="typeString">The type string.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="propertyName">Name of the property.</param>
    [HttpGet]
    public async Task<IActionResult> GetAsync(string typeString, Guid id, string propertyName)
    {
        var type = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName?.StartsWith(nameof(MUnique)) ?? false)
            .Select(assembly => assembly.GetType(typeString)).FirstOrDefault(t => t != null);
        if (type is null)
        {
            throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
        }

        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (property is null)
        {
            throw new ArgumentException($"Property {propertyName} not found in type {type}.", nameof(propertyName));
        }

        if (property.PropertyType != typeof(byte[]))
        {
            throw new ArgumentException($"Property {type}.{propertyName} is not a byte array.", nameof(propertyName));
        }

        using var persistenceContext = this._persistenceContextProvider.CreateNewTypedContext(type, false);

        var obj = await persistenceContext.GetByIdAsync(id, type).ConfigureAwait(false);
        if (obj is null)
        {
            return this.NotFound();
        }

        var array = (byte[]?)property.GetValue(obj);
        this.Response.ContentType = "application/octet-stream";
        if (array is null || array.Length == 0)
        {
            return this.NoContent();
        }

        await this.Response.Body.WriteAsync(array, 0, array.Length).ConfigureAwait(false);
        return this.Ok();
    }
}