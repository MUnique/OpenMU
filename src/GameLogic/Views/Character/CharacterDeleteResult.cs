// <copyright file="CharacterDeleteResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Result of a character delete request.
    /// </summary>
    public enum CharacterDeleteResult
    {
        /// <summary>
        /// Deleting was not successful.
        /// </summary>
        Unsuccessful,

        /// <summary>
        /// Deleting was successful.
        /// </summary>
        Successful,

        /// <summary>
        /// Deleting was not successful because a wrong security code was entered.
        /// </summary>
        WrongSecurityCode,
    }
}