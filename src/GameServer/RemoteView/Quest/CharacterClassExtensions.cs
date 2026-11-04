// <copyright file="CharacterClassExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extension methods for <see cref="CharacterClass"/>.
/// </summary>
internal static class CharacterClassExtensions
{
    /// <summary>
    /// Gets the base character class.
    /// </summary>
    /// <param name="characterClass">The character class.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The base character class.</returns>
    internal static CharacterClass GetBaseClass(this CharacterClass characterClass, GameConfiguration gameConfiguration)
    {
        var previousClass = gameConfiguration.CharacterClasses.FirstOrDefault(c => c.NextGenerationClass == characterClass);
        if (previousClass is null)
        {
            return characterClass;
        }

        return previousClass.GetBaseClass(gameConfiguration);
    }
}