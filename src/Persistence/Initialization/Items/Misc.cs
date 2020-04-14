// <copyright file="Misc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Initializing of misc items which don't fit into the other categories.
    /// </summary>
    public class Misc : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Misc"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Misc(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            this.CreateLifeStone();
        }

        private void CreateLifeStone()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Life Stone";
            itemDefinition.Number = 11;
            itemDefinition.Group = 13;
            itemDefinition.DropLevel = 75;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            this.GameConfiguration.Items.Add(itemDefinition);
        }
    }
}
