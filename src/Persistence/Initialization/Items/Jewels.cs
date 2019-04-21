// <copyright file="Jewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Class which contains item definitions for jewels.
    /// </summary>
    public class Jewels : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Jewels"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Jewels(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            this.GameConfiguration.Items.Add(this.CreateJewelOfBless());
            this.GameConfiguration.Items.Add(this.CreateJewelOfSoul());
            this.GameConfiguration.Items.Add(this.CreateJewelOfLife());
            this.GameConfiguration.Items.Add(this.CreateJewelOfCreation());
            this.GameConfiguration.Items.Add(this.CreateJewelOfGuardian());
            this.GameConfiguration.Items.Add(this.CreateJewelOfHarmony());
            this.GameConfiguration.Items.Add(this.CreateGemstone());
            this.GameConfiguration.Items.Add(this.CreateJewelOfChaos());
            this.GameConfiguration.Items.Add(this.CreateLowerRefineStone());
            this.GameConfiguration.Items.Add(this.CreateHigherRefineStone());
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Bless'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Bless'.</returns>
        private ItemDefinition CreateJewelOfBless()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Bless";
            itemDefinition.Number = 13;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 25;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BlessJewelConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 150;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Bless'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Bless'.</returns>
        private ItemDefinition CreateJewelOfSoul()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Soul";
            itemDefinition.Number = 14;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 30;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SoulJewelConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 150;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Life'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Life'.</returns>
        private ItemDefinition CreateJewelOfLife()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Life";
            itemDefinition.Number = 16;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 72;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LifeJewelConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Creation'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Creation'.</returns>
        private ItemDefinition CreateJewelOfCreation()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Creation";
            itemDefinition.Number = 22;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 72;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LifeJewelConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Guardian'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Guardian'.</returns>
        private ItemDefinition CreateJewelOfGuardian()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Guardian";
            itemDefinition.Number = 31;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 75;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Gemstone'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Gemstone'.</returns>
        private ItemDefinition CreateGemstone()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Gemstone";
            itemDefinition.Number = 41;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = false;
            itemDefinition.DropLevel = 150;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 25;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Harmony'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Harmony'.</returns>
        private ItemDefinition CreateJewelOfHarmony()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Harmony";
            itemDefinition.Number = 42;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = false;
            itemDefinition.DropLevel = 150;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.HarmonyJewelConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 25;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Jewel of Chaos'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Jewel of Chaos'.</returns>
        private ItemDefinition CreateJewelOfChaos()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Jewel of Chaos";
            itemDefinition.Number = 15;
            itemDefinition.Group = 12;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.DropLevel = 12;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Lower refine stone'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Lower refine stone'.</returns>
        private ItemDefinition CreateLowerRefineStone()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Lower refine stone";
            itemDefinition.Number = 43;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = false;
            itemDefinition.DropLevel = 150;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.LowerRefineStoneConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 25;
            return itemDefinition;
        }

        /// <summary>
        /// Creates an <see cref="ItemDefinition"/> for the 'Higher refine stone'.
        /// </summary>
        /// <returns><see cref="ItemDefinition"/> for the 'Higher refine stone'.</returns>
        private ItemDefinition CreateHigherRefineStone()
        {
            var itemDefinition = this.Context.CreateNew<ItemDefinition>();
            itemDefinition.Name = "Higher refine stone";
            itemDefinition.Number = 44;
            itemDefinition.Group = 14;
            itemDefinition.DropsFromMonsters = false;
            itemDefinition.DropLevel = 150;
            itemDefinition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.HigherRefineStoneConsumeHandler).FullName;
            itemDefinition.Durability = 1;
            itemDefinition.Width = 1;
            itemDefinition.Height = 1;
            itemDefinition.Value = 25;
            return itemDefinition;
        }
    }
}
