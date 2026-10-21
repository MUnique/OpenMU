// <copyright file="CharacterGuildMemberEnricher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Models;

/// <summary>
/// Enriches guild members with character and account data by resolving them through a player context.
/// </summary>
public class CharacterGuildMemberEnricher : IGuildMemberEnricher
{
    private readonly IPersistenceContextProvider _contextProvider;
    private readonly IDataSource<GameConfiguration> _gameConfigurationSource;
    private readonly ILogger<CharacterGuildMemberEnricher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterGuildMemberEnricher"/> class.
    /// </summary>
    /// <param name="contextProvider">The persistence context provider.</param>
    /// <param name="gameConfigurationSource">The game configuration source, required to create player contexts.</param>
    /// <param name="logger">The logger.</param>
    public CharacterGuildMemberEnricher(IPersistenceContextProvider contextProvider, IDataSource<GameConfiguration> gameConfigurationSource, ILogger<CharacterGuildMemberEnricher> logger)
    {
        this._contextProvider = contextProvider;
        this._gameConfigurationSource = gameConfigurationSource;
        this._logger = logger;
    }

    /// <inheritdoc />
    public async Task EnrichAsync(IReadOnlyList<GuildMemberViewItem> members)
    {
        if (members.Count == 0)
        {
            return;
        }

        using var playerContext = await this.TryCreatePlayerContextAsync().ConfigureAwait(false);
        if (playerContext is null)
        {
            return;
        }

        var accountCache = new Dictionary<string, Account?>(StringComparer.OrdinalIgnoreCase);
        foreach (var member in members)
        {
            await this.EnrichWithCharacterDataAsync(playerContext, member, accountCache).ConfigureAwait(false);
        }
    }

    private async Task<IPlayerContext?> TryCreatePlayerContextAsync()
    {
        try
        {
            var gameConfiguration = await this._gameConfigurationSource.GetOwnerAsync().ConfigureAwait(false);
            return this._contextProvider.CreateNewPlayerContext(gameConfiguration);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Failed to create a player context for guild member enrichment. Members are shown without character details.");
            return null;
        }
    }

    private async Task EnrichWithCharacterDataAsync(
        IPlayerContext playerContext,
        GuildMemberViewItem viewItem,
        Dictionary<string, Account?> accountCache)
    {
        try
        {
            // NOTE: Do NOT use GetByIdAsync<Character> here. The generic EF repository path
            // eagerly walks the full-model navigations, including config types
            // (CharacterClass, AttributeDefinition, ...) which are excluded from the
            // account DbContext model, so the load throws. Resolving through the account
            // instead uses AccountRepository/AccountJsonObjectLoader - the same load path
            // as login and account editing - which returns the full object graph.
            Account? account;
            if (accountCache.TryGetValue(viewItem.CharacterName, out var cachedAccount))
            {
                account = cachedAccount;
            }
            else
            {
                account = await playerContext.GetAccountByCharacterNameAsync(viewItem.CharacterName).ConfigureAwait(false);
                accountCache[viewItem.CharacterName] = account;
                if (account?.Characters is not null)
                {
                    foreach (var characterInAccount in account.Characters)
                    {
                        if (!string.IsNullOrEmpty(characterInAccount.Name))
                        {
                            accountCache[characterInAccount.Name] = account;
                        }
                    }
                }
            }

            if (account is null)
            {
                return;
            }

            var character = account.Characters.FirstOrDefault(c => c.GetId() == viewItem.CharacterId)
                ?? account.Characters.FirstOrDefault(c => c.Name == viewItem.CharacterName);
            if (character is null)
            {
                return;
            }

            viewItem.CharacterName = character.Name;
            viewItem.CharacterClass = character.CharacterClass?.Name.ToString();
            viewItem.Level = (int)(character.Attributes?.FirstOrDefault(attribute => attribute.Definition == Stats.Level)?.Value ?? 0);
            viewItem.MasterLevel = (int)(character.Attributes?.FirstOrDefault(attribute => attribute.Definition == Stats.MasterLevel)?.Value ?? 0);

            viewItem.AccountLoginName = account.LoginName;
            var accountId = account.GetId();
            viewItem.AccountId = accountId != Guid.Empty ? accountId : null;
        }
        catch (Exception ex)
        {
            // Character details are best-effort; the guild member is still shown with its name and position.
            this._logger.LogDebug(ex, "Failed to enrich guild member {CharacterId} with character data.", viewItem.CharacterId);
        }
    }
}
