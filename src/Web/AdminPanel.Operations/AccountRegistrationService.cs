// <copyright file="AccountRegistrationService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using System.ComponentModel.DataAnnotations;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.AdminAuth;
using MUnique.OpenMU.Web.AdminPanel.API;

/// <summary>The result of registering an account using OpenMU's native player persistence.</summary>
public enum AccountRegistrationStatus
{
    /// <summary>The registration payload did not match the game-client limits.</summary>
    Invalid,

    /// <summary>The login name is already in use.</summary>
    AlreadyExists,

    /// <summary>The normal player account was persisted.</summary>
    Created,
}

/// <summary>Creates native OpenMU player accounts shared by the audited admin operation and player API.</summary>
public sealed class AccountRegistrationService(IPersistenceContextProvider persistence, IDataSource<GameConfiguration> configurations)
{
    /// <summary>Checks the shared printable-ASCII and game-protocol registration constraints.</summary>
    /// <param name="registration">The registration payload to validate.</param>
    /// <returns><c>true</c> when every native registration constraint is satisfied.</returns>
    public static bool IsValid(AccountRegistration registration)
    {
        var validation = new List<ValidationResult>();
        return Validator.TryValidateObject(registration, new ValidationContext(registration), validation, true);
    }

    /// <summary>Validates and creates a normal account in OpenMU's account persistence.</summary>
    /// <param name="registration">The registration payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The registration outcome.</returns>
    public async ValueTask<AccountRegistrationStatus> RegisterAsync(AccountRegistration registration, CancellationToken cancellationToken = default)
    {
        if (!IsValid(registration))
        {
            return AccountRegistrationStatus.Invalid;
        }

        var config = await configurations.GetOwnerAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        using var context = persistence.CreateNewPlayerContext(config);
        if (await context.GetAccountByLoginNameAsync(registration.LoginName, cancellationToken).ConfigureAwait(false) is not null)
        {
            _ = BCrypt.Net.BCrypt.HashPassword(registration.Password);
            return AccountRegistrationStatus.AlreadyExists;
        }

        var account = context.CreateNew<Account>();
        account.LoginName = registration.LoginName;
        account.EMail = registration.EMail ?? string.Empty;
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registration.Password);
        account.State = AccountState.Normal;
        account.RegistrationDate = DateTime.UtcNow;
        try
        {
            if (!await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new AdminOperationException("The account change could not be saved.");
            }
        }
        catch
        {
            using var check = persistence.CreateNewPlayerContext(config);
            if (await check.GetAccountByLoginNameAsync(registration.LoginName, cancellationToken).ConfigureAwait(false) is not null)
            {
                return AccountRegistrationStatus.AlreadyExists;
            }

            throw;
        }

        return AccountRegistrationStatus.Created;
    }
}
