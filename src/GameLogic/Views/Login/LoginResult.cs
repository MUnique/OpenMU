// <copyright file="LoginResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Login
{
    /// <summary>
    /// The result of a login request.
    /// </summary>
    public enum LoginResult : byte
    {
        /// <summary>
        /// The password was wrong.
        /// </summary>
        InvalidPassword = 0,

        /// <summary>
        /// The login succeeded.
        /// </summary>
        OK = 1,

        /// <summary>
        /// The account is invalid.
        /// </summary>
        AccountInvalid = 2,

        /// <summary>
        /// The account is already connected.
        /// </summary>
        AccountAlreadyConnected = 3,

        /// <summary>
        /// The server is full.
        /// </summary>
        ServerIsFull = 4,

        /// <summary>
        /// The account is blocked.
        /// </summary>
        AccountBlocked = 5,

        /// <summary>
        /// The client has a wrong version.
        /// </summary>
        WrongVersion = 6,

        /// <summary>
        /// An error occured during connection.
        /// </summary>
        /// <remarks>I think in the original game server it is the connection to some of the servers behind.</remarks>
        ConnectionError = 7,

        /// <summary>
        /// Connection closed because of three failed login requests.
        /// </summary>
        ConnectionClosed3Fails = 8,

        /// <summary>
        /// There is no payment information.
        /// </summary>
        NoChargeInfo = 9,

        /// <summary>
        /// Subscription term is over.
        /// </summary>
        SubscriptionTermOver = 10,

        /// <summary>
        /// Subscription time is over.
        /// </summary>
        SubscriptionTimeOver = 11,

        /// <summary>
        /// Only players over 15 years are allowed.
        /// </summary>
        OnlyPlayersOver15Yrs = 0x11,

        /// <summary>
        /// The account is temporarily blocked.
        /// </summary>
        TemporaryBlocked = 0x0E,

        /// <summary>
        /// The client connected from a blocked country.
        /// </summary>
        BadCountry = 0xD2,
    }
}