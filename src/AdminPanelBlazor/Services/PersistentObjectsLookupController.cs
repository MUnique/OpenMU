// <copyright file="PersistentObjectsLookupController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A lookup controller which will return persistent objects which start with or contain the specified text.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.AdminPanelBlazor.Services.ILookupController" />
    public class PersistentObjectsLookupController : ILookupController
    {
        private readonly IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentObjectsLookupController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public PersistentObjectsLookupController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.context = persistenceContextProvider.CreateNewConfigurationContext();
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> GetSuggestionsAsync<T>(string text)
            where T : class
        {
            var values = this.context.Get<T>();
            if (string.IsNullOrEmpty(text))
            {
                return Task.FromResult(values);
            }

            return Task.FromResult(values.Where(v => v.GetName().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
                .Concat(values.Where(v => v.GetName().Contains(text, StringComparison.InvariantCultureIgnoreCase)))
                .Distinct());
        }
    }
}
