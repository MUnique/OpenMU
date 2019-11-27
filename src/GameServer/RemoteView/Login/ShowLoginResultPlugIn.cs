// <copyright file="ShowLoginResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Login
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Login;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowLoginResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowLoginResultPlugIn", "The default implementation of the IShowLoginResultPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("9ba2646b-72c2-4876-a316-c9aadb386037")]
    public class ShowLoginResultPlugIn : IShowLoginResultPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowLoginResultPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowLoginResultPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowLoginResult(LoginResult loginResult)
        {
            using var writer = this.player.Connection.StartSafeWrite(LoginResponse.HeaderType, LoginResponse.Length);
            _ = new LoginResponse(writer.Span)
            {
                Success = ConvertResult(loginResult),
            };

            writer.Commit();
        }

        private static LoginResponse.LoginResult ConvertResult(LoginResult loginResult)
        {
            return loginResult switch
            {
                LoginResult.AccountAlreadyConnected => LoginResponse.LoginResult.AccountAlreadyConnected,
                LoginResult.AccountBlocked => LoginResponse.LoginResult.AccountBlocked,
                LoginResult.AccountInvalid => LoginResponse.LoginResult.AccountInvalid,
                LoginResult.BadCountry => LoginResponse.LoginResult.BadCountry,
                LoginResult.ConnectionClosed3Fails => LoginResponse.LoginResult.ConnectionClosed3Fails,
                LoginResult.ConnectionError => LoginResponse.LoginResult.ConnectionError,
                LoginResult.InvalidPassword => LoginResponse.LoginResult.InvalidPassword,
                LoginResult.NoChargeInfo => LoginResponse.LoginResult.NoChargeInfo,
                LoginResult.OK => LoginResponse.LoginResult.Okay,
                LoginResult.OnlyPlayersOver15Yrs => LoginResponse.LoginResult.OnlyPlayersOver15Yrs,
                LoginResult.ServerIsFull => LoginResponse.LoginResult.ServerIsFull,
                LoginResult.SubscriptionTermOver => LoginResponse.LoginResult.SubscriptionTermOver,
                LoginResult.SubscriptionTimeOver => LoginResponse.LoginResult.SubscriptionTimeOver,
                LoginResult.TemporaryBlocked => LoginResponse.LoginResult.TemporaryBlocked,
                LoginResult.WrongVersion => LoginResponse.LoginResult.WrongVersion,
                _ => LoginResponse.LoginResult.ConnectionError,
            };
        }
    }
}