// <copyright file="Party.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics.Metrics;
using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Party;
using Nito.AsyncEx;

/// <summary>
/// A group of players who share chat, health visibility, and experience distribution.
/// </summary>
public sealed class Party : AsyncDisposable
{
    private static readonly Meter Meter = new(MeterName);
    private static readonly Counter<int> PartyCount = Meter.CreateCounter<int>("PartyCount");

    private readonly ILogger<Party> _logger;
    private readonly IPartyManager _partyManager;
    private readonly byte _maxPartySize;
    private readonly AsyncLock _experienceDistributionLock = new();
    private readonly TimeSpan _healthUpdateInterval = TimeSpan.FromMilliseconds(500);

    private CancellationTokenSource? _healthUpdateCts;

    /// <summary>
    /// Initializes a new instance of the <see cref="Party"/> class.
    /// </summary>
    /// <param name="partyManager">The party manager for membership tracking.</param>
    /// <param name="maxPartySize">Maximum size of the party.</param>
    /// <param name="logger">Logger for party events.</param>
    public Party(IPartyManager partyManager, byte maxPartySize, ILogger<Party> logger)
    {
        this._partyManager = partyManager;
        this._maxPartySize = maxPartySize;
        this._logger = logger;
        this.PartyList = new List<IPartyMember>(maxPartySize);

        this._healthUpdateCts = new CancellationTokenSource();
        _ = this.HealthUpdateLoopAsync(this._healthUpdateCts.Token);
        PartyCount.Add(1);
    }

    /// <summary>
    /// Gets the party members.
    /// </summary>
    public IList<IPartyMember> PartyList { get; }

    /// <summary>
    /// Gets the maximum party size.
    /// </summary>
    public byte MaxPartySize => this._maxPartySize;

    /// <summary>
    /// Gets the party master.
    /// </summary>
    public IPartyMember? PartyMaster { get; private set; }

    private static string MeterName => typeof(Party).FullName ?? nameof(Party);

