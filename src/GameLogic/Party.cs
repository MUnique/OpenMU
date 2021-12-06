// <copyright file="Party.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Party;

/// <summary>
/// The party object. Contains a group of players who can chat with each other, and get information about the health status of their party mates.
/// </summary>
public sealed class Party : Disposable
{
    private readonly ILogger<Party> _logger;

    private readonly Timer _healthUpdate;

    private readonly byte _maxPartySize;

    private readonly List<Player> _distributionList;

    /// <summary>
    /// Initializes a new instance of the <see cref="Party" /> class.
    /// </summary>
    /// <param name="partyMaster">The party master.</param>
    /// <param name="maxPartySize">Maximum size of the party.</param>
    /// <param name="logger">Logger of this party.</param>
    public Party(IPartyMember partyMaster, byte maxPartySize, ILogger<Party> logger)
    {
        this.PartyMaster = partyMaster;
        this._maxPartySize = maxPartySize;
        this._logger = logger;

        this.PartyList = new List<IPartyMember>(maxPartySize);
        this._distributionList = new List<Player>(this.MaxPartySize);
        var updateInterval = new TimeSpan(0, 0, 0, 0, 500);
        this._healthUpdate = new Timer(this.HealthUpdate_Elapsed, null, updateInterval, updateInterval);
        this.Add(partyMaster);
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
    public IPartyMember PartyMaster { get; }

    /// <summary>
    /// Kicks the player from the party.
    /// </summary>
    /// <param name="sender">The sender.</param>
    public void KickMySelf(IPartyMember sender)
    {
        for (int i = 0; i < this.PartyList.Count; i++)
        {
            if (this.PartyList[i].Id == sender.Id)
            {
                this.ExitParty(this.PartyList[i], (byte)i);
                return;
            }
        }
    }

    /// <summary>
    /// Kicks the player from the party.
    /// </summary>
    /// <param name="index">The party list index of the member to kick.</param>
    public void KickPlayer(byte index)
    {
        var toKick = this.PartyList[index];
        this.ExitParty(toKick, index);
    }

    /// <summary>
    /// Adds the specified new party mate.
    /// </summary>
    /// <param name="newPartyMate">The new party mate.</param>
    /// <returns><c>True</c>, if adding was successful; Otherwise, <c>false</c>.</returns>
    public bool Add(IPartyMember newPartyMate)
    {
        if (this.PartyList.Count >= this._maxPartySize)
        {
            return false;
        }

        this.PartyList.Add(newPartyMate);
        newPartyMate.Party = this;
        this.SendPartyList();
        return true;
    }

    /// <summary>
    /// Sends the chat message to all party members.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="senderCharacterName">The sender character name.</param>
    public void SendChatMessage(string message, string senderCharacterName)
    {
        for (int i = 0; i < this.PartyList.Count; i++)
        {
            try
            {
                this.PartyList[i].ViewPlugIns.GetPlugIn<IChatViewPlugIn>()?.ChatMessage(message, senderCharacterName, ChatMessageType.Party);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug("Error sending the chat message", ex);
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
    public int DistributeExperienceAfterKill(IAttackable killedObject, IObservable killer)
    {
        lock (this._distributionList)
        {
            try
            {
                return this.InternalDistributeExperienceAfterKill(killedObject, killer);
            }
            finally
            {
                this._distributionList.Clear();
            }
        }
    }

    /// <summary>
    /// Distributes the money after a kill.
    /// </summary>
    /// <param name="killedObject">The object which was killed.</param>
    /// <param name="killer">The killer which is member of the party. All players which observe the killer, get experience.</param>
    /// <param name="amount">The amount of money which should be distributed.</param>
    public void DistributeMoneyAfterKill(IAttackable killedObject, IPartyMember killer, uint amount)
    {
        lock (this._distributionList)
        {
            try
            {
                this._distributionList.AddRange(this.PartyList.OfType<Player>().Where(p => p.CurrentMap == killer.CurrentMap && !p.IsAtSafezone() && p.Attributes is { }));
                var moneyPart = amount / this._distributionList.Count;
                this._distributionList.ForEach(p => p.TryAddMoney((int)(moneyPart * p.Attributes![Stats.MoneyAmountRate])));
            }
            finally
            {
                this._distributionList.Clear();
            }
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
                    this.PartyList[i].ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()?.PartyMemberRemoved(i);
                    this.PartyList[i].Party = null;
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug("error at dispose", ex);
                }
            }

            this.PartyList.Clear();
        }

        this._healthUpdate.Dispose();
    }

    private int InternalDistributeExperienceAfterKill(IAttackable killedObject, IObservable killer)
    {
        if (killedObject is IObservable observable)
        {
            observable.ObserverLock.EnterReadLock();
            try
            {
                this._distributionList.AddRange(this.PartyList.OfType<Player>().Where(p => observable.Observers.Contains(p)));
            }
            finally
            {
                observable.ObserverLock.ExitReadLock();
            }
        }
        else
        {
            this._distributionList.AddRange(this.PartyList.OfType<Player>());
        }

        var count = this._distributionList.Count;
        if (count == 0)
        {
            return count;
        }

        var totalLevel = this._distributionList.Sum(p => (int)p.Attributes![Stats.Level] + p.Attributes![Stats.MasterLevel]);
        var averageLevel = totalLevel / count;
        var averageExperience = killedObject.Attributes[Stats.Level] * 1000 / averageLevel;
        var totalAverageExperience = averageExperience * count * Math.Pow(1.2, count - 1);
        var randomizedTotalExperience = Rand.NextInt((int)(totalAverageExperience * 0.8), (int)(totalAverageExperience * 1.2));
        var randomizedTotalExperiencePerLevel = randomizedTotalExperience / (float)totalLevel;
        foreach (var player in this._distributionList)
        {
            if (player.SelectedCharacter?.CharacterClass?.IsMasterClass ?? false)
            {
                var exp = (int)(randomizedTotalExperiencePerLevel * (player.Attributes![Stats.MasterLevel] + player.Attributes![Stats.Level]) * player.Attributes[Stats.MasterExperienceRate] * player.GameContext.ExperienceRate);
                player.AddMasterExperience(exp, killedObject);
            }
            else
            {
                var exp = (int)(randomizedTotalExperiencePerLevel * player.Attributes![Stats.Level] * player.Attributes[Stats.ExperienceRate] * player.GameContext.ExperienceRate);
                player.AddExperience(exp, killedObject);
            }
        }

        return randomizedTotalExperience;
    }

    private void ExitParty(IPartyMember player, byte index)
    {
        if (this.PartyList.Count < 3 || Equals(this.PartyMaster, player))
        {
            this.Dispose();
            return;
        }

        this.PartyList.Remove(player);
        player.Party = null;
        try
        {
            player.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()?.PartyMemberRemoved(index);
        }
        catch (Exception ex)
        {
            this._logger.LogDebug("Error when calling PartyMemberRemoved. Already disconnected?", ex);
        }

        this.SendPartyList();
    }

    private void HealthUpdate_Elapsed(object? state)
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
                partyMaster.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>()?.UpdatePartyHealth();
                for (var i = this.PartyList.Count - 1; i >= 1; i--)
                {
                    var member = this.PartyList[i];
                    var plugIn = member.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>();
                    if (plugIn?.IsHealthUpdateNeeded() ?? false)
                    {
                        plugIn.UpdatePartyHealth();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogDebug("Unexpected error during health update", ex);
        }
    }

    private void SendPartyList()
    {
        if (this.PartyList.Count == 0)
        {
            return;
        }

        for (byte i = 0; i < this.PartyList.Count; i++)
        {
            try
            {
                this.PartyList[i].ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()?.UpdatePartyList();
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error sending party list update");
            }
        }
    }
}