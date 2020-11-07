namespace MUnique.OpenMU.GameLogic.PlayerActions.MuBot
{
    using System;
    using MUnique.OpenMU.GameLogic.MuBot;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.MuBot;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to send back to the client the mu bot data
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
            //this event is triggering always
            player.MuBot.MoneyCollected += this.SendCollectedMoney;
        }

        /// <summary>
        /// send mu bot data back to the client
        /// </summary>
        /// <param name="status">status to be set.</param>
        public void UseMuBot(byte status)
        {
            if (!MuBotConfiguration.IsEnabled)
            {
                this.ShowMessage("Mu Bot is disabled");
                return;
            }

            switch (status)
            {
                case 0:
                    this.TurnOn();
                    break;
                case 1:
                    this.TurnOff();
                    break;
                default: // unknown
                    this.ShowMessage($"Mu Bot Cannot Handle status: {status}");
                    break;
            }
        }

        private void TurnOn()
        {
            if (this.player.Level < MuBotConfiguration.MinLevel)
            {
                this.ShowMessage($"Mu Bot can be used after level {MuBotConfiguration.MinLevel}");
            }

            if (this.player.Level >= MuBotConfiguration.MaxLevel)
            {
                this.ShowMessage($"Mu Bot cannot be used after level {MuBotConfiguration.MaxLevel}");
                return;
            }

            if (!this.player.MuBot.CanAfford())
            {
                this.ShowMessage($"Mu Bot requires {this.player.MuBot.GetRequiredMoney()} zen");
                return;
            }

            this.player.MuBot.Start();
            this.ToggleMuBot(0);
        }

        private void TurnOff()
        {
            this.player.MuBot.Stop();
            this.ToggleMuBot(1);
        }

        /// <summary>
        /// Send collected money to the client.
        /// </summary>
        /// <param name="sender">mu bot instance.</param>
        /// <param name="e">collected money arguments.</param>
        private void SendCollectedMoney(object sender, MoneyCollectedEventArgs e)
        {
            this.ToggleMuBot(0, e.Amount);
        }

        private void ShowMessage(string message)
        {
            this.player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.BlueNormal);
        }

        private void ToggleMuBot(byte status, int money = 0)
        {
            this.player.ViewPlugIns.GetPlugIn<IMuBotUseResponse>()?.SendMuBotUseResponse(
                status,
                (uint) money,
                Convert.ToByte(money != 0));
        }
    }
}