// <copyright file="AccountJsonQueryBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// A query builder which builds a query to retrieve an account object.
    /// </summary>
    public class AccountJsonQueryBuilder : JsonQueryBuilder
    {
        /// <inheritdoc />
        /// <remarks>All config data is just referenced.</remarks>
        protected override bool SelectReferences(string parentAlias, INavigation navigation)
        {
            return navigation.GetTargetType().Relational().Schema == "config";
        }
    }
}