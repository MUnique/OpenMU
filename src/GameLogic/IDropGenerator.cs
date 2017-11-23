// <copyright file="IDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The interface for a drop generator.
    /// </summary>
    public interface IDropGenerator
    {
        /// <summary>
        /// Gets the item drops which are generated when a monster got killed by a player.
        /// If money is dropped, it is added to the players inventory automatically. If the
        /// player is in a party, the money is split between all players.
        /// </summary>
        /// <param name="monster">The monster which got killed.</param>
        /// <param name="gainedExperience">The experience which the player gained form the kill (relevant for the money drop).</param>
        /// <param name="player">The player who killed the monster.</param>
        /// <returns>The item drops which are generated when a monster got killed by a player.</returns>
        IEnumerable<Item> GetItemDropsOrAddMoney(MonsterDefinition monster, int gainedExperience, Player player);
    }

    /// <summary>
    /// The default drop generator.
    /// </summary>
    public class DefaultDropGenerator : IDropGenerator
    {
        /// <summary>
        /// The amoun of money which is dropped at least, and added to the gained experience.
        /// </summary>
        public const int BaseMoneyDrop = 7;

        private readonly IEnumerable<DropItemGroup> baseGroups;

        private readonly IRandomizer randomizer;

        private readonly IList<ItemDefinition> ancientItems;

        private readonly IList<ItemDefinition> droppableItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDropGenerator" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="randomizer">The randomizer.</param>
        public DefaultDropGenerator(GameConfiguration config, IRandomizer randomizer)
        {
            this.baseGroups = config.BaseDropItemGroups;
            this.randomizer = randomizer;
            this.droppableItems = config.Items.Where(i => i.DropsFromMonsters).ToList();
            this.ancientItems = this.droppableItems.Where(i => i.PossibleItemSetGroups.Any(o => o.Items.Any(n => n.BonusOption.OptionType == ItemOptionTypes.AncientBonus))).ToList();
        }

        /// <inheritdoc/>
        public IEnumerable<Item> GetItemDropsOrAddMoney(MonsterDefinition monster, int gainedExperience, Player player)
        {
            var character = player.SelectedCharacter;
            var map = player.CurrentMap.Definition;
            var dropGroups =
                this.CombineDropGroups(monster.DropItemGroups, character.DropItemGroups, map.DropItemGroups)
                    .OrderBy(group => group.Chance);

            for (int i = 0; i < monster.NumberOfMaximumItemDrops; i++)
            {
                var group = this.SelectRandomGroup(dropGroups);
                if (group == null)
                {
                    continue;
                }

                if (group.ItemType == SpecialItemType.Money)
                {
                    // Apply zen value: exp + 7
                    var money = gainedExperience + BaseMoneyDrop;
                    var party = player.Party;
                    if (party == null)
                    {
                        player.TryAddMoney((int)(money * player.Attributes[Stats.MoneyAmountRate]));
                    }
                    else
                    {
                        var players = party.PartyList.OfType<Player>().Where(p => p.CurrentMap == player.CurrentMap && !p.IsAtSafezone()).ToList();
                        var moneyPart = money / players.Count;
                        players.ForEach(p => p.TryAddMoney((int)(moneyPart * p.Attributes[Stats.MoneyAmountRate])));
                    }
                }
                else
                {
                    var item = this.GetItemDrop(monster, group);
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a random item.
        /// </summary>
        /// <param name="monsterLvl">The monster level.</param>
        /// <param name="socketItems">if set to <c>true</c> [socket items].</param>
        /// <returns>A random item.</returns>
        protected Item GetRandomItem(int monsterLvl, bool socketItems)
        {
            var possible = this.GetPossibleList(monsterLvl).ToList();
            if (!possible.Any())
            {
                return null;
            }

            var itemDef = possible.ElementAt(this.randomizer.NextInt(0, possible.Count));
            var item = new TemporaryItem();
            item.Definition = itemDef;
            item.Level = (byte)((monsterLvl - itemDef.DropLevel) / 3);
            this.ApplyRandomOptions(item);
            return item;
        }

        /// <summary>
        /// Applies random options to the item.
        /// </summary>
        /// <param name="item">The item.</param>
        protected void ApplyRandomOptions(Item item)
        {
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            foreach (var option in item.Definition.PossibleItemOptions.Where(o => o.AddsRandomly))
            {
                for (int i = 0; i < option.MaximumOptionsPerItem; i++)
                {
                    if (this.randomizer.NextRandomBool(option.AddChance))
                    {
                        var newOption = option.PossibleOptions.SelectRandom(this.randomizer);
                        var itemOptionLink = new ItemOptionLink();
                        itemOptionLink.ItemOption = newOption;
                        itemOptionLink.Level = 1;
                        item.ItemOptions.Add(itemOptionLink);
                    }
                }
            }

            if (item.Definition.MaximumSockets > 0)
            {
                item.SocketCount = this.randomizer.NextInt(1, item.Definition.MaximumSockets + 1);
            }
        }

        /// <summary>
        /// Gets a random excellent item.
        /// </summary>
        /// <param name="monsterLvl">The monster level.</param>
        /// <returns>A random excellent item.</returns>
        protected Item GetRandomExcellentItem(int monsterLvl)
        {
            var possible = this.GetPossibleList(monsterLvl - 25).ToList();
            if (!possible.Any())
            {
                return null;
            }

            var itemDef = possible.SelectRandom(this.randomizer);
            var item = new TemporaryItem();
            item.Definition = itemDef;
            this.ApplyRandomOptions(item);
            if (itemDef.Skill != null && item.Definition.QualifiedCharacters.Any())
            {
                item.HasSkill = true; // every excellent item got skill
            }

            this.AddRandomExcOptions(item);
            return item;
        }

        /// <summary>
        /// Gets a random ancient item.
        /// </summary>
        /// <returns>A random ancient item.</returns>
        protected Item GetRandomAncient()
        {
            Item item = new TemporaryItem();
            item.Definition = this.ancientItems.SelectRandom(this.randomizer);
            this.ApplyRandomOptions(item);
            var itemDef = item.Definition;
            if (itemDef.Skill != null && item.Definition.QualifiedCharacters.Any())
            {
                item.HasSkill = true;
            }

            var ancientSet = item.ItemSetGroups.Where(g => g.Options.Any(o => o.OptionType == ItemOptionTypes.AncientOption)).SelectRandom(this.randomizer);
            item.ItemSetGroups.Add(ancientSet);
            var bonusOption = ancientSet.Items.First(i => i.ItemDefinition == item.Definition).BonusOption; // for example: +5str or +10str
            var bonusOptionLink = new ItemOptionLink();
            bonusOptionLink.ItemOption = bonusOption;
            bonusOptionLink.Level = bonusOption.LevelDependentOptions.Select(o => o.Level).SelectRandom();
            item.ItemOptions.Add(bonusOptionLink);
            return item;
        }

        private void AddRandomExcOptions(Item item)
        {
            var possibleItemOptions = item.Definition.PossibleItemOptions;
            var excellentOptions = possibleItemOptions.FirstOrDefault(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent));
            if (excellentOptions == null)
            {
                return;
            }

            for (int i = item.ItemOptions.Count(o => o.ItemOption.OptionType == ItemOptionTypes.Excellent); i < excellentOptions.MaximumOptionsPerItem; i++)
            {
                if (i == 0)
                {
                    var itemOptionLink = new ItemOptionLink();
                    itemOptionLink.ItemOption = excellentOptions.PossibleOptions.SelectRandom(this.randomizer);
                    item.ItemOptions.Add(itemOptionLink);
                    continue;
                }

                if (this.randomizer.NextRandomBool(excellentOptions.AddChance))
                {
                    ItemOption option = excellentOptions.PossibleOptions.SelectRandom(this.randomizer);
                    while (item.ItemOptions.Any(o => o.ItemOption == option))
                    {
                        option = excellentOptions.PossibleOptions.SelectRandom(this.randomizer);
                    }

                    var itemOptionLink = new ItemOptionLink();
                    itemOptionLink.ItemOption = option;
                    item.ItemOptions.Add(itemOptionLink);
                }
            }
        }

        private IEnumerable<DropItemGroup> CombineDropGroups(
            IEnumerable<DropItemGroup> monsterGroup,
            IEnumerable<DropItemGroup> characterGroup,
            IEnumerable<DropItemGroup> mapGroup)
        {
            IEnumerable<DropItemGroup> dropGroups = this.baseGroups ?? Enumerable.Empty<DropItemGroup>();
            if (monsterGroup != null)
            {
                dropGroups = dropGroups.Concat(monsterGroup);
            }

            if (characterGroup != null)
            {
                dropGroups = dropGroups.Concat(characterGroup);
            }

            if (mapGroup != null)
            {
                dropGroups = dropGroups.Concat(mapGroup);
            }

            return dropGroups;
        }

        private Item GetItemDrop(MonsterDefinition monster, DropItemGroup selectedGroup)
        {
            if (selectedGroup != null)
            {
                if (selectedGroup.PossibleItems?.Count > 0)
                {
                    var item = new TemporaryItem();
                    item.Definition = selectedGroup.PossibleItems.SelectRandom(this.randomizer);
                    this.ApplyRandomOptions(item);
                    return item;
                }

                switch (selectedGroup.ItemType)
                {
                    case SpecialItemType.Ancient:
                        return this.GetRandomAncient();
                    case SpecialItemType.Excellent:
                        return this.GetRandomExcellentItem((int)monster[Stats.Level]);
                    case SpecialItemType.RandomItem:
                        return this.GetRandomItem((int)monster[Stats.Level], false);
                    case SpecialItemType.SocketItem:
                        return this.GetRandomItem((int)monster[Stats.Level], true);
                }
            }

            return null;
        }

        private DropItemGroup SelectRandomGroup(IEnumerable<DropItemGroup> dropGroups)
        {
            double lot = this.randomizer.NextDouble();
            foreach (var group in dropGroups)
            {
                if (lot > group.Chance)
                {
                    lot -= group.Chance;
                }
                else
                {
                    return group;
                }
            }

            return null;
        }

        private IEnumerable<ItemDefinition> GetPossibleList(int monsterLevel)
        {
            ////TODO: Optimize this by using a sorted list and a binary search to find the starting point. Maybe even build up a cache?
            return from it in this.droppableItems
                    where (it.DropLevel <= monsterLevel)
                    && (it.DropLevel > monsterLevel - 12)
                    select it;
        }
    }
}
