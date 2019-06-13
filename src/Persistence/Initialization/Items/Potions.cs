// <copyright file="Potions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Class which contains item definitions for jewels.
    /// </summary>
    public class Potions : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Potions"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Potions(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Creates the potion item definitions.
        /// </summary>
        public override void Initialize()
        {
            this.GameConfiguration.Items.Add(this.CreateApple());
            this.GameConfiguration.Items.Add(this.CreateSmallHealingPotion());
            this.GameConfiguration.Items.Add(this.CreateMediumHealingPotion());
            this.GameConfiguration.Items.Add(this.CreateLargeHealingPotion());
            this.GameConfiguration.Items.Add(this.CreateSmallManaPotion());
            this.GameConfiguration.Items.Add(this.CreateMediumManaPotion());
            this.GameConfiguration.Items.Add(this.CreateLargeManaPotion());
            this.GameConfiguration.Items.Add(this.CreateSmallShieldPotion());
            this.GameConfiguration.Items.Add(this.CreateMediumShieldPotion());
            this.GameConfiguration.Items.Add(this.CreateLargeShieldPotion());
            this.GameConfiguration.Items.Add(this.CreateAlcohol());
            this.GameConfiguration.Items.Add(this.CreateAntidotePotion());
            this.GameConfiguration.Items.Add(this.CreateTownPortalScroll());
        }

        private ItemDefinition CreateAlcohol()
        {
            var alcohol = this.Context.CreateNew<ItemDefinition>();
            alcohol.Name = "Ale";
            alcohol.Number = 9;
            alcohol.Group = 14;
            alcohol.DropsFromMonsters = true;
            alcohol.DropLevel = 15;
            alcohol.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AlcoholConsumeHandler).FullName;
            alcohol.Durability = 1;
            alcohol.Value = 30;
            alcohol.Width = 1;
            alcohol.Height = 2;
            return alcohol;
        }

        /// <summary>
        /// Creates the apple definition.
        /// </summary>
        /// <returns>The created apple definition.</returns>
        private ItemDefinition CreateApple()
        {
            var apple = this.Context.CreateNew<ItemDefinition>();
            apple.Name = "Apple";
            apple.Number = 0;
            apple.Group = 14;
            apple.DropsFromMonsters = true;
            apple.DropLevel = 1;
            apple.MaximumItemLevel = 1;
            apple.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AppleConsumeHandler).FullName;
            apple.Durability = 3;
            apple.Value = 5;
            apple.Width = 1;
            apple.Height = 1;
            return apple;
        }

        /// <summary>
        /// Gets the small healing potion definition.
        /// </summary>
        /// <returns>The created small healing potion definition.</returns>
        private ItemDefinition CreateSmallHealingPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Small Healing Potion";
            potion.Number = 1;
            potion.Group = 14;
            potion.DropsFromMonsters = true;
            potion.MaximumItemLevel = 1;
            potion.DropLevel = 10;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallHealthPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Value = 10;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the medium healing potion definition.
        /// </summary>
        /// <returns>The created medium healing potion definition.</returns>
        private ItemDefinition CreateMediumHealingPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Medium Healing Potion";
            potion.Number = 2;
            potion.Group = 14;
            potion.DropsFromMonsters = true;
            potion.MaximumItemLevel = 1;
            potion.DropLevel = 25;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleHealthPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Value = 20;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the large healing potion definition.
        /// </summary>
        /// <returns>The created large healing definition.</returns>
        private ItemDefinition CreateLargeHealingPotion()
        {
            var definition = this.Context.CreateNew<ItemDefinition>();
            definition.Name = "Large Healing Potion";
            definition.Number = 3;
            definition.Group = 14;
            definition.DropsFromMonsters = true;
            definition.MaximumItemLevel = 1;
            definition.DropLevel = 40;
            definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigHealthPotionConsumeHandler).FullName;
            definition.Durability = 3;
            definition.Value = 30;
            definition.Width = 1;
            definition.Height = 1;
            return definition;
        }

        /// <summary>
        /// Gets the small mana potion definition.
        /// </summary>
        /// <returns>The created small mana potion definition.</returns>
        private ItemDefinition CreateSmallManaPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Small Mana Potion";
            potion.Number = 4;
            potion.Group = 14;
            potion.DropsFromMonsters = true;
            potion.MaximumItemLevel = 1;
            potion.DropLevel = 10;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallManaPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Value = 10;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the medium mana potion definition.
        /// </summary>
        /// <returns>The created medium mana potion definition.</returns>
        private ItemDefinition CreateMediumManaPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Medium Mana Potion";
            potion.Number = 5;
            potion.Group = 14;
            potion.DropsFromMonsters = true;
            potion.MaximumItemLevel = 1;
            potion.DropLevel = 25;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleManaPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Value = 20;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the large mana potion definition.
        /// </summary>
        /// <returns>The created large mana definition.</returns>
        private ItemDefinition CreateLargeManaPotion()
        {
            var definition = this.Context.CreateNew<ItemDefinition>();
            definition.Name = "Large Mana Potion";
            definition.Number = 6;
            definition.Group = 14;
            definition.DropsFromMonsters = true;
            definition.MaximumItemLevel = 1;
            definition.DropLevel = 40;
            definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigManaPotionConsumeHandler).FullName;
            definition.Durability = 3;
            definition.Value = 30;
            definition.Width = 1;
            definition.Height = 1;
            return definition;
        }

        /// <summary>
        /// Gets the small shield potion definition.
        /// </summary>
        /// <returns>The created small shield potion definition.</returns>
        private ItemDefinition CreateSmallShieldPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Small Shield Potion";
            potion.Number = 35;
            potion.Group = 14;
            potion.DropsFromMonsters = true;
            potion.DropLevel = 50;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallShieldPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the medium shield potion definition.
        /// </summary>
        /// <returns>The created medium shield potion definition.</returns>
        private ItemDefinition CreateMediumShieldPotion()
        {
            var potion = this.Context.CreateNew<ItemDefinition>();
            potion.Name = "Medium Shield Potion";
            potion.Number = 36;
            potion.Group = 14;
            potion.DropsFromMonsters = false;
            potion.DropLevel = 80;
            potion.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleShieldPotionConsumeHandler).FullName;
            potion.Durability = 3;
            potion.Width = 1;
            potion.Height = 1;
            return potion;
        }

        /// <summary>
        /// Gets the large shield potion definition.
        /// </summary>
        /// <returns>The created large shield definition.</returns>
        private ItemDefinition CreateLargeShieldPotion()
        {
            var definition = this.Context.CreateNew<ItemDefinition>();
            definition.Name = "Large Shield Potion";
            definition.Number = 37;
            definition.Group = 14;
            definition.DropsFromMonsters = false;
            definition.DropLevel = 100;
            definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigShieldPotionConsumeHandler).FullName;
            definition.Durability = 3;
            definition.Width = 1;
            definition.Height = 1;
            return definition;
        }

        /// <summary>
        /// Gets the antidote definition.
        /// </summary>
        /// <returns>The created antidote definition.</returns>
        private ItemDefinition CreateAntidotePotion()
        {
            var definition = this.Context.CreateNew<ItemDefinition>();
            definition.Name = "Antidote";
            definition.Number = 8;
            definition.Group = 14;
            definition.DropsFromMonsters = true;
            definition.DropLevel = 10;
            definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AntidoteConsumeHandler).FullName;
            definition.Durability = 3;
            definition.Value = 10;
            definition.Width = 1;
            definition.Height = 1;
            return definition;
        }

        private ItemDefinition CreateTownPortalScroll()
        {
            var definition = this.Context.CreateNew<ItemDefinition>();
            definition.Name = "Town Portal Scroll";
            definition.Number = 10;
            definition.Group = 14;
            definition.DropsFromMonsters = true;
            definition.DropLevel = 30;
            definition.ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.TownPortalScrollConsumeHandler).FullName;
            definition.Durability = 1;
            definition.Value = 30;
            definition.Width = 1;
            definition.Height = 2;
            return definition;
        }
    }
}
