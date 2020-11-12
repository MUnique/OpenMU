// <copyright file="MuBotUseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MuBot
{
    using MUnique.OpenMU.GameLogic.MuBot;

    /// <summary>
    /// Action to send back to the client the mu bot data.
    /// </summary>
    public class MuBotUseAction
    {
        private readonly Player player;
        private readonly MuBotConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBotUseAction"/> class.
        /// </summary>
        /// <param name="player">the player.</param>
        public MuBotUseAction(Player player)
        {
            this.player = player;
            this.configuration = player.GetMuBotConfiguration();
        }

        /// <summary>
        /// Toggle Mu Bot Status.
        /// </summary>
        /// <param name="status">status to be set.</param>
        public void UseMuBot(byte status)
        {
            if (this.configuration is null || !this.configuration.IsEnabled)
            {
                this.player.ShowMessage("Mu Bot is disabled");
                return;
            }

            switch (status)
            {
                case 0:
                    this.TurnOn();
                    break;
                case 1:
                    this.player.MuBot.Stop();
                    break;
                default: // unknown
                    this.player.ShowMessage($"Mu Bot Cannot Handle status: {status}");
                    break;
            }
        }

        private void TurnOn()
        {
            if (this.player.Level < this.configuration.MinLevel)
            {
                this.player.ShowMessage($"Mu Bot can be used after level {this.configuration.MinLevel}");
            }

            if (this.player.Level >= this.configuration.MaxLevel)
            {
                this.player.ShowMessage($"Mu Bot cannot be used after level {this.configuration.MaxLevel}");
                return;
            }

            if (!this.player.MuBot.CanAfford())
            {
                this.player.ShowMessage($"Mu Bot requires {this.player.MuBot.GetRequiredMoney()} zen");
                return;
            }

            this.player.MuBot.Start();
        }
    }
}
