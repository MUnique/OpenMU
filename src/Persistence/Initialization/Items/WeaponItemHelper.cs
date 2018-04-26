// <copyright file="WeaponItemHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Helper class to create weapon item definitions.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.Items.ItemHelperBase" />
    internal class WeaponItemHelper : ItemHelperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponItemHelper" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configration.</param>
        public WeaponItemHelper(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Initializes the weapons.
        /// </summary>
        public void InitializeWeapons()
        {
            //// TODO: Create regex to convert file to code
            this.CreateSmallAxe();
        }

        private void CreateSmallAxe()
        {
            this.CreateItem(1, 0, 0, 0, 1, 3, true, "Small Axe", 1, 1, 6, 20, 18, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1);
        }
    }
}