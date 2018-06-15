// <copyright file="InitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Base class for an <see cref="IInitializer"/>.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.IInitializer" />
    public abstract class InitializerBase : IInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializerBase"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        protected InitializerBase(IContext context, GameConfiguration gameConfiguration)
        {
            this.Context = context;
            this.GameConfiguration = gameConfiguration;
        }

        /// <summary>
        /// Gets the persistence context.
        /// </summary>
        /// <value>
        /// The persistence context.
        /// </value>
        protected IContext Context { get; }

        /// <summary>
        /// Gets the game configuration.
        /// </summary>
        /// <value>
        /// The game configuration.
        /// </value>
        protected GameConfiguration GameConfiguration { get; }

        /// <inheritdoc />
        public abstract void Initialize();
    }
}
