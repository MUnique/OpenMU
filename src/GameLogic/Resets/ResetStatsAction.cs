// <copyright file="ResetStatsAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Login;

/// <summary>
/// Action to reset a character's stats back to base values and refund the invested points.
/// </summary>
public class ResetStatsAction
{
    private readonly Player _player;
    private readonly LogoutAction _logoutAction = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetStatsAction"/> class.
    /// </summary>
    /// <param name="player">Player to reset stats for.</param>
    public ResetStatsAction(Player player)
    {
        this._player = player;
    }

    /// <summary>
    /// Resets the character stats to base values and refunds invested points.
    /// </summary>
    public async ValueTask ResetStatsAsync()
    {
        var statResetFeature = this._player.GameContext.FeaturePlugIns.GetPlugIn<StatResetFeaturePlugIn>();
        if (statResetFeature is null)
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.StatResetNotEnabled)).ConfigureAwait(false);
            return;
        }

        if (this._player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.CantResetStatsWithOpenedWindows)).ConfigureAwait(false);
            return;
        }

        if (this._player.Attributes is null || this._player.SelectedCharacter is null)
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.NotEnteredTheGame)).ConfigureAwait(false);
            return;
        }

        var configuration = statResetFeature.Configuration;
        if (configuration is null)
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.StatResetNotConfigured)).ConfigureAwait(false);
            return;
        }

        if (!this._player.IsAtSafezone())
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.CantResetStatsNotInSafezone)).ConfigureAwait(false);
            return;
        }

        if (this._player.Level < configuration.RequiredLevel)
        {
            await this.ShowMessageAsync(nameof(PlayerMessage.RequiredLevelForStatReset), configuration.RequiredLevel).ConfigureAwait(false);
            return;
        }

        if (!await this.TryConsumeCostsAsync(configuration).ConfigureAwait(false))
        {
            return;
        }

        this.ResetAttributes(configuration);

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
            await this.UpdateClientStatsAsync().ConfigureAwait(false);
        }
    }

    private void ResetAttributes(StatResetConfiguration configuration)
    {
        var selectedCharacter = this._player.SelectedCharacter!;
        var investedPoints = 0;

        foreach (var statDef in selectedCharacter.CharacterClass!.StatAttributes.Where(s => s.IncreasableByPlayer))
        {
            var currentValue = (int)this._player.Attributes![statDef.Attribute!];
            var baseValue = (int)statDef.BaseValue;
            investedPoints += currentValue - baseValue;
            this._player.Attributes[statDef.Attribute] = baseValue;
        }

        selectedCharacter.LevelUpPoints += investedPoints;
    }

    private async ValueTask<bool> TryConsumeCostsAsync(StatResetConfiguration configuration)
    {
        if (configuration.RequiredResetItem is null)
        {
            if (this._player.Money < configuration.RequiredMoney)
            {
                await this.ShowMessageAsync(nameof(PlayerMessage.NotEnoughMoneyForStatReset), configuration.RequiredMoney).ConfigureAwait(false);
                return false;
            }

            if (configuration.RequiredMoney > 0 && !this._player.TryRemoveMoney(configuration.RequiredMoney))
            {
                await this.ShowMessageAsync(nameof(PlayerMessage.NotEnoughMoneyForStatReset), configuration.RequiredMoney).ConfigureAwait(false);
                return false;
            }
        }
        else
        {
            if (this._player.Inventory is null)
            {
                return false;
            }

            var requiredDefinition = configuration.RequiredResetItem;
            var requiredItem = this._player.Inventory.Items
                .FirstOrDefault(item => item.Definition is { } definition
                                        && definition.Group == requiredDefinition.Group
                                        && definition.Number == requiredDefinition.Number);

            if (requiredItem is null)
            {
                await this.ShowMessageAsync(nameof(PlayerMessage.NotEnoughItemsForStatReset), 1, requiredDefinition.Name).ConfigureAwait(false);
                return false;
            }

            if (this._player.Money < configuration.RequiredMoney)
            {
                await this.ShowMessageAsync(nameof(PlayerMessage.NotEnoughMoneyForStatReset), configuration.RequiredMoney).ConfigureAwait(false);
                return false;
            }

            if (configuration.RequiredMoney > 0 && !this._player.TryRemoveMoney(configuration.RequiredMoney))
            {
                await this.ShowMessageAsync(nameof(PlayerMessage.NotEnoughMoneyForStatReset), configuration.RequiredMoney).ConfigureAwait(false);
                return false;
            }

            await this._player.DestroyInventoryItemAsync(requiredItem).ConfigureAwait(false);
        }

        return true;
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

    private async ValueTask UpdateClientStatsAsync()
    {
        await this._player.InvokeViewPlugInAsync<IUpdateCharacterBaseStatsPlugIn>(p => p.UpdateCharacterBaseStatsAsync()).ConfigureAwait(false);
    }

    private async ValueTask ShowMessageAsync(string messageKey, params object?[] args)
    {
        var message = this._player.GetLocalizedMessage(messageKey, args);
        await this._player.ShowBlueMessageAsync(message).ConfigureAwait(false);
    }
}
