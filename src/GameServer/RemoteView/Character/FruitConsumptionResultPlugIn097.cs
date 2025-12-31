// <copyright file="FruitConsumptionResultPlugIn097.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fruit consumption response plugin for 0.97 clients.
/// </summary>
[PlugIn(nameof(FruitConsumptionResultPlugIn097), "Fruit consumption response plugin for 0.97 clients.")]
[Guid("6BE7F3D4-4A56-4AAB-9E5F-0B1D59A6A169")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
[MaximumClient(0, 97, ClientLanguage.Invariant)]
public class FruitConsumptionResultPlugIn097 : IFruitConsumptionResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="FruitConsumptionResultPlugIn097"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public FruitConsumptionResultPlugIn097(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResponseAsync(FruitConsumptionResult result, byte statPoints, AttributeDefinition statAttribute)
    {
        await this._player.Connection.SendFruitConsumptionResponseAsync(Convert(result), statPoints, Convert(statAttribute)).ConfigureAwait(false);
    }

    private static FruitConsumptionResponse.FruitConsumptionResult Convert(FruitConsumptionResult result)
    {
        return result switch
        {
            FruitConsumptionResult.PlusSuccess => FruitConsumptionResponse.FruitConsumptionResult.PlusSuccess,
            FruitConsumptionResult.PlusFailed => FruitConsumptionResponse.FruitConsumptionResult.PlusFailed,
            FruitConsumptionResult.PlusPrevented => FruitConsumptionResponse.FruitConsumptionResult.PlusPrevented,
            FruitConsumptionResult.MinusSuccess => FruitConsumptionResponse.FruitConsumptionResult.MinusSuccess,
            FruitConsumptionResult.MinusFailed => FruitConsumptionResponse.FruitConsumptionResult.MinusFailed,
            FruitConsumptionResult.MinusPrevented => FruitConsumptionResponse.FruitConsumptionResult.MinusPrevented,
            FruitConsumptionResult.MinusSuccessCashShopFruit => FruitConsumptionResponse.FruitConsumptionResult.MinusSuccessCashShopFruit,
            FruitConsumptionResult.PlusPreventedByMaximum => FruitConsumptionResponse.FruitConsumptionResult.PlusPreventedByMaximum,
            FruitConsumptionResult.MinusPreventedByMaximum => FruitConsumptionResponse.FruitConsumptionResult.MinusPreventedByMaximum,
            FruitConsumptionResult.MinusPreventedByDefault => FruitConsumptionResponse.FruitConsumptionResult.MinusPreventedByDefault,
            _ => throw new ArgumentException($"Unknown result {result}", nameof(result)),
        };
    }

    private static FruitConsumptionResponse.FruitStatType Convert(AttributeDefinition statAttribute)
    {
        if (statAttribute == Stats.BaseEnergy)
        {
            return FruitConsumptionResponse.FruitStatType.Energy;
        }

        if (statAttribute == Stats.BaseAgility)
        {
            return FruitConsumptionResponse.FruitStatType.Agility;
        }

        if (statAttribute == Stats.BaseStrength)
        {
            return FruitConsumptionResponse.FruitStatType.Strength;
        }

        if (statAttribute == Stats.BaseVitality)
        {
            return FruitConsumptionResponse.FruitStatType.Vitality;
        }

        if (statAttribute == Stats.BaseLeadership)
        {
            return FruitConsumptionResponse.FruitStatType.Leadership;
        }

        throw new ArgumentException($"Unknown stat {statAttribute}", nameof(statAttribute));
    }
}