    /// <summary>
    /// Adds a new member to the party.
    /// </summary>
    /// <param name="newMember">The member to add.</param>
    /// <returns>True if the member was added successfully; false if the party is full.</returns>
    public async ValueTask<bool> AddAsync(IPartyMember newMember)
    {
        if (this.PartyList.Count >= this._maxPartySize)
        {
            return false;
        }

        if (this.PartyList.Count == 0)
        {
            this.PartyMaster = newMember;
        }

        this.PartyList.Add(newMember);
        newMember.Party = this;
        this._partyManager.TrackMembership(newMember.Name, this);

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Replaces <paramref name="oldMember"/> with <paramref name="newMember"/> in-place,
    /// preserving the member's slot index and master status.
    /// </summary>
    /// <param name="oldMember">The member to replace.</param>
    /// <param name="newMember">The new member to add.</param>
    public async ValueTask ReplaceMemberAsync(IPartyMember oldMember, IPartyMember newMember)
    {
        var index = this.PartyList.IndexOf(oldMember);
        if (index < 0)
        {
            return;
        }

        this.PartyList[index] = newMember;
        newMember.Party = this;
        oldMember.Party = null;

        this._partyManager.UntrackMembership(oldMember.Name);
        this._partyManager.TrackMembership(newMember.Name, this);

        if (this.PartyMaster == oldMember)
        {
            this.PartyMaster = newMember;
        }

        if (oldMember is Player oldPlayer && oldPlayer.Attributes is { } oldAttr)
        {
            oldAttr[Stats.NearbyPartyMemberCount] = 0;
        }

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Replaces the live member with an <see cref="OfflinePartyMember"/> snapshot,
    /// keeping the party slot reserved for reconnection.
    /// </summary>
    /// <param name="member">The member who is leaving temporarily.</param>
    public ValueTask LeaveTemporarilyAsync(IPartyMember member)
    {
        var snapshot = new OfflinePartyMember(member);
        return this.ReplaceMemberAsync(member, snapshot);
    }

    /// <summary>
    /// Kicks the member at the given index.
    /// </summary>
    /// <param name="index">The party list index of the member to kick.</param>
    public async ValueTask KickPlayerAsync(byte index)
    {
        var toKick = this.PartyList[index];
        await this.ExitPartyAsync(toKick, index).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows a member to kick themselves.
    /// </summary>
    /// <param name="sender">The member who initiated the kick.</param>
    public async ValueTask KickMySelfAsync(IPartyMember sender)
    {
        var index = this.PartyList.IndexOf(sender);
        if (index >= 0)
        {
            await this.ExitPartyAsync(sender, (byte)index).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Sends a chat message to all party members.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="senderCharacterName">The name of the sending character.</param>
    public async ValueTask SendChatMessageAsync(string message, string senderCharacterName)
    {
        foreach (var member in this.PartyList)
        {
            try
            {
                await member.InvokeViewPlugInAsync<IChatViewPlugIn>(
                    p => p.ChatMessageAsync(message, senderCharacterName, ChatMessageType.Party)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending chat message to party member {Name}", member.Name);
            }
        }
    }

    /// <summary>
    /// Distributes experience to nearby party members after a kill.
    /// </summary>
    /// <param name="killedObject">The object that was killed.</param>
    /// <param name="killer">The killer who is a party member.</param>
    /// <returns>The total experience distributed to all party members.</returns>
    public async ValueTask<int> DistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        using var _ = await this._experienceDistributionLock.LockAsync();
        return await this.InternalDistributeExperienceAfterKillAsync(killedObject, killer).ConfigureAwait(false);
    }

    /// <summary>
    /// Distributes money to nearby party members after a kill.
    /// </summary>
    /// <param name="killedObject">The object that was killed.</param>
    /// <param name="killer">The killer who is a party member.</param>
    /// <param name="amount">The amount of money to distribute.</param>
    public async ValueTask DistributeMoneyAfterKillAsync(IAttackable killedObject, IPartyMember killer, uint amount)
    {
        var recipients = this.GetNearbyPlayers(killer);
        if (recipients.Count == 0)
        {
            return;
        }

        var share = (int)(amount / recipients.Count);
        foreach (var player in recipients)
        {
            player.TryAddMoney(share);
        }
    }

    /// <summary>
    /// Gets drop item groups from nearby party members' active quests.
    /// </summary>
    /// <param name="killer">The party member who made the kill.</param>
    /// <returns>A list of drop item groups from nearby party members' active quests.</returns>
    public async ValueTask<IList<DropItemGroup>> GetQuestDropItemGroupsAsync(IPartyMember killer)
    {
        var nearby = this.GetNearbyPlayers(killer);
        if (nearby.Count == 0)
        {
            return [];
        }

        var result = nearby
            .SelectMany(p => p.SelectedCharacter?.GetQuestDropItemGroups() ?? Enumerable.Empty<DropItemGroup>())
            .ToList();

        return result.Count == 0 ? [] : result;
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._healthUpdateCts is { } cts)
        {
            await cts.CancelAsync().ConfigureAwait(false);
            cts.Dispose();
            this._healthUpdateCts = null;
        }

        foreach (var member in this.PartyList)
        {
            try
            {
                var index = (byte)this.PartyList.IndexOf(member);
                await member.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(
                    p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
                member.Party = null;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error notifying member {Name} of party dissolution", member.Name);
            }
        }

        this.PartyList.Clear();
        PartyCount.Add(-1);

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    private async ValueTask ExitPartyAsync(IPartyMember member, byte index)
    {
        var remainingCount = this.PartyList.Count(m => m != member);
        if (remainingCount < 2)
        {
            // Fewer than 2 members left — dissolve the party.
            await this.DisposeAsync().ConfigureAwait(false);
            return;
        }

        this.PartyList.Remove(member);
        member.Party = null;
        this._partyManager.UntrackMembership(member.Name);

        if (member is Player actualPlayer && actualPlayer.Attributes is { } attributes)
        {
            attributes[Stats.NearbyPartyMemberCount] = 0;
        }

        try
        {
            await member.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(
                p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogDebug(ex, "Error notifying kicked member {Name}", member.Name);
        }

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
    }

    private async ValueTask<int> InternalDistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        if (killedObject.IsSummonedMonster)
        {
            return 0;
        }

        List<Player> recipients;
        using (await killer.ObserverLock.ReaderLockAsync())
        {
            recipients = this.PartyList
                .OfType<Player>()
                .Where(p => p == killer || killer.Observers.Contains(p))
                .ToList();
        }

        var count = recipients.Count;
        if (count == 0)
        {
            return count;
        }

        var totalLevel = recipients.Sum(p => (int)p.Attributes![Stats.TotalLevel]);
        var averageLevel = totalLevel / count;
        var averageExperience = killedObject.CalculateBaseExperience(averageLevel);
        var totalAverageExperience = averageExperience * count * Math.Pow(1.05, count - 1);
        totalAverageExperience *= killedObject.CurrentMap?.Definition.ExpMultiplier ?? 1;
        totalAverageExperience *= recipients[0].GameContext.ExperienceRate;

        var total = Rand.NextInt((int)(totalAverageExperience * 0.8), (int)(totalAverageExperience * 1.2));
        var perLevel = total / totalLevel;

        foreach (var player in recipients)
        {
            var isMasterAtCap = (short)player.Attributes![Stats.Level] == player.GameContext.Configuration.MaximumLevel
                                && (player.SelectedCharacter?.CharacterClass?.IsMasterClass ?? false);

            var expShare = (int)(perLevel * player.Attributes![isMasterAtCap ? Stats.TotalLevel : Stats.Level]
                                 * (player.Attributes[isMasterAtCap ? Stats.MasterExperienceRate : Stats.ExperienceRate]
                                    + player.Attributes[Stats.BonusExperienceRate]));

            if (isMasterAtCap)
            {
                await player.AddMasterExperienceAsync(expShare, killedObject).ConfigureAwait(false);
            }
            else
            {
                await player.AddExperienceAsync(expShare, killedObject).ConfigureAwait(false);
            }
        }

        return total;
    }

    private List<Player> GetNearbyPlayers(IPartyMember killer)
    {
        return this.PartyList
            .OfType<Player>()
            .Where(p => p.CurrentMap == killer.CurrentMap
                        && !p.IsAtSafezone()
                        && p.IsAlive
                        && (p == killer || killer.Observers.Contains(p)))
            .ToList();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100", Justification = "Timer callback — exceptions caught internally.")]
    private async void HealthUpdateElapsed(object? state)
    {
        try
        {
            foreach (var member in this.PartyList)
            {
                var plugIn = member.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>();
                if (plugIn?.IsHealthUpdateNeeded() is true)
                {
                    await plugIn.UpdatePartyHealthAsync().ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogDebug(ex, "Unexpected error during health update");
        }
    }

    private async ValueTask UpdateNearbyCountAsync()
    {
        foreach (var member in this.PartyList)
        {
            if (member is not Player player || player.Attributes is not { } attributes)
            {
                continue;
            }

            try
            {
                using var _ = await player.ObserverLock.ReaderLockAsync().ConfigureAwait(false);
                attributes[Stats.NearbyPartyMemberCount] = this.PartyList.Count(player.Observers.Contains);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error updating {Stat} for {Name}", nameof(Stats.NearbyPartyMemberCount), player.Name);
            }
        }
    }

    private async ValueTask SendPartyListAsync()
    {
        foreach (var member in this.PartyList)
        {
            try
            {
                await member.InvokeViewPlugInAsync<IUpdatePartyListPlugIn>(
                    p => p.UpdatePartyListAsync()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending party list to {Name}", member.Name);
            }
        }
    }

    private async Task HealthUpdateLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(this._healthUpdateInterval);
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    foreach (var member in this.PartyList)
                    {
                        var plugIn = member.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>();
                        if (plugIn?.IsHealthUpdateNeeded() is true)
                        {
                            await plugIn.UpdatePartyHealthAsync().ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Unexpected error during health update");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // expected during shutdown
        }
    }
}