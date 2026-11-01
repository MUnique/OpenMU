// <copyright file="MapPowerUps.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The power-ups which the map a character is on grants to it, see <see cref="GameMapDefinition.CharacterPowerUpDefinitions"/>.
/// </summary>
/// <remarks>
/// The power-ups wrap elements of the map configuration, which lives as long as the server. They have to be
/// released when the character leaves the map or its attribute system is released, otherwise the configuration
/// keeps the attribute system - and with it the whole player - alive.
/// </remarks>
internal sealed class MapPowerUps : IDisposable
{
    private readonly List<PowerUpWrapper> _powerUps = new();

    /// <summary>
    /// Applies the power-ups of the map to the attribute system, replacing the ones of a previous map.
    /// </summary>
    /// <param name="map">The map definition.</param>
    /// <param name="attributes">The attribute system of the character which entered the map.</param>
    public void Apply(GameMapDefinition map, AttributeSystem attributes)
    {
        lock (this._powerUps)
        {
            this.RemoveWithinLock();
            if (map.CharacterPowerUpDefinitions is not { Count: > 0 } powerUpDefinitions)
            {
                return;
            }

            foreach (var powerUpDefinition in powerUpDefinitions.Where(definition => definition.TargetAttribute is not null))
            {
                this._powerUps.AddRange(PowerUpWrapper.CreateByPowerUpDefinition(powerUpDefinition, attributes));
            }
        }
    }

    /// <summary>
    /// Removes the applied power-ups from the attribute system.
    /// </summary>
    public void Remove()
    {
        lock (this._powerUps)
        {
            this.RemoveWithinLock();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Remove();
    }

    private void RemoveWithinLock()
    {
        foreach (var powerUp in this._powerUps)
        {
            powerUp.Dispose();
        }

        this._powerUps.Clear();
    }
}
