// <copyright file="ConnectedAccountLoginAttemptAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

/// <summary>
/// Action which handles a login attempt to an account which is still connected.
/// </summary>
/// <remarks>
/// The attempt is only reported after the password was verified, so it's usually the owner of the account.
/// Their previous session may be a stale one, e.g. of a dropped connection or frozen browser tab which
/// the server didn't notice yet. Such a session would block every further login attempt, until a game master
/// disconnects it. Therefore, the connected session is warned and disconnected, so that the next attempt succeeds.
/// </remarks>
public class ConnectedAccountLoginAttemptAction
{
    /// <summary>
    /// Handles the login attempt by warning and disconnecting the players of the account on this game server.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="loginName">The login name of the account.</param>
    public async ValueTask HandleAsync(IGameContext gameContext, string loginName)
    {
        var players = await gameContext.GetPlayersAsync().ConfigureAwait(false);
        foreach (var player in players.Where(p => p.Account?.LoginName == loginName).ToList())
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(Properties.PlayerMessage.LoginAttemptWarning)).ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
        }
    }
}
