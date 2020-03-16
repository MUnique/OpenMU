// <copyright file="ByteArrayDownloadController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// API-Controller which returns data of a byte array of an object.
    /// </summary>
    [Route("download/{typeString}/{id:guid}/{propertyName}")]
    public class ByteArrayDownloadController : Controller
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayDownloadController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public ByteArrayDownloadController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Gets the data of the specified property of the specified object.
        /// </summary>
        /// <param name="typeString">The type string.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="propertyName">Name of the property.</param>
        [HttpGet]
        public Task GetAsync(string typeString, Guid id, string propertyName)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.StartsWith(nameof(MUnique)))
                .Select(assembly => assembly.GetType(typeString)).FirstOrDefault(t => t != null);
            if (type == null)
            {
                throw new InvalidOperationException($"Only types of namespace {nameof(MUnique)} can be edited on this page.");
            }

            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (property == null)
            {
                throw new ArgumentException($"Property {propertyName} not found in type {type}.", nameof(propertyName));
            }

            if (property.PropertyType != typeof(byte[]))
            {
                throw new ArgumentException($"Property {type}.{propertyName} is not a byte array.", nameof(propertyName));
            }

            var createContextMethod = typeof(IPersistenceContextProvider).GetMethod(nameof(IPersistenceContextProvider.CreateNewTypedContext)).MakeGenericMethod(type);
            using var persistenceContext = (IContext)createContextMethod.Invoke(this.persistenceContextProvider, Array.Empty<object>());

            var method = typeof(IContext).GetMethod(nameof(IContext.GetById)).MakeGenericMethod(type);
            var obj = method.Invoke(persistenceContext, new object[] { id });
            var array = (byte[])property.GetValue(obj);
            this.Response.ContentType = "application/octet-stream";
            return this.Response.Body.WriteAsync(array, 0, array.Length);
        }
    }
}
