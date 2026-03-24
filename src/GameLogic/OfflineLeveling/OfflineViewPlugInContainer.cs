// <copyright file="OfflineViewPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin container for the <see cref="OfflineLevelingPlayer"/>, providing stub implementations
/// of client-facing views necessary for successful safe-zone respawns.
/// </summary>
internal sealed class OfflineViewPlugInContainer : ICustomPlugInContainer<IViewPlugIn>
{
    private readonly IRespawnAfterDeathPlugIn _respawnPlugIn = new OfflineRespawnPlugIn();
    private readonly IMapChangePlugIn _mapChangePlugIn;
    private readonly OfflinePartyHealthViewPlugIn _partyHealthPlugIn;
    private readonly OfflineUpdatePartyListPlugIn _updatePartyListPlugIn;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfflineViewPlugInContainer"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    public OfflineViewPlugInContainer(OfflineLevelingPlayer player)
    {
        this._mapChangePlugIn = new OfflineMapChangePlugIn(player);
        this._partyHealthPlugIn = new OfflinePartyHealthViewPlugIn(player);
        this._updatePartyListPlugIn = new OfflineUpdatePartyListPlugIn(player);
    }

    /// <inheritdoc/>
    public T? GetPlugIn<T>()
        where T : class, IViewPlugIn
    {
        if (typeof(T) == typeof(IRespawnAfterDeathPlugIn))
        {
            return (T)this._respawnPlugIn;
        }

        if (typeof(T) == typeof(IMapChangePlugIn))
        {
            return (T)this._mapChangePlugIn;
        }

        if (typeof(T) == typeof(IPartyHealthViewPlugIn))
        {
            return (T)(IPartyHealthViewPlugIn)this._partyHealthPlugIn;
        }

        if (typeof(T) == typeof(IUpdatePartyListPlugIn))
        {
            return (T)(IUpdatePartyListPlugIn)this._updatePartyListPlugIn;
        }

        return null;
    }

    private sealed class OfflinePartyHealthViewPlugIn : IPartyHealthViewPlugIn
    {
        private readonly OfflineLevelingPlayer _player;
        private byte[]? _healthValues;

        public OfflinePartyHealthViewPlugIn(OfflineLevelingPlayer player)
        {
            this._player = player;
        }

        public bool IsHealthUpdateNeeded()
        {
            var partyList = this._player.Party?.PartyList;
            if (partyList is null)
            {
                this._healthValues?.ClearToDefaults();
                return false;
            }

            this._healthValues ??= new byte[partyList.Count];

            bool updated = false;
            for (int i = 0; i < partyList.Count; i++)
            {
                var member = partyList[i];
                double value = member.MaximumHealth > 0
                    ? (double)member.CurrentHealth / member.MaximumHealth
                    : 0;
                var newValue = (byte)(value * 10);

                if (i < this._healthValues.Length && this._healthValues[i] != newValue)
                {
                    this._healthValues[i] = newValue;
                    updated = true;
                }
            }

            return updated;
        }

        public async ValueTask UpdatePartyHealthAsync()
        {
            var partyList = this._player.Party?.PartyList;
            if (partyList is null)
            {
                return;
            }

            foreach (var member in partyList)
            {
                if (member == this._player)
                {
                    continue;
                }

                await member.InvokeViewPlugInAsync<IPartyHealthViewPlugIn>(p => p.UpdatePartyHealthAsync()).ConfigureAwait(false);
            }
        }
    }

    private sealed class OfflineUpdatePartyListPlugIn : IUpdatePartyListPlugIn
    {
        private readonly OfflineLevelingPlayer _player;

        public OfflineUpdatePartyListPlugIn(OfflineLevelingPlayer player)
        {
            this._player = player;
        }

        public async ValueTask UpdatePartyListAsync()
        {
            var partyList = this._player.Party?.PartyList;
            if (partyList is null)
            {
                return;
            }

            foreach (var member in partyList)
            {
                if (member == this._player)
                {
                    continue;
                }

                await member.InvokeViewPlugInAsync<IUpdatePartyListPlugIn>(p => p.UpdatePartyListAsync()).ConfigureAwait(false);
            }
        }
    }
}
