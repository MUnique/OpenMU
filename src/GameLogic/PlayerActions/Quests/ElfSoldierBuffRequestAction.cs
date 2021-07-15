// <copyright file="ElfSoldierBuffRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Quests
{
    using System;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action of requesting the elf soldier buff.
    /// </summary>
    /// <remarks>
    /// Instead of hard-coding all this stuff, we could define something like a 'RequestableBuff' in the MonsterDefinition.
    /// </remarks>
    public class ElfSoldierBuffRequestAction
    {
        private static readonly short ElfSoldierNumber = 257;

        private static readonly MagicEffectDefinition BuffEffect = new ()
        {
            InformObservers = true,
            Name = "Elf Soldier Buff",
            Number = 3,
            StopByDeath = true,
        };

        /// <summary>
        /// Requests the buff and adds it to the <see cref="Player.MagicEffectList"/> when the player is allowed to get it.
        /// </summary>
        /// <param name="player">The player.</param>
        public void RequestBuff(Player player)
        {
            if (player.OpenedNpc is null
                || player.OpenedNpc.Definition.NpcWindow != NpcWindow.NpcDialog
                || player.OpenedNpc.Definition.Number != ElfSoldierNumber)
            {
                return;
            }

            if (player.Level > 220)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You're strong enough on your own.", MessageType.BlueNormal);
                return;
            }

            player.MagicEffectList.AddEffect(new MagicEffect(
                TimeSpan.FromMinutes(60),
                BuffEffect,
                new MagicEffect.ElementWithTarget(new ConstantElement(50 + (player.Level / 5)), Stats.DefenseBase),
                new MagicEffect.ElementWithTarget(new ConstantElement(45 + (player.Level / 3)), Stats.BaseDamageBonus)));
        }
    }
}
