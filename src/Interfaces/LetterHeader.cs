// <copyright file="LetterHeader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;

    /// <summary>
    /// The header of a letter.
    /// </summary>
    public class LetterHeader
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the sender.
        /// </summary>
        /// <remarks>
        /// The persistence implementation can implement to keep an internal id, too.
        /// However, it should keep the name, if it should stay available after a sender character got deleted.
        /// </remarks>
        public string SenderName { get; set; }

        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <remarks>
        /// The persistence implementation can implement to keep an internal id, too.
        /// In this case, it may not be required to save the name itself.
        /// </remarks>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the letter date.
        /// </summary>
        public DateTime LetterDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the letter has been read.
        /// </summary>
        public bool ReadFlag { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.SenderName} -> {this.ReceiverName} - {this.Subject} ({this.LetterDate})";
        }
    }
}
