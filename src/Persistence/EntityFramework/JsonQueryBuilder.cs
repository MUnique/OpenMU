// <copyright file="JsonQueryBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// A query builder which creates a query to return a whole object graph as json by using postgres json functions.
    /// </summary>
    public class JsonQueryBuilder
    {
        private ISet<IEntityType> addedTypes = new HashSet<IEntityType>();

        /// <summary>
        /// Builds the json query for the given entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The query which returns the objects of the given type as json string.</returns>
        public string BuildJsonQueryForEntity(IEntityType entityType)
        {
            this.addedTypes.Clear();
            var relational = entityType.Relational();
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT row_to_json(result) as ").AppendLine(relational.TableName)
                .AppendLine("from (");
            this.AddTypeToQuery(entityType, stringBuilder, "a");
            stringBuilder.AppendLine(") result;");

            return stringBuilder.ToString();
        }

        private void AddTypeToQuery(IEntityType entityType, StringBuilder stringBuilder, string alias)
        {
            stringBuilder.Append("select ").Append(alias).AppendLine(".*");
            this.addedTypes.Add(entityType);
            this.AddNavigationsToQuery(entityType, stringBuilder, alias);
            var relational = entityType.Relational();
            stringBuilder.Append(" from ").Append(relational.Schema).Append(".").Append("\"").Append(relational.TableName).Append("\" ").AppendLine(alias);
        }

        private void AddNavigationsToQuery(IEntityType entityType, StringBuilder stringBuilder, string parentAlias)
        {
            var navigationAlias = ((char)(parentAlias[0] + 1)).ToString();
            if (navigationAlias == "i")
            {
                // stopping circular reference
                return;
            }

            foreach (var navigation in entityType.GetNavigations())
            {
                if (navigation.IsCollection())
                {
                    var keyProperty = navigation.ForeignKey.Properties.First();
                    var navigationType = keyProperty.DeclaringEntityType;
                    var primaryKeyName = navigationType.FindDeclaredPrimaryKey().Properties[0].Relational().ColumnName;

                    if (navigationType.FindDeclaredPrimaryKey().Properties.Count() > 1)
                    {
                        continue;
                    }

                    stringBuilder.AppendLine(", (")
                        .AppendLine("select array_to_json(array_agg(row_to_json(").Append(navigationAlias).Append(")))")
                        .Append("from (");

                    if (this.addedTypes.Contains(navigationType))
                    {
                        // just select the Id properties
                        bool first = true;
                        foreach (var primaryKey in navigationType.FindPrimaryKey().Properties)
                        {
                            if (!first)
                            {
                                stringBuilder.Append(", ");
                            }
                            else
                            {
                                stringBuilder.Append("select ");
                            }

                            stringBuilder.Append("\"").Append(primaryKey.Relational().ColumnName).Append("\"");
                            first = false;
                        }

                        var navigationRelational = navigationType.Relational();
                        stringBuilder.AppendLine(string.Empty);
                        stringBuilder.Append("from ")
                            .Append(navigationRelational.Schema).Append(".\"").Append(navigationRelational.TableName).Append("\" ")
                            .Append("where ").Append("\"").Append(keyProperty.Name).Append("\" = ").Append(parentAlias).Append(".\"").Append(primaryKeyName).AppendLine("\"")
                            .Append(") as ").AppendLine(navigationAlias)
                            .Append(") as \"").Append(navigation.Name).AppendLine("\"");
                    }
                    else
                    {
                        this.AddTypeToQuery(navigationType, stringBuilder, navigationAlias);
                        stringBuilder.Append(") ").Append(navigationAlias).Append(" where ").Append(navigationAlias).Append(".\"").Append(keyProperty.Name).Append("\" = ").Append(parentAlias).Append(".\"").Append(primaryKeyName).AppendLine("\"")
                            .Append(") as \"").Append(navigation.Name).AppendLine("\"");
                    }
                }
                else //// it's a foreign key
                {
                    if (navigation.ForeignKey.DeclaringEntityType != navigation.DeclaringEntityType)
                    {
                        // inverse property
                        continue;
                    }

                    var targetType = navigation.GetTargetType();
                    if (targetType.FindPrimaryKey().Properties.Count() > 1)
                    {
                        // many to many?
                        continue;
                    }

                    if (this.addedTypes.Contains(targetType))
                    {
                        continue;
                    }

                    stringBuilder.AppendLine(", (")
                        .AppendLine("select row_to_json(").Append(navigationAlias).Append(")")
                        .Append("from (");

                    var primaryKey = targetType.FindPrimaryKey().Properties.First();
                    var foreignKey = navigation.ForeignKey.Properties.First();
                    this.AddTypeToQuery(targetType, stringBuilder, navigationAlias);
                    stringBuilder.Append(") ").Append(navigationAlias).Append(" where ").Append(navigationAlias).Append(".\"").Append(primaryKey.Relational().ColumnName).Append("\" = ").Append(parentAlias).Append(".\"").Append(foreignKey.Name).AppendLine("\"")
                        .Append(") as \"").Append(navigation.Name).AppendLine("\"");
                }
            }
        }
    }
}
