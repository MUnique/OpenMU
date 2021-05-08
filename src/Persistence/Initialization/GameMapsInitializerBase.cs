// <copyright file="GameMapsInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Base class which initializes game maps.
    /// </summary>
    public abstract class GameMapsInitializerBase : InitializerBase, IGameMapsInitializer
    {
        private IList<IMapInitializer>? mapInitializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMapsInitializerBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        protected GameMapsInitializerBase(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Gets the map initializer types.
        /// </summary>
        protected abstract IEnumerable<Type> MapInitializerTypes { get; }

        /// <summary>
        /// Gets the map initializers.
        /// </summary>
        private IList<IMapInitializer> MapInitializers => this.mapInitializers ??= this.CreateInitializers();

        /// <inheritdoc />
        public override void Initialize()
        {
            foreach (var mapInitializer in this.MapInitializers)
            {
                mapInitializer.Initialize();
            }
        }

        /// <summary>
        /// Sets the safezone maps.
        /// Needs to be called after the context has been saved, otherwise referencing between the maps will fail.
        /// </summary>
        public void SetSafezoneMaps()
        {
            foreach (var mapInitializer in this.MapInitializers)
            {
                mapInitializer.SetSafezoneMap();
            }
        }

        private IList<IMapInitializer> CreateInitializers()
        {
            var parameters = new object[] { this.Context, this.GameConfiguration };
            return this.MapInitializerTypes
                .Select(type => type.GetConstructors().First().Invoke(parameters))
                .OfType<IMapInitializer>()
                .ToList();
        }
    }
}