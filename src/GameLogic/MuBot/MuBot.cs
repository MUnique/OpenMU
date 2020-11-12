// <copyright file="MuBot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MuBot
{
    using System;
    using System.Timers;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// MuBot Status Map.
    /// </summary>
    public enum MuBotStatus : byte
    {
        /// <summary>
        /// enabled
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// disabled
        /// </summary>
        Disabled = 1,
    }

    /// <summary>
    /// Mu Bot Class.
    /// </summary>
    public class MuBot
    {
        /// <summary>
        /// Internal Timer interval for money collect.
        /// (5 minutes as original server).
        /// </summary>
        private const int Interval = 5 * 60 * 1000;

        /// <summary>
        /// Associated player.
        /// </summary>
        private readonly Player player;

        /// <summary>
        /// The current configuration.
        /// </summary>
        private readonly MuBotConfiguration configuration;

        /// <summary>
        /// Last time that money was collected.
        /// </summary>
        private DateTime lastCollect;

        /// <summary>
        /// Timer for money collect.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuBot"/> class.
        /// </summary>
        /// <param name="player">current player.</param>
        public MuBot(Player player)
        {
            this.player = player;
            this.configuration = this.player.GetMuBotConfiguration();
            if (this.configuration is null)
            {
                return;
            }

            this.player.PlayerEnteredWorld += this.OnPlayerEnteredWorld;
        }

        /// <summary>
        /// Start Mu Bot Timer.
        /// </summary>
        public void Start()
        {
            this.timer = new Timer(Interval)
            {
                Enabled = true,
            };
            this.timer.Elapsed += (sender, e) => this.Collect();
            this.lastCollect = DateTime.Now;
            this.player.ToggleMuBot(MuBotStatus.Enabled);
        }

        /// <summary>
        /// Stop Mu Bot Timer.
        /// </summary>
        public void Stop()
        {
            try
            {
                this.Collect();
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = null;
                this.player.ToggleMuBot(MuBotStatus.Disabled);
            }
            catch (Exception e)
            {
                this.player.Logger.LogWarning(
                    "Exception during disposing the mu bot timer: {0}\n{1}",
                    e.Message,
                    e.StackTrace);
            }
        }

        /// <summary>
        /// Check if player has enough money.
        /// </summary>
        /// <returns>if player can pay the bot.</returns>
        public bool CanAfford()
        {
            return this.player.Money >= this.GetRequiredMoney();
        }

        /// <summary>
        /// Level + MLevel * BotCost * spentTime / 100.
        /// </summary>
        /// <returns>the required money.</returns>
        public int GetRequiredMoney()
        {
            return (int)Math.Round(
                (this.player.Level + this.player.Attributes[Stats.MasterLevel]) *
                this.configuration.Cost *
                this.GetSpentTime() / 1000);
        }

        /// <summary>
        /// Performs the money collection.
        /// </summary>
        private void Collect()
        {
            var amount = this.GetRequiredMoney();
            if (amount > 0 && this.player.TryRemoveMoney(amount))
            {
                this.player.SendMuBotMoneyUsed(amount);
            }

            this.lastCollect = DateTime.Now;
        }

        /// <summary>
        /// Get spent time in mu bot.
        /// </summary>
        /// <returns>seconds since last money collection.</returns>
        private int GetSpentTime()
        {
            if (this.lastCollect == default)
            {
                return 1;
            }

            return (int)(DateTime.Now - this.lastCollect).TotalSeconds;
        }

        /// <summary>
        /// Send Mu Bot Data After Enter.
        /// </summary>
        private void OnPlayerEnteredWorld(object sender, EventArgs e)
        {
            if (this.player.SelectedCharacter.MuBotData == null)
            {
                return;
            }

            this.player.SendCurrentMuBotData();
        }
    }
}
