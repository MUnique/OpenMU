// <copyright file="LetterHeader.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities
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
        /// TODO: Reference to character id needed?
        /// </remarks>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <remarks>
        /// TODO: Reference to character id needed?
        /// </remarks>
        public string Receiver { get; set; }

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
    }
}
