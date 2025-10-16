// <copyright file="ResetCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to reset a character.
/// </summary>
public class ResetCharacterAction
{
    private readonly Player _player;
    private readonly NonPlayerCharacter? _npc;
    private readonly LogoutAction _logoutAction = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetCharacterAction"/> class.
    /// </summary>
    /// <param name="player">Player to reset.</param>
    /// <param name="npc">NPC which the player talks to to initiate the reset action.</param>
    public ResetCharacterAction(Player player, NonPlayerCharacter? npc = null)
    {
        this._player = player;
        this._npc = npc;
    }

    /// <summary>
    /// Reset specific character.
    /// </summary>
    public async ValueTask ResetCharacterAsync()
    {
        var resetFeature = this._player.GameContext.FeaturePlugIns.GetPlugIn<ResetFeaturePlugIn>();
        if (resetFeature is null)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_Disabled", "Reset is not enabled.");
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld && this._npc is null)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_WindowsOpen", "Cannot perform a reset while windows are open.");
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        if (this._player.Attributes is null || this._player.SelectedCharacter is null)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_NotInGame", "You have not entered the game.");
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        var configuration = resetFeature.Configuration;
        if (configuration is null)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_NotConfigured", "Reset feature is not configured.");
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        if (this._player.Level < configuration.RequiredLevel)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_LevelRequirement", "The required level for reset is {0}.", configuration.RequiredLevel);
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        if (configuration.ResetLimit > 0 && (this.GetResetCount() + 1) > configuration.ResetLimit)
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_LimitReached", "Maximum resets reached: {0}.", configuration.ResetLimit);
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return;
        }

        if (!await this.TryConsumeMoneyAsync(configuration).ConfigureAwait(false))
        {
            return;
        }

        this._player.Attributes[Stats.Resets]++;
        this._player.Attributes[Stats.Level] = configuration.LevelAfterReset;
        this._player.SelectedCharacter.Experience = 0;
        this.UpdateStats(configuration);
        if (configuration.MoveHome)
        {
            await this.MoveHomeAsync().ConfigureAwait(false);
        }

        if (configuration.LogOut)
        {
            await this._logoutAction.LogoutAsync(this._player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);
        }
        else
        {
            await this.UpdateClientStatsAsync(configuration).ConfigureAwait(false);
        }
    }

    private ValueTask ShowMessageAsync(string message)
    {
        if (this._npc is null)
        {
            return this._player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal));
        }
        else
        {
            return this._player.InvokeViewPlugInAsync<IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, this._npc));
        }
    }

    private int GetResetCount()
    {
        return (int)this._player.Attributes![Stats.Resets];
    }

    private async ValueTask<bool> TryConsumeMoneyAsync(ResetConfiguration configuration)
    {
        var calculatedRequiredZen = configuration.RequiredMoney;
        if (configuration.MultiplyRequiredMoneyByResetCount)
        {
            calculatedRequiredZen *= this.GetResetCount() + 1;
        }

        if (!this._player.TryRemoveMoney(calculatedRequiredZen))
        {
            var message = this._player.GetLocalizedMessage("Reset_Message_NotEnoughZen", "You don't have enough zen to reset, {0} required.", calculatedRequiredZen);
            await this.ShowMessageAsync(message).ConfigureAwait(false);
            return false;
        }

        return true;
    }

    private void UpdateStats(ResetConfiguration configuration)
    {
        var calculatedPointsPerReset = Math.Max(0, this.GetResetPoints(configuration));
        if (configuration.MultiplyPointsByResetCount)
        {
            calculatedPointsPerReset *= this.GetResetCount();
        }

        if (configuration.ResetStats)
        {
            this._player.SelectedCharacter!.CharacterClass!.StatAttributes
                .Where(s => s.IncreasableByPlayer)
                .ForEach(s => this._player.Attributes![s.Attribute] = s.BaseValue);
        }

        if (configuration.ReplacePointsPerReset)
        {
            this._player.SelectedCharacter!.LevelUpPoints = calculatedPointsPerReset;
        }
        else
        {
            this._player.SelectedCharacter!.LevelUpPoints += calculatedPointsPerReset;
        }
    }

    private int GetResetPoints(ResetConfiguration configuration)
    {
        var pointsPerReset = (int)this._player.Attributes![Stats.PointsPerReset];
        if (pointsPerReset == 0)
        {
            pointsPerReset = configuration.PointsPerReset;
        }

        return pointsPerReset;
    }

    private async ValueTask MoveHomeAsync()
    {
        var homeMapDef = this._player.SelectedCharacter!.CharacterClass!.HomeMap;
        if (homeMapDef is { }
            && await this._player.GameContext.GetMapAsync((ushort)homeMapDef.Number).ConfigureAwait(false) is { SafeZoneSpawnGate: { } spawnGate })
        {
            this._player.SelectedCharacter.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            this._player.SelectedCharacter.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
            this._player.SelectedCharacter.CurrentMap = spawnGate.Map;
            this._player.Rotation = spawnGate.Direction;
        }
    }

    private async ValueTask UpdateClientStatsAsync(ResetConfiguration configuration)
    {
        if (configuration.ResetStats)
        {
            await this._player.InvokeViewPlugInAsync<IUpdateCharacterBaseStatsPlugIn>(p => p.UpdateCharacterBaseStatsAsync()).ConfigureAwait(false);
        }

        await this._player.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
    }
}
