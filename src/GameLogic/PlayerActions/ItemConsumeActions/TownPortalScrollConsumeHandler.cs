// -----------------------------------------------------------------------
// <copyright file="TownPortalScrollConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Consume handler for the town portal scroll.
    /// It warps the player to the nearest town.
    /// </summary>
    /// <remarks>
    /// We might need a field for the "nearest" town in the <see cref="GameMapDefinition"/>.
    /// <see cref="GameMapDefinition.SafezoneMap"/> might not be suitable.
    /// </remarks>
    public class TownPortalScrollConsumeHandler : BaseConsumeHandler
    {
        /// <inheritdoc />
        public override bool ConsumeItem(Player player, Item item, Item targetItem)
        {
            if (base.ConsumeItem(player, item, targetItem))
            {
                var targetMap = player.CurrentMap.Definition.SafezoneMap ?? player.SelectedCharacter.CharacterClass.HomeMap;
                var exitGate = targetMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
                player.WarpTo(exitGate);
                return true;
            }

            return false;
        }
    }
}
