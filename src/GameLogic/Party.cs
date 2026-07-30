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
using MUnique.OpenMU.Persistence;
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

    private readonly object _writeLock = new();
    private readonly AsyncLock _distributionLock = new();
    private readonly List<Player> _distributionList;

    private readonly TimeSpan _healthUpdateInterval = TimeSpan.FromMilliseconds(500);
    private readonly Task? _healthUpdateTask;
    private CancellationTokenSource? _healthUpdateCts;

    private IPartyMember[] _partyMembers = [];

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
        this._distributionList = new List<Player>(maxPartySize);

        this._healthUpdateCts = new CancellationTokenSource();
        this._healthUpdateTask = this.HealthUpdateLoopAsync(this._healthUpdateCts.Token);
        PartyCount.Add(1);
    }

    /// <summary>
    /// Gets the party members.
    /// </summary>
    public IReadOnlyList<IPartyMember> PartyList => this._partyMembers;

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
        lock (this._writeLock)
        {
            if (this._partyMembers.Length >= this._maxPartySize)
            {
                return false;
            }

            if (this._partyMembers.Length == 0)
            {
                this.PartyMaster = newMember;
            }

            newMember.Party = this;
            this._partyManager.TrackMembership(newMember.Name, this);
            this._partyMembers = [.. this._partyMembers, newMember];
        }

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Replaces <paramref name="oldMember"/> with <paramref name="newMember"/> in-place,
    /// preserving the member's slot index and master status.
    /// </summary>
    /// <param name="oldMember">The member to replace.</param>
    /// <param name="newMember">The new member to insert.</param>
    public async ValueTask ReplaceMemberAsync(IPartyMember oldMember, IPartyMember newMember)
    {
        lock (this._writeLock)
        {
            var index = Array.IndexOf(this._partyMembers, oldMember);
            if (index < 0)
            {
                return;
            }

            var updated = (IPartyMember[])this._partyMembers.Clone();
            updated[index] = newMember;

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

            this._partyMembers = updated;
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
        var toKick = this._partyMembers[index];
        await this.ExitPartyAsync(toKick, index).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows a member to kick themselves.
    /// </summary>
    /// <param name="sender">The member who initiated the kick.</param>
    public async ValueTask KickMySelfAsync(IPartyMember sender)
    {
        var index = Array.IndexOf(this._partyMembers, sender);
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
        foreach (var member in this._partyMembers)
        {
            try
            {
                await member.InvokeViewPlugInAsync<IChatViewPlugIn>(
                    p => p.ChatMessageAsync(message, senderCharacterName, ChatMessageType.Party)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending chat message to {Name}", member.Name);
            }
        }
    }

    /// <summary>
    /// Distributes experience to nearby party members after a kill.
    /// </summary>
    /// <param name="killedObject">The object that was killed.</param>
    /// <param name="killer">The killer who is a party member.</param>
    /// <returns>The experience which each party member gained, with all experience rates applied.</returns>
    public async ValueTask<IReadOnlyList<ExperienceShare>> DistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        using var l = await this._distributionLock.LockAsync();
        try
        {
            return await this.InternalDistributeExperienceAfterKillAsync(killedObject, killer).ConfigureAwait(false);
        }
        finally
        {
            this._distributionList.Clear();
        }
    }

    /// <summary>
    /// Distributes money to nearby party members after a kill.
    /// </summary>
    /// <param name="killed">The object that was killed.</param>
    /// <param name="killer">The killer who is a party member.</param>
    /// <param name="shares">The part of the money which is reserved for each party member.</param>
    public ValueTask DistributeMoneyAfterKillAsync(IAttackable killed, IPartyMember killer, IReadOnlyList<MoneyShare> shares)
    {
        // No lock is taken here: unlike the experience distribution this no longer touches the shared
        // _distributionList, and paying out the pre-computed shares is consistent with the lock-free
        // pick up path in DroppedMoney.
        this._logger.LogDebug("Distributing money after killing {name}", killed.GetName());
        _ = MoneyDistribution.TryPayShares(shares, player => this.IsEligibleForMoney(player, killer));
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Determines whether the player may receive a part of a money drop of the party.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="killer">The killer who is a party member.</param>
    /// <returns><c>True</c>, if the player may receive money; Otherwise, <c>false</c>.</returns>
    internal bool IsEligibleForMoney(Player player, IPartyMember killer)
    {
        return this._partyMembers.Contains(player)
               && player.CurrentMap == killer.CurrentMap
               && !player.IsAtSafezone()
               && player.Attributes is { };
    }

    /// <summary>
    /// Gets drop item groups from nearby party members' active quests.
    /// </summary>
    /// <param name="killer">The party member who made the kill.</param>
    /// <returns>A list of drop item groups from nearby party members' active quests.</returns>
    public async ValueTask<IList<DropItemGroup>> GetQuestDropItemGroupsAsync(IPartyMember killer)
    {
        using var l = await this._distributionLock.LockAsync();
        try
        {
            using (await killer.ObserverLock.ReaderLockAsync().ConfigureAwait(false))
            {
                this._distributionList.AddRange(
                    this._partyMembers.OfType<Player>()
                        .Where(p => p.CurrentMap == killer.CurrentMap
                                    && !p.IsAtSafezone()
                                    && p.IsAlive
                                    && (p == killer || killer.Observers.Contains(p))));
            }

            if (this._distributionList.Count == 0)
            {
                return [];
            }

            var result = this._distributionList
                .SelectMany(m => m.SelectedCharacter?.GetQuestDropItemGroups() ?? [])
                .ToList();

            return result.Count == 0 ? [] : result;
        }
        finally
        {
            this._distributionList.Clear();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        if (this._healthUpdateCts is { } cts)
        {
            await cts.CancelAsync().ConfigureAwait(false);
        }

        if (this._healthUpdateTask is { } task)
        {
            await task.ConfigureAwait(false);
        }

        IPartyMember[] members;
        lock (this._writeLock)
        {
            members = this._partyMembers;
            this._partyMembers = [];
        }

        for (byte i = 0; i < members.Length; i++)
        {
            var member = members[i];
            try
            {
                var index = i;
                await member.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(
                    p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
                this.CleanupMember(member);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error notifying {Name} of party dissolution", member.Name);
            }
        }

        PartyCount.Add(-1);
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._healthUpdateCts?.Cancel();
            this._healthUpdateCts?.Dispose();
            this._healthUpdateCts = null;
        }

        base.Dispose(disposing);
    }

    private static float CalculatePartyExperiencePerLevel(List<Player> recipients, IAttackable killed)
    {
        var memberCount = recipients.Count;
        var totalLevel = recipients.Sum(p => (int)p.Attributes![Stats.TotalLevel]);
        var averageLevel = totalLevel / memberCount;
        var baseExperience = killed.CalculateBaseExperience(averageLevel);

        var partyBonusMultiplier = Math.Pow(1.05, memberCount - 1);
        var mapExperienceMultiplier = killed.CurrentMap?.Definition.ExpMultiplier ?? 1;
        var totalBaseExperience = baseExperience * memberCount * partyBonusMultiplier * mapExperienceMultiplier;

        var attributes = recipients[0].Attributes!;
        var randomMinMultiplier = attributes[Stats.RandomExperienceMinMultiplier];
        var randomMaxMultiplier = attributes[Stats.RandomExperienceMaxMultiplier];
        var totalExperience = CalculateTotalExperience(totalBaseExperience, randomMinMultiplier, randomMaxMultiplier);

        return (float)totalExperience / totalLevel;
    }

    private static int CalculateTotalExperience(double totalBaseExperience, float randomMinMultiplier, float randomMaxMultiplier)
    {
        if (randomMinMultiplier <= 0 || randomMaxMultiplier <= 0)
        {
            return (int)totalBaseExperience;
        }

        var minimumExperience = (int)(totalBaseExperience * randomMinMultiplier);
        var maximumExperience = (int)(totalBaseExperience * randomMaxMultiplier);
        if (minimumExperience < maximumExperience)
        {
            return Rand.NextInt(minimumExperience, maximumExperience);
        }

        return (int)totalBaseExperience;
    }

    private static async ValueTask<int> AwardExperienceAsync(Player player, float perLevel, IAttackable killed)
    {
        var attributes = player.Attributes!;
        var isAtMaxLevel = (short)attributes[Stats.Level] == player.GameContext.Configuration.MaximumLevel;
        var isMasterClass = player.SelectedCharacter?.CharacterClass?.IsMasterClass ?? false;

        if (isAtMaxLevel && isMasterClass)
        {
            var exp = (int)(perLevel
                            * attributes[Stats.TotalLevel]
                            * player.GameContext.MasterExperienceRate
                            * (attributes[Stats.MasterExperienceRate] + attributes[Stats.BonusExperienceRate]));

            await player.AddMasterExperienceAsync(exp, killed).ConfigureAwait(false);
            return exp;
        }

        var normalExperience = (int)(perLevel
                                     * attributes[Stats.Level]
                                     * player.GameContext.ExperienceRate
                                     * (attributes[Stats.ExperienceRate] + attributes[Stats.BonusExperienceRate]));

        if (!isAtMaxLevel)
        {
            await player.AddExperienceAsync(normalExperience, killed).ConfigureAwait(false);
        }

        // At the maximum level without the master quest no experience is awarded, but the amount is
        // still returned: the money drop is derived from it, and a solo kill returns it as well
        // (see Player.AddExpAfterKillAsync), so such a member must not end up without any money.
        return normalExperience;
    }

    private async ValueTask ExitPartyAsync(IPartyMember member, byte index)
    {
        bool shouldDispose;
        lock (this._writeLock)
        {
            if (!this._partyMembers.Contains(member))
            {
                return;
            }

            var remainingCount = this._partyMembers.Length - 1;
            shouldDispose = remainingCount < 2;

            if (!shouldDispose)
            {
                this._partyMembers = this._partyMembers.Where(m => m != member).ToArray();

                // If the party master is leaving, assign the new master to the first remaining member.
                if (this.PartyMaster == member && this._partyMembers.Length > 0)
                {
                    this.PartyMaster = this._partyMembers[0];
                }
            }
        }

        if (shouldDispose)
        {
            await this.DisposeAsync().ConfigureAwait(false);
            return;
        }

        // Notify the member before cleaning up so the index is still valid.
        try
        {
            await member.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(
                p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogDebug(ex, "Error notifying kicked member {Name}", member.Name);
        }

        this.CleanupMember(member);

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
    }

    private void CleanupMember(IPartyMember member)
    {
        member.Party = null;
        this._partyManager.UntrackMembership(member.Name);

        if (member is Player player && player.Attributes is { } attributes)
        {
            attributes[Stats.NearbyPartyMemberCount] = 0;
        }
    }

    private async ValueTask<IReadOnlyList<ExperienceShare>> InternalDistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        if (killedObject.IsSummonedMonster)
        {
            return [];
        }

        using (await killer.ObserverLock.ReaderLockAsync().ConfigureAwait(false))
        {
            this._distributionList.AddRange(
                this._partyMembers.OfType<Player>()
                    .Where(p => p.Attributes is { }
                                && (p == killer || killer.Observers.Contains(p))));
        }

        if (this._distributionList.Count == 0)
        {
            return [];
        }

        var perLevel = CalculatePartyExperiencePerLevel(this._distributionList, killedObject);

        // The shares are copied into their own list, because _distributionList is reused and cleared by the caller.
        var shares = new List<ExperienceShare>(this._distributionList.Count);
        foreach (var player in this._distributionList)
        {
            var experience = await AwardExperienceAsync(player, perLevel, killedObject).ConfigureAwait(false);
            shares.Add(new ExperienceShare(player, experience));
        }

        return shares;
    }

    private async ValueTask UpdateNearbyCountAsync()
    {
        foreach (var member in this._partyMembers)
        {
            if (member is not Player player || player.Attributes is not { } attributes)
            {
                continue;
            }

            try
            {
                using var l = await player.ObserverLock.ReaderLockAsync().ConfigureAwait(false);
                attributes[Stats.NearbyPartyMemberCount] = this._partyMembers.Count(player.Observers.Contains);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error updating {Stat} for {Name}", nameof(Stats.NearbyPartyMemberCount), player.Name);
            }
        }
    }

    private async ValueTask SendPartyListAsync()
    {
        foreach (var member in this._partyMembers)
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
                    foreach (var member in this._partyMembers)
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
            // Expected during shutdown.
        }
    }
}
