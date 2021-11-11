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
    public void ResetCharacter()
    {
        var resetFeature = this._player.GameContext.FeaturePlugIns.GetPlugIn<ResetFeaturePlugIn>();
        if (resetFeature is null)
        {
            this.ShowMessage("Reset is not enabled.");
            return;
        }

        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld && this._npc is null)
        {
            this.ShowMessage("Cannot do reset with any windows opened.");
            return;
        }

        if (this._player.Attributes is null || this._player.SelectedCharacter is null)
        {
            this.ShowMessage("Not entered the game.");
            return;
        }

        var configuration = resetFeature.Configuration;
        if (configuration is null)
        {
            this.ShowMessage("Reset is not configured.");
            return;
        }

        if (this._player.Level < configuration.RequiredLevel)
        {
            this.ShowMessage($"Required level for reset is {configuration.RequiredLevel}.");
            return;
        }

        if (configuration.ResetLimit > 0 && (this.GetResetCount() + 1) > configuration.ResetLimit)
        {
            this.ShowMessage($"Maximum resets of {configuration.ResetLimit} reached.");
            return;
        }

        if (!this.TryConsumeMoney(configuration))
        {
            return;
        }

        this._player.Attributes[Stats.Resets]++;
        this._player.Attributes[Stats.Level] = configuration.LevelAfterReset;
        this._player.SelectedCharacter.Experience = 0;
        this.UpdateStats(configuration);
        this.MoveHome();
        this.Logout();
    }

    private void ShowMessage(string message)
    {
        if (this._npc is null)
        {
            this._player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage(message, MessageType.BlueNormal);
        }
        else
        {
            this._player.ViewPlugIns.GetPlugIn<IShowMessageOfObjectPlugIn>()?.ShowMessageOfObject(message, this._npc);
        }
    }

    private int GetResetCount()
    {
        return (int)this._player.Attributes![Stats.Resets];
    }

    private bool TryConsumeMoney(ResetConfiguration configuration)
    {
        var calculatedRequiredZen = configuration.RequiredMoney;
        if (configuration.MultiplyRequiredMoneyByResetCount)
        {
            calculatedRequiredZen *= this.GetResetCount() + 1;
        }

        if (!this._player.TryRemoveMoney(calculatedRequiredZen))
        {
            this.ShowMessage($"You don't have enough money for reset, required zen is {calculatedRequiredZen}");
            return false;
        }

        return true;
    }

    private void UpdateStats(ResetConfiguration configuration)
    {
        var calculatedPointsPerReset = configuration.PointsPerReset;
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

        this._player.SelectedCharacter!.LevelUpPoints = calculatedPointsPerReset;
    }

    private void MoveHome()
    {
        var homeMapDef = this._player.SelectedCharacter!.CharacterClass!.HomeMap;
        if (homeMapDef is { }
            && this._player.GameContext.GetMap((ushort)homeMapDef.Number) is { SafeZoneSpawnGate: { } spawnGate })
        {
            this._player.SelectedCharacter.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            this._player.SelectedCharacter.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
            this._player.SelectedCharacter.CurrentMap = spawnGate.Map;
            this._player.Rotation = spawnGate.Direction;
        }
    }

    private void Logout()
    {
        this._logoutAction.Logout(this._player, LogoutType.BackToCharacterSelection);
    }
}