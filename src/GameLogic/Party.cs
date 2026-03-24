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
using Nito.AsyncEx.Synchronous;

/// <summary>
/// The party object. Contains a group of players who can chat with each other, and get information about the health status of their party mates.
/// </summary>
public sealed class Party : Disposable
{
    private static readonly Meter Meter = new(MeterName);

    private static readonly Counter<int> PartyCount = Meter.CreateCounter<int>("PartyCount");

    private readonly ILogger<Party> _logger;

    private readonly IPartyManager _partyManager;

    private readonly Timer _healthUpdate;

    private readonly byte _maxPartySize;

    private readonly List<Player> _distributionList;

    private readonly AsyncLock _distributionLock = new AsyncLock();

    /// <summary>
    /// Initializes a new instance of the <see cref="Party" /> class.
    /// </summary>
    /// <param name="partyManager">The party manager for persistence.</param>
    /// <param name="maxPartySize">Maximum size of the party.</param>
    /// <param name="logger">Logger of this party.</param>
    public Party(IPartyManager partyManager, byte maxPartySize, ILogger<Party> logger)
    {
        this._partyManager = partyManager;
        this._maxPartySize = maxPartySize;
        this._logger = logger;

        this.PartyList = new List<IPartyMember>(maxPartySize);
        this._distributionList = new List<Player>(this.MaxPartySize);
        var updateInterval = new TimeSpan(0, 0, 0, 0, 500);
        this._healthUpdate = new Timer(this.HealthUpdateElapsed, null, updateInterval, updateInterval);
        PartyCount.Add(1);
    }

    /// <summary>
    /// Gets the party list.
    /// </summary>
    public IList<IPartyMember> PartyList { get; }

    /// <summary>
    /// Gets the maximum size of the party.
    /// </summary>
    public byte MaxPartySize => this._maxPartySize;

    /// <summary>
    /// Gets the party master.
    /// </summary>
    public IPartyMember? PartyMaster { get; private set; }

    private static string MeterName => typeof(Party).FullName ?? nameof(Party);

    /// <summary>
    /// Notifies the party that a member has disconnected temporarily.
    /// Delegates offline snapshot creation to <see cref="IPartyManager.OnMemberDisconnectedAsync"/>.
    /// </summary>
    /// <param name="member">The member who disconnected.</param>
    public async ValueTask LeaveTemporarilyAsync(IPartyMember member)
    {
        await this._partyManager.OnMemberDisconnectedAsync(member).ConfigureAwait(false);
    }

    /// <summary>
    /// Kicks the player from the party.
    /// </summary>
    /// <param name="sender">The sender.</param>
    public async ValueTask KickMySelfAsync(IPartyMember sender)
    {
        for (int i = 0; i < this.PartyList.Count; i++)
        {
            if (this.PartyList[i].Id == sender.Id)
            {
                await this.ExitPartyAsync(this.PartyList[i], (byte)i).ConfigureAwait(false);
                return;
            }
        }
    }

    /// <summary>
    /// Kicks the player from the party.
    /// </summary>
    /// <param name="index">The party list index of the member to kick.</param>
    public async ValueTask KickPlayerAsync(byte index)
    {
        var toKick = this.PartyList[index];
        await this.ExitPartyAsync(toKick, index).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the specified new party mate.
    /// </summary>
    /// <param name="newPartyMate">The new party mate.</param>
    /// <returns><c>True</c>, if adding was successful; Otherwise, <c>false</c>.</returns>
    public async ValueTask<bool> AddAsync(IPartyMember newPartyMate)
    {
        if (this.PartyList.Count >= this._maxPartySize)
        {
            return false;
        }

        if (this.PartyList.Count == 0)
        {
            this.PartyMaster = newPartyMate;
        }

        this.PartyList.Add(newPartyMate);
        newPartyMate.Party = this;
        this._partyManager.TrackPartyMembership(newPartyMate.Name, this);

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Replaces the specified <paramref name="oldMember"/> with a <paramref name="newMember"/>.
    /// Used during offline leveling handover to keep the party intact.
    /// </summary>
    /// <param name="oldMember">The old member.</param>
    /// <param name="newMember">The new member.</param>
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
        this._partyManager.RemovePartyMembership(oldMember.Name);
        this._partyManager.TrackPartyMembership(newMember.Name, this);

        if (this.PartyMaster == oldMember)
        {
            this.PartyMaster = newMember;
        }

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);

        if (oldMember is Player oldPlayer && oldPlayer.Attributes is { } oldAttr)
        {
            oldAttr[Stats.NearbyPartyMemberCount] = 0;
        }
    }

