// <copyright file="CharacterAppearanceDataAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Adapter which implements <see cref="IAppearanceData"/> and takes a <see cref="Character"/>.
    /// </summary>
    internal class CharacterAppearanceDataAdapter : IAppearanceData
    {
        private readonly Character character;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterAppearanceDataAdapter"/> class.
        /// </summary>
        /// <param name="character">The character.</param>
        public CharacterAppearanceDataAdapter(Character character)
        {
            this.character = character;
        }

        /// <summary>
        /// Occurs when the appearance of the player changed.
        /// </summary>
        /// <remarks>This never happens in this implementation.</remarks>
        public event EventHandler AppearanceChanged;

        /// <inheritdoc/>
        public CharacterClass CharacterClass => this.character?.CharacterClass;

        /// <inheritdoc />
        public CharacterPose Pose => CharacterPose.Standing;

        /// <inheritdoc />
        public bool FullAncientSetEquipped => this.character.HasFullAncientSetEquipped();

        /// <inheritdoc />
        public IEnumerable<ItemAppearance> EquippedItems
        {
            get
            {
                if (this.character.Inventory != null)
                {
                    return this.character.Inventory.Items
                        .Where(item => item.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex)
                        .Select(item => item.GetAppearance());
                }

                return Enumerable.Empty<ItemAppearance>();
            }
        }
    }
}