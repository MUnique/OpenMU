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
        //// private static readonly int[] rise = new int[] { 0, 3, 7, 10, 14, 17, 21, 24, 28, 31, 35, 40, 45, 50, 56, 63 };
        //// private static readonly int[] dmgadd = new int[] { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };

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