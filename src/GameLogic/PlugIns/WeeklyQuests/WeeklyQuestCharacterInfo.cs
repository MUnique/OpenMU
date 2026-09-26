// <copyright file="WeeklyQuestCharacterInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The data of a character which decides which weekly quests are available for it.
/// </summary>
/// <param name="Class">The class of the character.</param>
/// <param name="Level">The level of the character.</param>
/// <param name="Resets">The reset count of the character.</param>
public sealed record WeeklyQuestCharacterInfo(CharacterClass? Class, int Level, int Resets);
