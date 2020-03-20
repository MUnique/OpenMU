// <copyright file="JsonQueryBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using log4net;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// A query builder which creates a query to return a whole object graph as json by using postgres json functions.
    /// </summary>
    /// <remarks>
    /// TODO: Make the resulting query more readable by adding identing for subqueries.
    /// </remarks>
    public class JsonQueryBuilder
    {
        /// <summary>
        /// The logger of this class.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(JsonQueryBuilder));

        /// <summary>
        /// Builds the json query for the given entity type.
        /// </summary>
        /// <remarks>
        /// It creates a query like this (e.g. for GameConfiguration):
        /// select result."Id" "$id", result."Id", row_to_json(result) as GameConfiguration
        /// from (
        ///     select a."Id", a.*, (
        ///       -- additional subqueries which return json
        ///       ...)
        ///     from config."GameConfiguration" a
        /// ) result;.
        /// </remarks>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The query which returns the objects of the given type as json string.</returns>
        public string BuildJsonQueryForEntity(IEntityType entityType)
        {
            Log.Debug($"Building the json query for {entityType.Name}.");
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("select result.\"Id\" \"$id\", result.\"Id\" id, row_to_json(result) as ").AppendLine(entityType.GetTableName())
                .AppendLine("from (");
            this.AddTypeToQuery(entityType, stringBuilder, "a");
            stringBuilder.AppendLine(") result");
            var result = stringBuilder.ToString();

            if (Log.IsDebugEnabled)
            {
                Log.Debug($"Finished building the json query for {entityType.Name}. Result: {result}");
            }

            return result;
        }

        /// <summary>
        /// Gets the navigations of the entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The navigations of the entity type.</returns>
        /// <remarks>
        /// Can be overwritten to apply sorting.
        /// TODO: Can sorting based on dependencies be done automatically?.
        /// </remarks>
        protected virtual IEnumerable<INavigation> GetNavigations(IEntityType entityType)
        {
            return entityType.GetNavigations();
        }

        private void AddTypeToQuery(IEntityType entityType, StringBuilder stringBuilder, string alias)
        {
            stringBuilder.Append("select ").Append(alias).Append(".\"Id\" as \"$id\", ").Append(alias).Append(".*");
            this.AddNavigationsToQuery(entityType, stringBuilder, alias);
            stringBuilder.Append(" from ").Append(entityType.GetSchema()).Append(".").Append("\"").Append(entityType.GetTableName()).Append("\" ").AppendLine(alias);
        }

        private void AddNavigationsToQuery(IEntityType entityType, StringBuilder stringBuilder, string parentAlias)
        {
            var navigationAlias = this.GetNextAlias(parentAlias);
            if (navigationAlias == "i")
            {
                // stopping circular reference
                Log.DebugFormat("Stopping circular reference at entity type {0}", entityType.Name);
                return;
            }

            var navigations = this.GetNavigations(entityType);

            foreach (var navigation in navigations)
            {
                if (navigation.IsCollection())
                {
                    this.AddCollection(navigation, entityType, stringBuilder, parentAlias);
                }
                else //// it's a foreign key
                {
                    this.AddNavigation(navigation, stringBuilder, parentAlias);
                }
            }
        }

        private string GetNextAlias(string parentAlias)
        {
            return ((char)(parentAlias[0] + 1)).ToString();
        }

        private void AddNavigation(INavigation navigation, StringBuilder stringBuilder, string parentAlias)
        {
            if (navigation.ForeignKey.DeclaringEntityType != navigation.DeclaringEntityType)
            {
                // inverse property, no data required
                Log.DebugFormat("Inverse property {0}", navigation.Name);
                return;
            }

            var navigationAlias = this.GetNextAlias(parentAlias);
            var targetType = navigation.GetTargetType();
            var foreignKey = navigation.ForeignKey.Properties.First();
            if (foreignKey.IsShadowProperty())
            {
                // We assume that every important foreign key is mapped to a "real" property
                Log.DebugFormat("Shadow property {0}", navigation.Name);
                return;
            }

            var isBackReference = navigation.ForeignKey.PrincipalToDependent?.IsCollection() ?? false;
            if (isBackReference)
            {
                // It's a back reference of a collection - we just have to create a reference json object
                Log.DebugFormat("Back Reference property {0}", navigation.Name);
            }

            stringBuilder.Append(", (");
            if (!navigation.IsMemberOfAggregate() || isBackReference)
            {
                stringBuilder.Append("json_build_object('$ref', ").Append(parentAlias).Append(".\"").Append(foreignKey.Name).Append("\")");
            }
            else
            {
                stringBuilder.Append("select row_to_json(").Append(navigationAlias).Append(") from (");
                this.AddTypeToQuery(targetType, stringBuilder, navigationAlias);
                var primaryKey = targetType.FindPrimaryKey().Properties.First();
                stringBuilder.Append(") ").Append(navigationAlias).Append(" where ").Append(navigationAlias).Append(".\"").Append(primaryKey.GetColumnName()).Append("\" = ").Append(parentAlias).Append(".\"").Append(foreignKey.Name).AppendLine("\"");
            }

            stringBuilder.Append(") as \"").Append(navigation.Name).AppendLine("\"");
        }

        private void AddCollection(INavigation navigation, IEntityType entityType, StringBuilder stringBuilder, string parentAlias)
        {
            var keyProperty = navigation.ForeignKey.Properties.First();
            var navigationType = keyProperty.DeclaringEntityType;

            if (navigationType.FindDeclaredPrimaryKey().Properties.Count > 1)
            {
                this.AddManyToManyCollection(navigationType, entityType, stringBuilder, parentAlias, keyProperty);
            }
            else
            {
                this.AddOneToManyCollection(navigation, navigationType, stringBuilder, parentAlias, keyProperty);
            }

            stringBuilder.Append(") as \"").Append(navigation.Name.Replace("Joined", string.Empty)).AppendLine("\"");
        }

        private void AddOneToManyCollection(INavigation navigation, IEntityType navigationType, StringBuilder stringBuilder, string parentAlias, IProperty keyProperty)
        {
            var navigationAlias = this.GetNextAlias(parentAlias);
            var primaryKeyName = navigationType.FindDeclaredPrimaryKey().Properties[0].GetColumnName();
            stringBuilder.AppendLine(", (")
                .Append("select array_to_json(array_agg(row_to_json(").Append(navigationAlias).AppendLine("))) from (");

            if (navigation.IsMemberOfAggregate())
            {
                this.AddTypeToQuery(navigationType, stringBuilder, navigationAlias);
                stringBuilder.Append(") ").AppendLine(navigationAlias)
                    .Append("where ").Append(navigationAlias).Append(".\"").Append(keyProperty.Name).Append("\" = ").Append(parentAlias).Append(".\"").Append(primaryKeyName).AppendLine("\"");
            }
            else
            {
                var primaryKeyProperty = navigationType.FindPrimaryKey().Properties[0]; // It's always one property, usually called "Id"
                stringBuilder.Append("select \"").Append(primaryKeyProperty.GetColumnName()).AppendLine("\" as \"$ref\"");
                stringBuilder.Append("from ").Append(navigationType.GetSchema()).Append(".\"").Append(navigationType.GetTableName()).AppendLine("\" ")
                    .Append("where \"").Append(keyProperty.Name).Append("\" = ").Append(parentAlias).Append(".\"").Append(primaryKeyName).AppendLine("\"")
                    .Append(") as ").AppendLine(navigationAlias);
            }
        }

        /// <summary>
        /// Adds the many to many collection to the query.
        /// We assume that every many to many join entity only consists of the both foreign keys.
        /// Additionally we assume that every target entity is referencable.
        /// Therefore we just select the target entities as reference.
        /// When adding an object of the target type, our Ef-Core entity classes would automatically create join entities
        /// with the right keys, because they use <see cref="ManyToManyCollectionAdapter{T,TJoin}"/>s.
        /// </summary>
        /// <param name="navigationType">Type of the navigation, the join entity type.</param>
        /// <param name="entityType">Type of the entity which is currently getting processed. It holds the collection which is about to be added.</param>
        /// <param name="stringBuilder">The string builder which is used to create the query string.</param>
        /// <param name="parentAlias">The parent alias, required to reference the primary key of the <paramref name="entityType"/>.</param>
        /// <param name="keyProperty">The key property.</param>
        private void AddManyToManyCollection(IEntityType navigationType, IEntityType entityType, StringBuilder stringBuilder, string parentAlias, IProperty keyProperty)
        {
            var navigationAlias = this.GetNextAlias(parentAlias);
            var entityTypePrimaryKeyName = entityType.FindPrimaryKey().Properties.First().GetColumnName(); // usually "Id"
            var otherEntityTypeForeignKey = navigationType.GetForeignKeys().FirstOrDefault(fk => fk.PrincipalEntityType != entityType);
            var otherEntityTypeKey = navigationType.GetKeys().FirstOrDefault(fk => fk.DeclaringEntityType != entityType);
            var referenceColumnToOtherEntity = otherEntityTypeForeignKey?.Properties.First().GetColumnName() ?? otherEntityTypeKey?.Properties.First().GetColumnName();

            stringBuilder.AppendLine(", (")
                .Append("select array_to_json(array_agg(row_to_json(").Append(navigationAlias).AppendLine("))) from (");

            stringBuilder.Append("select \"").Append(referenceColumnToOtherEntity).AppendLine("\" as \"$ref\"")
                .Append("from ").Append(navigationType.GetSchema()).Append(".\"").Append(navigationType.GetTableName()).AppendLine("\" ")
                .Append("where ").Append("\"").Append(keyProperty.Name).Append("\" = ").Append(parentAlias).Append(".\"").Append(entityTypePrimaryKeyName).AppendLine("\"")
                .Append(") as ").AppendLine(navigationAlias);
        }
    }
}
