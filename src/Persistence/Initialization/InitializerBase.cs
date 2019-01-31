// <copyright file="InitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;

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

        /// <summary>
        /// Creates an attribute requirement with the specified minimum value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <returns>The created requirement.</returns>
        protected AttributeRequirement CreateRequirement(AttributeDefinition attribute, int minimumValue)
        {
            var requirement = this.Context.CreateNew<AttributeRequirement>();
            requirement.Attribute = attribute.GetPersistent(this.GameConfiguration);
            requirement.MinimumValue = minimumValue;
            return requirement;
        }

        /// <summary>
        /// Creates an item requirement, if the required value is greater than 0.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="requiredValue">The required value.</param>
        protected void CreateItemRequirementIfNeeded(ItemDefinition item, AttributeDefinition attribute, int requiredValue)
        {
            if (requiredValue == 0)
            {
                return;
            }

            var requirement = this.CreateRequirement(attribute, requiredValue);
            item.Requirements.Add(requirement);
        }
    }
}
