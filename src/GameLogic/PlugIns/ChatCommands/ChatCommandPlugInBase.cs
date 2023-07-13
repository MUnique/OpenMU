// <copyright file="ChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using System.Xml.Linq;

/// <summary>
/// The base of every chat command plug in.
/// </summary>
/// <typeparam name="T">The type of arguments base.</typeparam>
public abstract class ChatCommandPlugInBase<T> : IChatCommandPlugIn
    where T : ArgumentsBase, new()
{
    /// <inheritdoc/>
    public abstract string Key { get; }

    /// <inheritdoc/>
    public abstract CharacterStatus MinCharacterStatusRequirement { get; }

    /// <inheritdoc/>
    public virtual async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            var arguments = command.ParseArguments<T>();
            await this.DoHandleCommandAsync(player, arguments).ConfigureAwait(false);
        }
        catch (ArgumentException argEx)
        {
            await this.ShowMessageToAsync(player, $"[{this.Key}] {argEx.Message}").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, $"Unexpected error handling the chat command '{this.Key}'.", command);
        }
    }

    /// <summary>
    /// Handles the chat command safely.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="arguments">The arguments.</param>
    protected abstract ValueTask DoHandleCommandAsync(Player player, T arguments);

    /// <summary>
    /// Shows a message to a player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="message">The message.</param>
    /// <param name="messageType">The message type.</param>
    protected ValueTask ShowMessageToAsync(Player player, string message, MessageType messageType = MessageType.BlueNormal)
    {
        return player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, messageType));
    }

    /// <summary>
    /// Gets a player by his character name.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="characterName">The character name.</param>
    /// <returns>The target player.</returns>
    protected Player GetPlayerByCharacterName(Player player, string characterName)
    {
        if (string.IsNullOrWhiteSpace(characterName))
        {
            throw new ArgumentException("Character name is required.");
        }

        return player.GameContext.GetPlayerByCharacterName(characterName)
               ?? throw new ArgumentException($"Character {characterName} not found.");
    }

    /// <summary>
    /// Gets a guild id by name.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The guild id.</returns>
    protected async ValueTask<uint> GetGuildIdByNameAsync(Player player, string guildName)
    {
        var guildServer = (player.GameContext as IGameServerContext)!.GuildServer;
        var guildId = await guildServer.GetGuildIdByNameAsync(guildName).ConfigureAwait(false);

        if (guildId == default)
        {
            throw new ArgumentException($"Guild {guildName} not found.");
        }

        return guildId;
    }

    /// <summary>
    /// Gets a exit gate.
    /// </summary>
    /// <param name="gameMaster">The game master.</param>
    /// <param name="map">The name or id of the map.</param>
    /// <param name="coordinates">The coordinates X and Y.</param>
    /// <returns>The ExitGate.</returns>
    protected async ValueTask<ExitGate> GetExitGateAsync(Player gameMaster, string map, Point coordinates)
    {
        if (coordinates.X == default && coordinates.Y == default)
        {
            return this.GetWarpInfo(gameMaster, map)?.Gate
                   ?? throw new ArgumentException($"Map {map} not found.");
        }

        var mapDefinition = ushort.TryParse(map, out var mapId)
            ? (await gameMaster.GameContext.GetMapAsync(mapId).ConfigureAwait(false))?.Definition
            : gameMaster.GameContext.Configuration.Maps.FirstOrDefault(x => x.Name.Equals(map, StringComparison.OrdinalIgnoreCase));

        if (mapDefinition == null)
        {
            throw new ArgumentException($"Map {map} not found.");
        }

        return new ExitGate
        {
            Map = mapDefinition,
            X1 = coordinates.X,
            X2 = coordinates.X,
            Y1 = coordinates.Y,
            Y2 = coordinates.Y,
        };
    }

    /// <summary>
    /// Gets a warp info.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="map">The name or id of the map.</param>
    /// <returns>The WarpInfo.</returns>
    protected WarpInfo? GetWarpInfo(Player player, string map)
    {
        var warpList = player.GameContext.Configuration.WarpList;
        return ushort.TryParse(map, out var mapId)
            ? warpList.FirstOrDefault(info => info.Gate?.Map?.Number == mapId)
            : warpList.FirstOrDefault(info => info.Name.Equals(map, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Change <see cref="AccountState"/> from Account by character name.
    /// </summary>
    /// <param name="gameMaster">GameMaster Player.</param>
    /// <param name="name">Name of character to be changed.</param>
    /// <param name="accountState">New <see cref="AccountState"/>.</param>
    protected ValueTask ChangeAccountStateByCharacterNameAsync(Player gameMaster, string? name, AccountState accountState)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"{nameof(name)} is required.");
        }

        return this.ChangeAccountStateAsync(gameMaster, (MUnique.OpenMU.Persistence.IPlayerContext context) => context.GetAccountByCharacterNameAsync(name), accountState);
    }

    /// <summary>
    /// Change <see cref="AccountState"/> from Account by login name.
    /// </summary>
    /// <param name="gameMaster">GameMaster Player.</param>
    /// <param name="loginName">Login from account to be changed.</param>
    /// <param name="accountState">New <see cref="AccountState"/>.</param>
    protected ValueTask ChangeAccountStateByLoginNameAsync(Player gameMaster, string? loginName, AccountState accountState)
    {
        if (string.IsNullOrEmpty(loginName))
        {
            throw new ArgumentException($"{nameof(loginName)} is required.");
        }

        return this.ChangeAccountStateAsync(gameMaster, (MUnique.OpenMU.Persistence.IPlayerContext context) => context.GetAccountByLoginNameAsync(loginName), accountState);
    }

    /// <summary>
    /// Changes ChatBanUntil value from Account.
    /// </summary>
    /// <param name="player">Player to be banned/unbanned.</param>
    /// <param name="chatBanUntil">Date and time until which the chat ban is in effect.</param>
    protected async ValueTask ChangeAccountChatBanUntilAsync(Player player, DateTime? chatBanUntil)
    {
        if (player.Account != null)
        {
            player.Account.ChatBanUntil = chatBanUntil;
        }
        else
        {
            throw new ArgumentException($"{nameof(player.Account)} not found.");
        }
    }

    private async ValueTask ChangeAccountStateAsync(Player gameMaster, Func<MUnique.OpenMU.Persistence.IPlayerContext, ValueTask<Account?>> accountSelector, AccountState accountState)
    {
        using var context = gameMaster.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameMaster.GameContext.Configuration);
        var account = await accountSelector(context).ConfigureAwait(false);

        if (account != null)
        {
            foreach (var character in account.Characters)
            {
                var player = gameMaster.GameContext.GetPlayerByCharacterName(character.Name ?? string.Empty);

                // disconect to change account
                if (player != null)
                {
                    await player.DisconnectAsync().ConfigureAwait(false);
                    break;
                }
            }

            account.State = accountState;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            throw new ArgumentException($"account not found.");
        }
    }
}