    /// <summary>
    /// Sends the chat message to all party members.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="senderCharacterName">The sender character name.</param>
    public async ValueTask SendChatMessageAsync(string message, string senderCharacterName)
    {
        for (int i = 0; i < this.PartyList.Count; i++)
        {
            try
            {
                await this.PartyList[i].InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(message, senderCharacterName, ChatMessageType.Party)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending the chat message");
            }
        }
    }

    /// <summary>
    /// Distributes the experience after kill.
    /// </summary>
    /// <param name="killedObject">The object which was killed.</param>
    /// <param name="killer">The killer which is member of the party. All players which observe the killer, get experience.</param>
    /// <returns>
    /// The total distributed experience to all party members.
    /// </returns>
    public async ValueTask<int> DistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        using var d = await this._distributionLock.LockAsync();
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
    /// Distributes the money after a kill.
    /// </summary>
    /// <param name="killedObject">The object which was killed.</param>
    /// <param name="killer">The killer which is member of the party. All players which observe the killer, get experience.</param>
    /// <param name="amount">The amount of money which should be distributed.</param>
    public async ValueTask DistributeMoneyAfterKillAsync(IAttackable killedObject, IPartyMember killer, uint amount)
    {
        using var d = await this._distributionLock.LockAsync();
        try
        {
            this._distributionList.AddRange(this.PartyList.OfType<Player>().Where(p => p.CurrentMap == killer.CurrentMap && !p.IsAtSafezone() && p.Attributes is { }));
            if (this._distributionList.Count == 0)
            {
                return;
            }

            var moneyPart = amount / this._distributionList.Count;
            this._distributionList.ForEach(p => p.TryAddMoney((int)(moneyPart * p.Attributes![Stats.MoneyAmountRate])));
        }
        finally
        {
            this._distributionList.Clear();
        }
    }

