namespace MUnique.OpenMU.GameLogic.MuBot
{
    using System;

    /// <summary>
    /// Collected money arguments
    /// </summary>
    public class MoneyCollectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoneyCollectedEventArgs"/> class.
        /// </summary>
        /// <param name="amount">the amount.</param>
        public MoneyCollectedEventArgs(int amount)
        {
            this.Amount = amount;
        }

        /// <summary>
        /// Gets the amount of money collected
        /// </summary>
        public int Amount { get; }
    }
}