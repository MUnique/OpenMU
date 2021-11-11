// <copyright file="Quest150.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Initializer for an account to test the level 150 quests.
/// </summary>
internal class Quest150 : QuestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quest150"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Quest150(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "quest1", 150)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Quest150"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    public Quest150(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration, accountName, level)
    {
    }
}