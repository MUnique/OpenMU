// <copyright file="WarpGateAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action to warp to another place through a gate.
/// </summary>
public class WarpGateAction
{
    /// <summary>
    /// Enters the gate.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="gate">The enter gate.</param>
    public async ValueTask EnterGateAsync(Player player, EnterGate gate)
    {
        if (await this.IsWarpLegitAsync(player, gate).ConfigureAwait(false))
        {
            if (RaklionPlugIn.GetContext(player.GameContext) is { } raklion
                && !await raklion.CanEnterThroughGateAsync(player, gate.TargetGate!.Map!).ConfigureAwait(false))
            {
                // Like the original game, the player is warped to its current position.
                // The game client shows its loading screen until a map change is completed.
                await this.WarpToCurrentPositionAsync(player).ConfigureAwait(false);
                return;
            }

            await player.WarpToAsync(gate.TargetGate!).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IMapChangePlugIn>(p => p.MapChangeFailedAsync()).ConfigureAwait(false);
        }
    }

    private async ValueTask<bool> IsWarpLegitAsync(Player player, EnterGate? enterGate)
    {
        if (enterGate?.TargetGate?.Map is null)
        {
            return false;
        }

        if (player.SelectedCharacter is null)
        {
            return false;
        }

        var requirement = player.SelectedCharacter?.GetEffectiveMoveLevelRequirement(enterGate.LevelRequirement);
        if (requirement > player.Attributes![Stats.Level])
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.LevelTooLowToEnterMap)).ConfigureAwait(false);
            return false;
        }

        if (enterGate.TargetGate.Map.TryGetRequirementError(player, out var errorMessage))
        {
            await player.ShowBlueMessageAsync(errorMessage).ConfigureAwait(false);
            return false;
        }

        var currentPosition = player.IsWalking ? player.WalkTarget : player.Position;
        var inaccuracy = player.GameContext.Configuration.InfoRange;
        if (player.CurrentMap!.Definition.EnterGates.Contains(enterGate)
            && !(this.IsXInRange(currentPosition, enterGate, inaccuracy)
                 && this.IsYInRange(currentPosition, enterGate, inaccuracy)))
        {
            return false;
        }

        return true;
    }

    private async ValueTask WarpToCurrentPositionAsync(Player player)
    {
        var position = player.IsWalking ? player.WalkTarget : player.Position;
        var currentPosition = new ExitGate
        {
            Map = player.CurrentMap!.Definition,
            X1 = position.X,
            X2 = position.X,
            Y1 = position.Y,
            Y2 = position.Y,
            Direction = player.Rotation,
        };

        await player.WarpToAsync(currentPosition).ConfigureAwait(false);
    }

    private bool IsXInRange(Point currentPosition, Gate gate, byte inaccuracy) => currentPosition.X >= gate.X1 - inaccuracy
                                                                                  && currentPosition.X <= gate.X2 + inaccuracy;

    private bool IsYInRange(Point currentPosition, Gate gate, byte inaccuracy) => currentPosition.Y >= gate.Y1 - inaccuracy
                                                                                  && currentPosition.Y <= gate.Y2 + inaccuracy;
}