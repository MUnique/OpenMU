// <copyright file="PlayerLoginStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.PlayerPortal.Api;

/// <summary>Authentication outcomes with no distinction among nonexistent, wrong-password, and blocked accounts.</summary>
public enum PlayerLoginStatus
{
    /// <summary>Credentials were rejected or the account is not a normal player account.</summary>
    InvalidCredentials,

    /// <summary>The account identifier exceeded its login attempt window.</summary>
    RateLimited,

    /// <summary>A new session could not be allocated because the local session store is full.</summary>
    SessionCapacityReached,

    /// <summary>A new player session was created.</summary>
    Authenticated,
}
