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

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBotUseAction"/> class.
        /// </summary>
        /// <param name="player">the player.</param>
        public MuBotUseAction(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// send mu bot data back to the client.
        /// </summary>
        /// <param name="status">status to be set.</param>
        public void UseMuBot(byte status)
        {
            if (!MuBotConfiguration.IsEnabled)
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
            if (this.player.Level < MuBotConfiguration.MinLevel)
            {
                this.player.ShowMessage($"Mu Bot can be used after level {MuBotConfiguration.MinLevel}");
            }

            if (this.player.Level >= MuBotConfiguration.MaxLevel)
            {
                this.player.ShowMessage($"Mu Bot cannot be used after level {MuBotConfiguration.MaxLevel}");
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