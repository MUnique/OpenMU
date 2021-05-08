// <copyright file="GameMaster.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Initializes a game master account.
    /// </summary>
    internal class GameMaster : Level400
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMaster"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public GameMaster(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration, "testgm", 400)
        {
        }

        /// <inheritdoc />
        protected override Account CreateAccount()
        {
            var account = base.CreateAccount();
            account.State = AccountState.GameMaster;
            account.Characters.ForEach(c => c.CharacterStatus = CharacterStatus.GameMaster);
            return account;
        }
    }
}
