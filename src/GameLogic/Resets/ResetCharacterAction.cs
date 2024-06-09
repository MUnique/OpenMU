// <copyright file="ResetCharacterAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views;
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
    private readonly LogoutAction _logoutAction = new ();

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
            await this.ShowMessageAsync("Reset is not enabled.").ConfigureAwait(false);
            return;
        }

        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld && this._npc is null)
        {
            await this.ShowMessageAsync("Cannot do reset with any windows opened.").ConfigureAwait(false);
            return;
        }

        if (this._player.Attributes is null || this._player.SelectedCharacter is null)
        {
            await this.ShowMessageAsync("Not entered the game.").ConfigureAwait(false);
            return;
        }

        var configuration = resetFeature.Configuration;
        if (configuration is null)
        {
            await this.ShowMessageAsync("Reset is not configured.").ConfigureAwait(false);
            return;
        }

        if (this._player.Level < configuration.RequiredLevel)
        {
            await this.ShowMessageAsync($"Required level for reset is {configuration.RequiredLevel}.").ConfigureAwait(false);
            return;
        }

        if (configuration.ResetLimit > 0 && (this.GetResetCount() + 1) > configuration.ResetLimit)
        {
            await this.ShowMessageAsync($"Maximum resets of {configuration.ResetLimit} reached.").ConfigureAwait(false);
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
        await this.MoveHomeAsync().ConfigureAwait(false);
        await this._logoutAction.LogoutAsync(this._player, LogoutType.BackToCharacterSelection).ConfigureAwait(false);
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
            await this.ShowMessageAsync($"You don't have enough money for reset, required zen is {calculatedRequiredZen}").ConfigureAwait(false);
            return false;
        }

        return true;
    }

    private void UpdateStats(ResetConfiguration configuration)
    {
        var calculatedPointsPerReset = this.GetResetPoints(configuration);
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

        this._player.SelectedCharacter!.LevelUpPoints = Math.Max(0, calculatedPointsPerReset);
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
}