// <copyright file="ResetCharacterException.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Character.Reset
{
    using System;

    /// <summary>
    /// Action to reset a character.
    /// </summary>
    public class ResetCharacterException : Exception
    {
        /// <inheritdoc />
        public ResetCharacterException(string message)
            : base(message)
        {
        }
    }
}
