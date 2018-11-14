// <copyright file="AccountDto.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Data transfer object for <see cref="Account"/>.
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// Gets or sets the identifier of the account.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the e mail address of the account.
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Gets or sets the name of the login of the account.
        /// </summary>
        /// <value>
        /// The name of the login.
        /// </value>
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets the new password of a changed account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the state of the account.
        /// </summary>
        public AccountState State { get; set; }
    }
}
