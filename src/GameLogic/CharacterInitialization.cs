// <copyright file="CharacterInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Text.RegularExpressions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Initializes the data of newly created characters, so that they are playable right away.
/// </summary>
/// <remarks>
/// It's shared by every place which creates characters - the character creation in the game,
/// the bots and the admin panel - so that they all end up with the same data.
/// </remarks>
public static class CharacterInitialization
{
    /// <summary>
    /// Determines whether the name is valid for a character, according to the <see cref="GameConfiguration.CharacterNameRegex"/>.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <param name="name">The name of the character.</param>
    /// <returns><c>true</c>, if the name is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidCharacterName(this GameConfiguration configuration, string name)
    {
        return string.IsNullOrWhiteSpace(configuration.CharacterNameRegex)
               || Regex.IsMatch(name, configuration.CharacterNameRegex);
    }

    /// <summary>
    /// Gets the first character slot of the account which isn't used by a character.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="configuration">The game configuration, which defines the available slots.</param>
    /// <returns>The free slot; or <c>null</c>, if all slots are used.</returns>
    public static byte? GetFreeCharacterSlot(this Account account, GameConfiguration configuration)
    {
        var usedSlots = account.Characters.Select(c => (int)c.CharacterSlot);
        return Enumerable.Range(0, configuration.MaximumCharactersPerAccount)
            .Except(usedSlots)
            .Select(slot => (byte?)slot)
            .FirstOrDefault();
    }

    /// <summary>
    /// Initializes a newly created character of the specified class: its stat attributes with the
    /// base values of the class, the home map with a random spawn position, an empty inventory and
    /// the default key configuration.
    /// </summary>
    /// <param name="context">The persistence context which creates the dependent objects.</param>
    /// <param name="character">The new character.</param>
    /// <param name="characterClass">The class of the character.</param>
    public static void InitializeNewCharacter(this IContext context, Character character, CharacterClass characterClass)
    {
        character.CharacterClass = characterClass;
        character.CreateDate = DateTime.UtcNow;
        character.KeyConfiguration = CreateDefaultKeyConfiguration();

        // Distinct, because a character class may define the same stat attribute more than once (data
        // which got duplicated by an update); a character must never hold an attribute twice.
        foreach (var attribute in characterClass.StatAttributes
                     .DistinctBy(a => a.Attribute)
                     .Where(a => character.Attributes.All(existing => existing.Definition != a.Attribute))
                     .Select(a => context.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
        {
            character.Attributes.Add(attribute);
        }

        character.CurrentMap = characterClass.HomeMap;
        if (character.CurrentMap?.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is { } spawnGate)
        {
            character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
        }

        character.Inventory ??= context.CreateNew<ItemStorage>();
    }

    /// <summary>
    /// Creates the default key configuration for a newly created character.
    /// </summary>
    /// <returns>The default key configuration.</returns>
    /// <remarks>
    /// The key configuration is an opaque blob which is interpreted by the game client. Within it,
    /// the potion quick-slots Q, W, E and R are stored as offsets into the potion item group, at
    /// byte indices 21 (Q), 22 (W), 23 (E) and 25 (R). We bind Q to the healing potion and W to the
    /// mana potion; E and R stay unbound. An all-zero configuration would otherwise make the client
    /// bind offset 0 (the apple, which it treats as a healing item) to all four slots, so each one
    /// would act as a health potion.
    /// </remarks>
    public static byte[] CreateDefaultKeyConfiguration()
    {
        const byte healingPotion = 1;
        const byte manaPotion = 4;
        const byte unbound = 0xFF;

        var keyConfiguration = new byte[30];
        keyConfiguration[21] = healingPotion; // Q
        keyConfiguration[22] = manaPotion; // W
        keyConfiguration[23] = unbound; // E
        keyConfiguration[25] = unbound; // R
        return keyConfiguration;
    }
}
