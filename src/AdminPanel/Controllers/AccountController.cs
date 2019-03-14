// <copyright file="AccountController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using Microsoft.AspNetCore.Mvc;
    using MUnique.OpenMU.AdminPanel.Models;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Controller for the account list.
    /// </summary>
    [Route("admin/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AccountController));
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public AccountController(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
        }

        /// <summary>
        /// Returns a slice of the account list, defined by an offset and a count.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>A slice of the account list, defined by an offset and a count.</returns>
        [HttpGet("{offset}/{count}")]
        public ActionResult<List<AccountDto>> List(int offset, int count)
        {
            using (var context = this.persistenceContextProvider.CreateNewPlayerContext(null))
            {
                var accounts = context.GetAccountsOrderedByLoginName(offset, count)
                    .Select(a => new AccountDto
                    {
                        Id = a.GetId(),
                        LoginName = a.LoginName,
                        EMail = a.EMail,
                        State = a.State,
                    })
                    .ToList();

                return accounts;
            }
        }

        /// <summary>
        /// Saves the specified account.
        /// Creates a new account, if the <see cref="AccountDto.Id"/> is empty, or updates an existing account if the <see cref="AccountDto.Id"/> isn't empty.
        /// </summary>
        /// <param name="accountDto">The account.</param>
        /// <returns>The success of the operation.</returns>
        [HttpPost]
        public IActionResult Save([FromBody] AccountDto accountDto)
        {
            try
            {
                using (var configContext = this.persistenceContextProvider.CreateNewConfigurationContext())
                {
                    var configuration = configContext.Get<GameConfiguration>().FirstOrDefault();
                    using (var context = this.persistenceContextProvider.CreateNewPlayerContext(configuration))
                    {
                        Account account = accountDto.Id == Guid.Empty
                            ? context.CreateNew<Account>()
                            : context.GetById<Account>(accountDto.Id);

                        this.AssignAccountValues(account, accountDto);

                        context.SaveChanges();
                        return this.Ok(account.GetId());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes the account with the specified id.
        /// </summary>
        /// <param name="accountId">The account id.</param>
        /// <returns>The success of the operation.</returns>
        [HttpPost]
        public IActionResult Delete(Guid accountId)
        {
            try
            {
                using (var context = this.persistenceContextProvider.CreateNewContext())
                {
                    var account = context.GetById<Account>(accountId);
                    var success = context.Delete(account);

                    return this.Ok(success && context.SaveChanges());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                throw;
            }
        }

        private void AssignAccountValues(Account account, AccountDto dto)
        {
            account.EMail = dto.EMail;
            account.State = dto.State;
            account.LoginName = dto.LoginName;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }
        }
    }
}
