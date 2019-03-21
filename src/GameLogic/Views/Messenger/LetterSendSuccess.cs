// <copyright file="LetterSendSuccess.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// The letter send success flag.
    /// </summary>
    public enum LetterSendSuccess : byte
    {
        /// <summary>
        /// There was a problem and he should try again.
        /// </summary>
        TryAgain = 0,

        /// <summary>
        /// The letter has been sent successfully.
        /// </summary>
        Success = 1,

        /// <summary>
        /// The mailbox of the recipient is full.
        /// </summary>
        MailboxFull = 2,

        /// <summary>
        /// The receiver does not exist.
        /// </summary>
        ReceiverNotExists = 3,

        /// <summary>
        /// A letter can't be sent to yourself.
        /// </summary>
        CantSendToYourself = 4,

        /// <summary>
        /// The sender doesn't have enough money to send a letter.
        /// </summary>
        NotEnoughMoney = 7,
    }
}