// <copyright file="CreateCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.Character;

    /// <summary>
    /// Action to create a new character in the character selection screen.
    /// </summary>
    public class CreateCharacterAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CreateCharacterAction));

        /// <summary>
        /// Tries to create a new character.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="characterClassId">The character class identifier.</param>
        public void CreateCharacter(Player player, string characterName, int characterClassId)
        {
            if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
            {
                Log.Error($"Account {player.Account.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
                return;
            }

            CharacterClass characterClass = player.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.Number == characterClassId);
            if (characterClass != null)
            {
                var character = this.CreateCharacter(player, characterName, characterClass);
                if (character != null)
                {
                    player.ViewPlugIns.GetPlugIn<IShowCreatedCharacterPlugIn>()?.ShowCreatedCharacter(character);
                }
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IShowCharacterCreationFailedPlugIn>()?.ShowCharacterCreationFailed();
            }
        }

        private Character CreateCharacter(Player player, string name, CharacterClass charclass)
        {
            var account = player.Account;
            if (account == null)
            {
                Log.Warn("Account Object is null.");
                throw new ArgumentNullException(nameof(player), "Account Object is null.");
            }

            Log.DebugFormat("Enter CreateCharacter: {0} {1} {2}", account.LoginName, name, charclass);
            var isValidName = Regex.IsMatch(name, player.GameContext.Configuration.CharacterNameRegex);
            Log.DebugFormat("CreateCharacter: Character Name matches = {0}", isValidName);
            if (!isValidName)
            {
                return null;
            }

            var freeSlot = this.GetFreeSlot(player);
            if (freeSlot == null)
            {
                return null;
            }

            if (!charclass.CanGetCreated)
            {
                return null;
            }

            var character = player.PersistenceContext.CreateNew<Character>();
            character.CharacterClass = charclass;
            character.Name = name;
            character.CharacterSlot = freeSlot.Value;
            character.CreateDate = DateTime.Now;
            character.KeyConfiguration = new byte[30];
            var attributes = character.CharacterClass.StatAttributes.Select(a => player.PersistenceContext.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)).ToList();
            attributes.ForEach(character.Attributes.Add);
            character.CurrentMap = charclass.HomeMap;
            var randomSpawnGate = character.CurrentMap.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
            character.PositionX = (byte)Rand.NextInt(randomSpawnGate.X1, randomSpawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(randomSpawnGate.Y1, randomSpawnGate.Y2);
            character.Inventory = player.PersistenceContext.CreateNew<ItemStorage>();
            account.Characters.Add(character);
            player.GameContext.PlugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()?.CharacterCreated(player, character);
            Log.Debug("Creating Character Complete.");
            return character;
        }

        private byte? GetFreeSlot(Player player)
        {
            var usedSlots = player.Account.Characters.Select(c => (int)c.CharacterSlot);
            var freeSlots = Enumerable.Range(0, player.GameContext.Configuration.MaximumCharactersPerAccount).Except(usedSlots).ToList();
            if (freeSlots.Any())
            {
                return (byte)freeSlots.First();
            }

            return null;
        }
    }
}
