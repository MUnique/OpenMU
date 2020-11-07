﻿namespace MUnique.OpenMU.GameLogic.MuBot
{
    using System;
    using System.Timers;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Mu Bot Class
    /// </summary>
    public class MuBot
    {
        /// <summary>
        /// Internal Timer interval for money collect
        /// </summary>
        private const int Interval = 1000 * 10; //5 * 60 * 1000;

        /// <summary>
        /// Associated player.
        /// </summary>
        private readonly Player player;

        /// <summary>
        /// Last time that money was collected
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
        }

        /// <summary>
        /// Trigger when money is collected
        /// </summary>
        public event EventHandler<MoneyCollectedEventArgs> MoneyCollected;

        /// <summary>
        /// Start Mu Bot Timer
        /// </summary>
        public void Start()
        {
            this.timer = new Timer(Interval)
            {
                Enabled = true,
                AutoReset = true,
            };
            this.timer.Elapsed += (sender, e) => this.Collect();
            this.lastCollect = DateTime.Now;
        }

        /// <summary>
        /// Stop Mu Bot Timer
        /// </summary>
        public void Stop()
        {
            try
            {
                this.Collect();
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = null;
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
        /// Check if player has enough money
        /// </summary>
        /// <returns>if player can pay the bot.</returns>
        public bool CanAfford()
        {
            return this.player.Money >= this.GetRequiredMoney();
        }

        /// <summary>
        /// Level + MLevel * BotCost * spentTime / 100
        /// </summary>
        /// <returns>the required money.</returns>
        public int GetRequiredMoney()
        {
            return (int) Math.Round(
                (this.player.Level + this.player.Attributes[Stats.MasterLevel]) *
                MuBotConfiguration.Cost *
                this.GetSpentTime() / 100);
        }

        /// <summary>
        /// Performs the money collection
        /// </summary>
        private void Collect()
        {
            var amount = this.GetRequiredMoney();
            this.lastCollect = DateTime.Now;
            if (!this.player.TryRemoveMoney(amount))
            {
                return;
            }

            this.MoneyCollected?.Invoke(this, new MoneyCollectedEventArgs(amount));
        }

        /// <summary>
        /// Get spent time in mu bot
        /// </summary>
        /// <returns>seconds since last money collection.</returns>
        private int GetSpentTime()
        {
            if (this.lastCollect == default)
            {
                return 1;
            }

            return (int) (DateTime.Now - this.lastCollect).TotalSeconds;
        }
    }
}