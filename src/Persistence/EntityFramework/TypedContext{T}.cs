// <copyright file="TypedContext{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using MUnique.OpenMU.DataModel.Composition;

    /// <summary>
    /// A context which is used to show and edit instances of <typeparamref name="T" />.
    /// This context does not track and save data of other types, except direct object references which are marked with an <see cref="MemberOfAggregateAttribute" />.
    /// </summary>
    /// <typeparam name="T">The type which should be edited.</typeparam>
    internal class TypedContext<T> : EntityDataContext
    {
        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var types = modelBuilder.Model.GetEntityTypes().ToList();

            var editTypes = this.DetermineEditTypes(types);

            foreach (var type in editTypes)
            {
                modelBuilder.Entity(type.ClrType);
            }

            foreach (var type in types.Except(editTypes))
            {
                modelBuilder.Ignore(type.ClrType);
            }
        }

        private IEnumerable<IMutableEntityType> DetermineEditTypes(IList<IMutableEntityType> types)
        {
            var mainType = types.FirstOrDefault(met => met.ClrType == typeof(T))
                           ?? types.FirstOrDefault(met => met.ClrType.BaseType == typeof(T));
            if (mainType is null)
            {
                yield break;
            }

            yield return mainType;
            foreach (var navType in this.DetermineNavigationTypes(mainType))
            {
                yield return navType;
            }
        }

        private IEnumerable<IMutableEntityType> DetermineNavigationTypes(IMutableEntityType parentType)
        {
            var navigations = parentType.GetNavigations().Where(nav => nav.PropertyInfo is { });
            foreach (var navigation in navigations)
            {
                yield return navigation.TargetEntityType;

                if (navigation.IsMemberOfAggregate() || navigation.PropertyInfo.Name.StartsWith("Joined"))
                {
                    foreach (var navEditType in this.DetermineNavigationTypes(navigation.TargetEntityType))
                    {
                        yield return navEditType;
                    }
                }
            }
        }
    }
}