    /// <summary>
    /// Gets the quest drop item groups for the whole party.
    /// </summary>
    /// <param name="killer">The killer.</param>
    /// <returns>The list of <see cref="DropItemGroup"/> which should be considered when generating a drop.</returns>
    public async ValueTask<IList<DropItemGroup>> GetQuestDropItemGroupsAsync(IPartyMember killer)
    {
        using var d = await this._distributionLock.LockAsync();
        try
        {
            using (await killer.ObserverLock.ReaderLockAsync().ConfigureAwait(false))
            {
                this._distributionList.AddRange(
                    this.PartyList.OfType<Player>()
                        .Where(p => p.CurrentMap == killer.CurrentMap
                                    && !p.IsAtSafezone()
                                    && p.IsAlive
                                    && (p == killer || killer.Observers.Contains(p))));
            }

            IList<DropItemGroup> result = [];

            var dropItemGroups = this._distributionList
                .SelectMany(m => m.SelectedCharacter?.GetQuestDropItemGroups() ?? Enumerable.Empty<DropItemGroup>());
            foreach (var dropItemGroup in dropItemGroups)
            {
                if (result.Count == 0)
                {
                    result = new List<DropItemGroup>();
                }

                result.Add(dropItemGroup);
            }

            return result;
        }
        finally
        {
            this._distributionList.Clear();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (this.PartyList.Count > 0)
        {
            for (byte i = 0; i < this.PartyList.Count; i++)
            {
                try
                {
                    var index = i;
                    this.PartyList[i].InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(p => p.PartyMemberRemovedAsync(index)).AsTask().WaitWithoutException();
                    this.PartyList[i].Party = null;
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug(ex, "error at dispose");
                }
            }

            this.PartyList.Clear();
        }

        this._healthUpdate.Dispose();
        PartyCount.Add(-1);
    }

    private async ValueTask<int> InternalDistributeExperienceAfterKillAsync(IAttackable killedObject, IObservable killer)
    {
        if (killedObject.IsSummonedMonster)
        {
            // Do not award experience or drop items for summoned monsters.
            return 0;
        }

        using (await killer.ObserverLock.ReaderLockAsync())
        {
            // All players in the range of the player are getting experience.
            // This might not be like in the original server, where observing the killed monster counts,
            // but at this stage, the monster already has cleared his observers.
            this._distributionList.AddRange(this.PartyList.OfType<Player>().Where(p => p == killer || killer.Observers.Contains(p)));
        }

        var count = this._distributionList.Count;
        if (count == 0)
        {
            return count;
        }

        var totalLevel = this._distributionList.Sum(p => (int)p.Attributes![Stats.TotalLevel]);
        var averageLevel = totalLevel / count;
        var averageExperience = killedObject.CalculateBaseExperience(averageLevel);
        var totalAverageExperience = averageExperience * count * Math.Pow(1.05, count - 1);
        totalAverageExperience *= killedObject.CurrentMap?.Definition.ExpMultiplier ?? 1;
        totalAverageExperience *= this._distributionList.First().GameContext.ExperienceRate;

        var randomizedTotalExperience = Rand.NextInt((int)(totalAverageExperience * 0.8), (int)(totalAverageExperience * 1.2));
        var randomizedTotalExperiencePerLevel = randomizedTotalExperience / totalLevel;
        foreach (var player in this._distributionList)
        {
            if ((short)player.Attributes![Stats.Level] == player.GameContext.Configuration.MaximumLevel)
            {
                if (player.SelectedCharacter?.CharacterClass?.IsMasterClass ?? false)
                {
                    var expMaster = (int)(randomizedTotalExperiencePerLevel * player.Attributes![Stats.TotalLevel] * (player.Attributes[Stats.MasterExperienceRate] + player.Attributes[Stats.BonusExperienceRate]));
                    await player.AddMasterExperienceAsync(expMaster, killedObject).ConfigureAwait(false);
                }
            }
            else
            {
                var exp = (int)(randomizedTotalExperiencePerLevel * player.Attributes![Stats.Level] * (player.Attributes[Stats.ExperienceRate] + player.Attributes[Stats.BonusExperienceRate]));
                await player.AddExperienceAsync(exp, killedObject).ConfigureAwait(false);
            }
        }

        return randomizedTotalExperience;
    }

    private async ValueTask ExitPartyAsync(IPartyMember player, byte index)
    {
        // We dissolve only if fewer than 2 real members exist.
        var remainingMembers = this.PartyList.Count(m => m != player);
        if (remainingMembers < 2)
        {
            this.Dispose();
            return;
        }

        this.PartyList.Remove(player);
        player.Party = null;
        this._partyManager.RemovePartyMembership(player.Name);
        try
        {
            await player.InvokeViewPlugInAsync<IPartyMemberRemovedPlugIn>(p => p.PartyMemberRemovedAsync(index)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogDebug(ex, "Error when calling PartyMemberRemoved. Already disconnected?");
        }

        await this.SendPartyListAsync().ConfigureAwait(false);
        await this.UpdateNearbyCountAsync().ConfigureAwait(false);
        if (player is Player actualPlayer && actualPlayer.Attributes is { } attributes)
        {
            attributes[Stats.NearbyPartyMemberCount] = 0;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void HealthUpdateElapsed(object? state)
    {
        try
        {
            var partyMaster = this.PartyList.FirstOrDefault();
            if (partyMaster is null)
            {
                return;
            }

            bool updateNeeded = partyMaster.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>()?.IsHealthUpdateNeeded() ?? false;
            if (updateNeeded)
            {
                await partyMaster.InvokeViewPlugInAsync<IPartyHealthViewPlugIn>(p => p.UpdatePartyHealthAsync()).ConfigureAwait(false);
                for (var i = this.PartyList.Count - 1; i >= 1; i--)
                {
                    var member = this.PartyList[i];
                    var plugIn = member.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>();
                    if (plugIn?.IsHealthUpdateNeeded() ?? false)
                    {
                        await plugIn.UpdatePartyHealthAsync().ConfigureAwait(false);
                    }
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
        if (this.PartyList.Count == 0)
        {
            return;
        }

        for (byte i = 0; i < this.PartyList.Count; i++)
        {
            try
            {
                if (this.PartyList[i] is not Player player || player.Attributes is not { } attributes)
                {
                    continue;
                }

                using var readerLock = await player.ObserverLock.ReaderLockAsync().ConfigureAwait(false);

                attributes[Stats.NearbyPartyMemberCount] = this.PartyList.Count(player.Observers.Contains);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error updating {statsName}", nameof(Stats.NearbyPartyMemberCount));
            }
        }
    }

    private async ValueTask SendPartyListAsync()
    {
        if (this.PartyList.Count == 0)
        {
            return;
        }

        for (byte i = 0; i < this.PartyList.Count; i++)
        {
            try
            {
                await this.PartyList[i].InvokeViewPlugInAsync<IUpdatePartyListPlugIn>(p => p.UpdatePartyListAsync()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending party list update");
            }
        }
    }
}