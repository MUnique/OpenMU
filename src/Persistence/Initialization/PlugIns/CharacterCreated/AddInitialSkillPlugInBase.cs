// <copyright file="AddInitialSkillPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Base class for a <see cref="ICharacterCreatedPlugIn"/> which adds an initial skill for a specific character class.
/// </summary>
public class AddInitialSkillPlugInBase : ICharacterCreatedPlugIn
{
    private readonly byte _characterClassNumber;
    private readonly ushort _skillNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddInitialSkillPlugInBase"/> class.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <param name="skillNumber">The skill number.</param>
    protected AddInitialSkillPlugInBase(byte characterClassNumber, ushort skillNumber)
    {
        this._characterClassNumber = characterClassNumber;
        this._skillNumber = skillNumber;
    }

    /// <inheritdoc />
    public void CharacterCreated(Player player, Character createdCharacter)
    {
        using var logScope = player.Logger.BeginScope(this.GetType());
        if (this._characterClassNumber != createdCharacter.CharacterClass?.Number)
        {
            player.Logger.LogDebug("Wrong character class {0}, expected {1}", createdCharacter.CharacterClass?.Number, this._characterClassNumber);
            return;
        }

        var skillDefinition =
            player.GameContext.Configuration.Skills.FirstOrDefault(s => s.Number == this._skillNumber);
        if (skillDefinition is null)
        {
            player.Logger.LogError($"Skill not found: {this._skillNumber}");
            return;
        }

        if (!skillDefinition.QualifiedCharacters.Contains(createdCharacter.CharacterClass))
        {
            player.Logger.LogError($"Skill {skillDefinition.Name} is not available for character class {createdCharacter.CharacterClass.Name}.");
            return;
        }

        if (createdCharacter.LearnedSkills.Any(entry => entry.Skill?.Number == skillDefinition.Number))
        {
            // This plug-in is not only called when a character is created, but also for characters which
            // were created outside the game (e.g. on the database or with the admin panel) and are missing
            // their inventory. Adding the skill again would give the character the same skill twice, which
            // its skill list can't handle.
            player.Logger.LogDebug("Skill {0} is already learned by character {1}.", skillDefinition.Name, createdCharacter.Name);
            return;
        }

        var skillEntry = player.PersistenceContext.CreateNew<SkillEntry>();
        skillEntry.Skill = skillDefinition;
        createdCharacter.LearnedSkills.Add(skillEntry);
    }
}