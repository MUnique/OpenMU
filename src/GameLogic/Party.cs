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
    private readonly List<IPartyMember> _partyList;
    private readonly AsyncLock _distributionLock = new();
    private readonly List<Player> _distributionList;
    private readonly TimeSpan _healthUpdateInterval = TimeSpan.FromMilliseconds(500);
    private readonly Task? _healthUpdateTask;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose(bool disposing) method.")]
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
        this._partyList = new List<IPartyMember>(maxPartySize);
        this._distributionList = new List<Player>(maxPartySize);

        this._healthUpdateCts = new CancellationTokenSource();
        this._healthUpdateTask = this.HealthUpdateLoopAsync(this._healthUpdateCts.Token);
        PartyCount.Add(1);
    }

    /// <summary>
    /// Gets the party members.
    /// </summary>
    public IReadOnlyList<IPartyMember> PartyList => this._partyList;

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
        if (this._partyList.Count >= this._maxPartySize)
        {
            return false;
        }

        if (this._partyList.Count == 0)
        {
            this.PartyMaster = newMember;
        }

        this._partyList.Add(newMember);
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
    /// <param name="newMember">The new member to insert.</param>
    public async ValueTask ReplaceMemberAsync(IPartyMember oldMember, IPartyMember newMember)
    {
        var index = this._partyList.IndexOf(oldMember);
        if (index < 0)
        {
            return;
        }

        this._partyList[index] = newMember;
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
        var toKick = this._partyList[index];
        await this.ExitPartyAsync(toKick, index).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows a member to kick themselves.
    /// </summary>
    /// <param name="sender">The member who initiated the kick.</param>
    public async ValueTask KickMySelfAsync(IPartyMember sender)
    {
        var index = this._partyList.IndexOf(sender);
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
        foreach (var member in this._partyList)
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
    /// <returns>The total experience distributed.</returns>
    public async ValueTask<int> DistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        using var _ = await this._distributionLock.LockAsync();
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
    /// <param name="amount">The amount of money to distribute.</param>
    public async ValueTask DistributeMoneyAfterKillAsync(IAttackable killed, IPartyMember killer, uint amount)
    {
        using var _ = await this._distributionLock.LockAsync();
        try
        {
            this._logger.LogDebug("Distributing money after killing {name}", killed.GetName());
            this._distributionList.AddRange(
                this._partyList.OfType<Player>()
                    .Where(p => p.CurrentMap == killer.CurrentMap
                                && !p.IsAtSafezone()
                                && p.Attributes is { }));

            if (this._distributionList.Count == 0)
            {
                return;
            }

            var moneyPart = amount / this._distributionList.Count;
            foreach (var player in this._distributionList)
            {
                player.TryAddMoney((int)(moneyPart * player.Attributes![Stats.MoneyAmountRate]));
            }
        }
        finally
        {
            this._distributionList.Clear();
        }
    }

    /// <summary>
    /// Gets drop item groups from nearby party members' active quests.
    /// </summary>
    /// <param name="killer">The party member who made the kill.</param>
    /// <returns>A list of drop item groups from nearby party members' active quests.</returns>
    public async ValueTask<IList<DropItemGroup>> GetQuestDropItemGroupsAsync(IPartyMember killer)
    {
        using var _ = await this._distributionLock.LockAsync();
        try
        {
            using (await killer.ObserverLock.ReaderLockAsync().ConfigureAwait(false))
            {
                this._distributionList.AddRange(
                    this._partyList.OfType<Player>()
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

        for (byte i = 0; i < this._partyList.Count; i++)
        {
            var member = this._partyList[i];
            try
            {
                var index = i;
                await member.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(
                    p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
                member.Party = null;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error notifying {Name} of party dissolution", member.Name);
            }
        }

        this._partyList.Clear();
        PartyCount.Add(-1);

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._healthUpdateCts?.Dispose();
            this._healthUpdateCts = null;
        }

        base.Dispose(disposing);
    }

    private static (int Total, int PerLevel) CalculatePartyExperience(List<Player> recipients, IAttackable killed)
    {
        var count = recipients.Count;
        var totalLevel = recipients.Sum(p => (int)p.Attributes![Stats.TotalLevel]);
        var averageLevel = totalLevel / count;
        var baseExp = killed.CalculateBaseExperience(averageLevel);

        var totalAvg = baseExp * count * Math.Pow(1.05, count - 1);
        totalAvg *= killed.CurrentMap?.Definition.ExpMultiplier ?? 1;
        totalAvg *= recipients[0].GameContext.ExperienceRate;

        var total = Rand.NextInt((int)(totalAvg * 0.8), (int)(totalAvg * 1.2));
        var perLevel = total / totalLevel;

        return (total, perLevel);
    }

    private static async ValueTask AwardExperienceAsync(Player player, int perLevel, IAttackable killed)
    {
        var isMasterAtCap =
            (short)player.Attributes![Stats.Level] == player.GameContext.Configuration.MaximumLevel
            && (player.SelectedCharacter?.CharacterClass?.IsMasterClass ?? false);

        var levelStat = isMasterAtCap ? Stats.TotalLevel : Stats.Level;
        var rateStat = isMasterAtCap ? Stats.MasterExperienceRate : Stats.ExperienceRate;

        var exp = (int)(perLevel
                        * player.Attributes![levelStat]
                        * (player.Attributes[rateStat] + player.Attributes[Stats.BonusExperienceRate]));

        if (isMasterAtCap)
        {
            await player.AddMasterExperienceAsync(exp, killed).ConfigureAwait(false);
        }
        else
        {
            await player.AddExperienceAsync(exp, killed).ConfigureAwait(false);
        }
    }

    private async ValueTask ExitPartyAsync(IPartyMember member, byte index)
    {
        var remainingCount = this._partyList.Count(m => m != member);
        if (remainingCount < 2)
        {
            await this.DisposeAsync().ConfigureAwait(false);
            return;
        }

        this._partyList.Remove(member);
        member.Party = null;
        this._partyManager.UntrackMembership(member.Name);

        if (member is Player player && player.Attributes is { } attributes)
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

        using (await killer.ObserverLock.ReaderLockAsync())
        {
            this._distributionList.AddRange(
                this._partyList.OfType<Player>()
                    .Where(p => p == killer || killer.Observers.Contains(p)));
        }

        if (this._distributionList.Count == 0)
        {
            return 0;
        }

        var (total, perLevel) = CalculatePartyExperience(this._distributionList, killedObject);

        foreach (var player in this._distributionList)
        {
            await AwardExperienceAsync(player, perLevel, killedObject).ConfigureAwait(false);
        }

        return total;
    }


    private async ValueTask UpdateNearbyCountAsync()
    {
        foreach (var member in this._partyList)
        {
            if (member is not Player player || player.Attributes is not { } attributes)
            {
                continue;
            }

            try
            {
                using var _ = await player.ObserverLock.ReaderLockAsync().ConfigureAwait(false);
                attributes[Stats.NearbyPartyMemberCount] = this._partyList.Count(player.Observers.Contains);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error updating {Stat} for {Name}", nameof(Stats.NearbyPartyMemberCount), player.Name);
            }
        }
    }

    private async ValueTask SendPartyListAsync()
    {
        foreach (var member in this._partyList)
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
                    foreach (var member in this._partyList)
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