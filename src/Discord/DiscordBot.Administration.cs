// <copyright file="DiscordBot.Administration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord;

using global::Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// The part of the <see cref="DiscordBot"/> which the admin panel uses to show its state and to set up a Discord server.
/// </summary>
public sealed partial class DiscordBot
{
    /// <summary>
    /// The close code of the gateway when the bot requests a privileged intent which isn't enabled for it.
    /// </summary>
    private const int DisallowedIntentsCloseCode = 4014;

    /// <summary>
    /// The close code of the gateway when Discord rejects the token.
    /// </summary>
    private const int AuthenticationFailedCloseCode = 4004;

    private DiscordBotProblem _problem;

    /// <summary>
    /// Gets the problem which stopped the bot, until it's started again.
    /// </summary>
    public DiscordBotProblem Problem
    {
        get => this._problem;
        private set
        {
            if (this._problem != value)
            {
                this._problem = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the name of the bot user, when it's connected.
    /// </summary>
    public string? UserName => this._client?.CurrentUser?.Username;

    /// <summary>
    /// Gets the identifier of the Discord server whose channels receive the notifications and the status.
    /// </summary>
    public ulong? RoutingServerId => this._client is { } client ? this.GetRoutingGuild(client)?.Id : null;

    /// <summary>
    /// Gets the Discord servers which the bot is in.
    /// </summary>
    /// <returns>The Discord servers, ordered by their name.</returns>
    public IReadOnlyList<DiscordServerInfo> GetDiscordServers()
    {
        return this._client?.Guilds
                   .Select(guild => new DiscordServerInfo(guild.Id, guild.Name))
                   .OrderBy(server => server.Name, StringComparer.CurrentCultureIgnoreCase)
                   .ToList()
               ?? [];
    }

    /// <summary>
    /// Sets up the roles, categories and channels of the layout on a Discord server, like <c>/openmu setup</c>.
    /// </summary>
    /// <param name="discordServerId">The identifier of the Discord server.</param>
    /// <returns>The result, or <c>null</c>, if the bot isn't connected or isn't in the Discord server.</returns>
    public async ValueTask<DiscordProvisioningResult?> SetUpDiscordServerAsync(ulong discordServerId)
    {
        if (this._client is not { } client || client.GetGuild(discordServerId) is not { } guild)
        {
            return null;
        }

        this._logger.LogInformation("The Discord server {guild} ({guildId}) is set up from the admin panel.", guild.Name, guild.Id);
        return await this.ProvisionAsync(client, guild).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines the problem which an exception of the client reports, and which stops the bot.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The problem, or <see cref="DiscordBotProblem.None"/>.</returns>
    private static DiscordBotProblem GetStoppingProblem(Exception? exception)
    {
        for (; exception is not null; exception = exception.InnerException)
        {
            switch (exception)
            {
                case global::Discord.Net.HttpException { HttpCode: System.Net.HttpStatusCode.Unauthorized }:
                case global::Discord.Net.WebSocketClosedException { CloseCode: AuthenticationFailedCloseCode }:
                    return DiscordBotProblem.TokenRejected;
                case global::Discord.Net.WebSocketClosedException { CloseCode: DisallowedIntentsCloseCode }:
                    return DiscordBotProblem.IntentNotEnabled;
            }
        }

        return DiscordBotProblem.None;
    }

    private async ValueTask<DiscordProvisioningResult> ProvisionAsync(DiscordSocketClient client, SocketGuild guild)
    {
        var result = await this._provisioner.ProvisionAsync(new SocketGuildServer(guild), this._layout).ConfigureAwait(false);
        if (this.GetRoutingGuild(client)?.Id == guild.Id)
        {
            // The cache of the client might not know the created channels yet.
            this._routing = DiscordChannelRouting.Create(this._settings, this._layout, result.ChannelIds);
        }

        return result;
    }
}
