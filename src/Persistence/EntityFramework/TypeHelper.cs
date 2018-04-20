// <copyright file="TypeHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Helper class which offers functions related to the extended data model types.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// A cache which holds extended types (Value) for their corresponding base type (Key).
        /// </summary>
        private static readonly IDictionary<Type, Type> EfCoreTypes = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// Gets the ef core type of <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">Base type of the data model.</typeparam>
        /// <returns>Extended ef core type of <typeparamref name="TBase"/>.</returns>
        public static Type GetEfCoreTypeOf<TBase>()
        {
            if (!EfCoreTypes.TryGetValue(typeof(TBase), out Type efCoreType))
            {
                efCoreType = typeof(TypeHelper).Assembly.GetTypes().First(t => typeof(TBase).IsAssignableFrom(t));
                EfCoreTypes.Add(typeof(TBase), efCoreType);
            }

            return efCoreType;
        }

        /// <summary>
        /// Creates a new object of the extended ef core type of the <typeparamref name="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">The base type of the data model.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>A new object of the extended ef core type of the <typeparamref name="TBase"/>.</returns>
        public static TBase CreateNew<TBase>(params object[] args)
            where TBase : class
        {
            var efType = GetEfCoreTypeOf<TBase>();
            if (args.Length == 0)
            {
                return Activator.CreateInstance(efType) as TBase;
            }

            return Activator.CreateInstance(efType, args) as TBase;
        }
    }
}