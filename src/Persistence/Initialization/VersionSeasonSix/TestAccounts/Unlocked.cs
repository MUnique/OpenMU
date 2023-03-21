// <copyright file="Unlocked.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Initializes a test account with all character classes unlocked, so they can be created.
/// </summary>
internal class Unlocked : AccountInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Unlocked" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Unlocked(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration, "testunlock", 0)
    {
    }

    /// <inheritdoc />
    protected override Account CreateAccount()
    {
        var account = base.CreateAccount();
        account.State = AccountState.GameMaster;
        var unlockableClasses = this.GameConfiguration.CharacterClasses.Where(c => c.CanGetCreated && c.CreationAllowedFlag > 0);
        unlockableClasses.ForEach(account.UnlockedCharacterClasses.Add);

        return account;
    }
}