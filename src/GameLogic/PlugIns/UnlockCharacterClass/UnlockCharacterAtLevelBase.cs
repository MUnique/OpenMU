// <copyright file="UnlockCharacterAtLevelBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.UnlockCharacterClass;

/// <summary>
/// Unlocks the a character class as soon as the first character of an account reaches a certain level.
/// </summary>
public abstract class UnlockCharacterAtLevelBase : ICharacterLevelUpPlugIn
{
    private readonly byte _classNumber;
    private readonly int _minimumLevel;
    private readonly string _warningMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnlockCharacterAtLevelBase"/> class.
    /// </summary>
    /// <param name="classNumber">The class number.</param>
    /// <param name="minimumLevel">The minimum level.</param>
    /// <param name="className">Name of the class.</param>
    protected UnlockCharacterAtLevelBase(byte classNumber, int minimumLevel, string className)
    {
        this._classNumber = classNumber;
        this._minimumLevel = minimumLevel;
        this._warningMessage = $"Couldn't unlock {className}, because it couldn't be found in the configuration.";
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (player.Level >= this._minimumLevel
            && player.Account is { } account
            && account.UnlockedCharacterClasses.All(c => c.Number != this._classNumber))
        {
            var unlockedClass = player.GameContext.Configuration.CharacterClasses.FirstOrDefault(c => c.Number == this._classNumber);
            if (unlockedClass is null)
            {
                player.Logger.LogWarning(this._warningMessage);
                return;
            }

            account.UnlockedCharacterClasses.Add(unlockedClass);
        }
    }
}