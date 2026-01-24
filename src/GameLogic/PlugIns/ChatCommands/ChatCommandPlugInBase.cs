// <copyright file="ChatCommandPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.AttributeSystem;

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;

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
            var arguments = await command.TryParseArgumentsAsync<T>(player).ConfigureAwait(false);
            if (arguments is not null)
            {
                await this.DoHandleCommandAsync(player, arguments).ConfigureAwait(false);
            }
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
    /// Gets a player by his character name.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="characterName">The character name.</param>
    /// <returns>The target player.</returns>
    protected async ValueTask<Player?> GetPlayerByCharacterNameAsync(Player player, string characterName)
    {
        if (string.IsNullOrWhiteSpace(characterName))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNameIsRequired)).ConfigureAwait(false);
            return null;
        }

        var result = player.GameContext.GetPlayerByCharacterName(characterName);
        if (result is null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNotFound), characterName).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Gets a guild id by name.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The guild id.</returns>
    protected async ValueTask<uint?> GetGuildIdByNameAsync(Player player, string guildName)
    {
        var guildServer = (player.GameContext as IGameServerContext)!.GuildServer;
        var guildId = await guildServer.GetGuildIdByNameAsync(guildName).ConfigureAwait(false);

        if (guildId == 0)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GuildNotFound), guildName).ConfigureAwait(false);
            return null;
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
    protected async ValueTask<ExitGate?> GetExitGateAsync(Player gameMaster, string map, Point coordinates)
    {
        if (coordinates == default)
        {
            var result = this.GetWarpInfo(gameMaster, map)?.Gate;
            if (result is null)
            {
                await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MapNotFound), map).ConfigureAwait(false);
            }

            return result;
        }

        var mapDefinition = ushort.TryParse(map, out var mapId)
            ? (await gameMaster.GameContext.GetMapAsync(mapId).ConfigureAwait(false))?.Definition
            : gameMaster.GameContext.Configuration.Maps.FirstOrDefault(x =>
                x.Name.GetTranslationAsSpan(gameMaster.Culture).Equals(map, StringComparison.OrdinalIgnoreCase)
                || x.Name.GetValueInNeutralLanguageAsSpan().Equals(map, StringComparison.OrdinalIgnoreCase));

        if (mapDefinition is null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.MapNotFound), map).ConfigureAwait(false);
            return null;
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
            : warpList.FirstOrDefault(info => info.Name.ToString()?.Equals(map, StringComparison.OrdinalIgnoreCase) ?? false);
    }

    /// <summary>
    /// Change <see cref="AccountState"/> from Account by character name.
    /// </summary>
    /// <param name="gameMaster">GameMaster Player.</param>
    /// <param name="name">Name of character to be changed.</param>
    /// <param name="accountState">New <see cref="AccountState"/>.</param>
    /// <returns>Flag, if successful.</returns>
    protected async ValueTask<bool> TryChangeAccountStateByCharacterNameAsync(Player gameMaster, string? name, AccountState accountState)
    {
        if (string.IsNullOrEmpty(name))
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.CharacterNameIsRequired)).ConfigureAwait(false);
            return false;
        }

        return await this.ChangeAccountStateAsync(gameMaster, context => context.GetAccountByCharacterNameAsync(name), accountState).ConfigureAwait(false);
    }

    /// <summary>
    /// Change <see cref="AccountState"/> from Account by login name.
    /// </summary>
    /// <param name="gameMaster">GameMaster Player.</param>
    /// <param name="loginName">Login from account to be changed.</param>
    /// <param name="accountState">New <see cref="AccountState"/>.</param>
    protected async ValueTask<bool> ChangeAccountStateByLoginNameAsync(Player gameMaster, string? loginName, AccountState accountState)
    {
        if (string.IsNullOrEmpty(loginName))
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LoginNameRequired)).ConfigureAwait(false);
            return false;
        }

        return await this.ChangeAccountStateAsync(gameMaster, context => context.GetAccountByLoginNameAsync(loginName), accountState).ConfigureAwait(false);
    }

    /// <summary>
    /// Changes ChatBanUntil value from Account.
    /// </summary>
    /// <param name="player">Player to be banned/unbanned.</param>
    /// <param name="chatBanUntil">Date and time until which the chat ban is in effect.</param>
    protected async ValueTask<bool> ChangeAccountChatBanUntilAsync(Player player, DateTime? chatBanUntil)
    {
        if (player.Account == null)
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AccountNotFound)).ConfigureAwait(false);
            return false;
        }

        player.Account.ChatBanUntil = chatBanUntil;
        return true;
    }

    /// <summary>
    /// Tries to get the stat attribute of the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="statType">Type of the stat.</param>
    /// <returns>The found stat attribute, or <see langword="null"/>.</returns>
    protected async ValueTask<AttributeDefinition?> TryGetAttributeAsync(Player player, string? statType)
    {
        if (player.SelectedCharacter is not { } selectedCharacter)
        {
            return null;
        }

        var attribute = statType switch
        {
            "str" => Stats.BaseStrength,
            "agi" => Stats.BaseAgility,
            "vit" => Stats.BaseVitality,
            "ene" => Stats.BaseEnergy,
            "cmd" => Stats.BaseLeadership,
            _ => null,
        };
        if (attribute is null)
        {
            await player.ShowLocalizedBlueMessageAsync(PlayerMessage.UnknownAttribute, statType).ConfigureAwait(false);
            return null;
        }

        if (selectedCharacter.Attributes.All(sa => sa.Definition != attribute))
        {
            await player.ShowLocalizedBlueMessageAsync(PlayerMessage.CharacterHasNoStatAttribute, statType).ConfigureAwait(false);
            return null;
        }

        return attribute;
    }

    private async ValueTask<bool> ChangeAccountStateAsync(Player gameMaster, Func<MUnique.OpenMU.Persistence.IPlayerContext, ValueTask<Account?>> accountSelector, AccountState accountState)
    {
        using var context = gameMaster.GameContext.PersistenceContextProvider.CreateNewPlayerContext(gameMaster.GameContext.Configuration);
        var account = await accountSelector(context).ConfigureAwait(false);

        if (account == null)
        {
            await gameMaster.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.AccountNotFound)).ConfigureAwait(false);
            return false;
        }

        foreach (var character in account.Characters)
        {
            var player = gameMaster.GameContext.GetPlayerByCharacterName(character.Name ?? string.Empty);

            // disconnect to change account
            if (player != null)
            {
                await player.DisconnectAsync().ConfigureAwait(false);
                break;
            }
        }

        account.State = accountState;
        await context.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}