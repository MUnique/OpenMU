// <copyright file="CreateCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character;

using System.Text.RegularExpressions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to create a new character in the character selection screen.
/// </summary>
public class CreateCharacterAction
{
    /// <summary>
    /// Tries to create a new character.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="characterClassId">The character class identifier.</param>
    public async ValueTask CreateCharacterAsync(Player player, string characterName, int characterClassId)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.PlayerState.CurrentState != PlayerState.CharacterSelection)
        {
            player.Logger.LogError($"Account {player.Account!.LoginName} not in the right state, but {player.PlayerState.CurrentState}.");
            return;
        }

        var characterClass = player.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.Number == characterClassId);
        if (characterClass is not null)
        {
            var character = await this.CreateCharacterAsync(player, characterName, characterClass).ConfigureAwait(false);
            if (character != null)
            {
                await player.InvokeViewPlugInAsync<IShowCreatedCharacterPlugIn>(p => p.ShowCreatedCharacterAsync(character)).ConfigureAwait(false);
                return;
            }
        }

        await player.InvokeViewPlugInAsync<IShowCharacterCreationFailedPlugIn>(p => p.ShowCharacterCreationFailedAsync()).ConfigureAwait(false);
    }

    private async ValueTask<DataModel.Entities.Character?> CreateCharacterAsync(Player player, string name, CharacterClass characterClass)
    {
        var account = player.Account;
        if (account is null)
        {
            player.Logger.LogWarning("Account Object is null.");
            throw new ArgumentNullException(nameof(player));
        }

        player.Logger.LogDebug("Enter CreateCharacter: {0} {1} {2}", account.LoginName, name, characterClass);
        var isValidName = string.IsNullOrWhiteSpace(player.GameContext.Configuration.CharacterNameRegex) || Regex.IsMatch(name, player.GameContext.Configuration.CharacterNameRegex);
        player.Logger.LogDebug("CreateCharacter: Character Name matches = {0}", isValidName);
        if (!isValidName)
        {
            return null;
        }

        var freeSlot = this.GetFreeSlot(player);
        if (freeSlot is null)
        {
            return null;
        }

        if (!characterClass.CanGetCreated || characterClass.HomeMap is null)
        {
            return null;
        }

        var character = player.PersistenceContext.CreateNew<DataModel.Entities.Character>();
        character.CharacterClass = characterClass;
        character.Name = name;
        character.CharacterSlot = freeSlot.Value;
        character.CreateDate = DateTime.UtcNow;
        character.KeyConfiguration = new byte[30];
        var attributes = character.CharacterClass.StatAttributes.Select(a => player.PersistenceContext.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)).ToList();
        attributes.ForEach(character.Attributes.Add);
        character.CurrentMap = characterClass.HomeMap;
        var randomSpawnGate = character.CurrentMap!.ExitGates.Where(g => g.IsSpawnGate).SelectRandom();
        if (randomSpawnGate is not null)
        {
            character.PositionX = (byte)Rand.NextInt(randomSpawnGate.X1, randomSpawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(randomSpawnGate.Y1, randomSpawnGate.Y2);
        }

        character.Inventory = player.PersistenceContext.CreateNew<ItemStorage>();
        account.Characters.Add(character);
        player.GameContext.PlugInManager.GetPlugInPoint<ICharacterCreatedPlugIn>()?.CharacterCreated(player, character);
        try
        {
            await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            account.Characters.Remove(character);
            player.PersistenceContext.Detach(character);
            player.Logger.LogError(ex, "Error when trying to create character '{0}'", name);
            var message = ex.InnerException?.Message ?? ex.Message;
            if (message.Contains("IX_Character_Name") || message.Contains("23505"))
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("A character with the same name already exists.", MessageType.BlueNormal)).ConfigureAwait(false);
            }

            return null;
        }

        player.Logger.LogDebug("Creating Character Complete.");
        return character;
    }

    private byte? GetFreeSlot(Player player)
    {
        var usedSlots = player.Account!.Characters.Select(c => (int)c.CharacterSlot);
        var freeSlots = Enumerable.Range(0, player.GameContext.Configuration.MaximumCharactersPerAccount).Except(usedSlots).ToList();
        if (freeSlots.Any())
        {
            return (byte)freeSlots.First();
        }

        return null;
    }
}