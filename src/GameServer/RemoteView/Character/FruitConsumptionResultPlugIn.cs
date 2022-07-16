// <copyright file="FruitConsumptionResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IFruitConsumptionResponsePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("CharacterFocusedPlugIn", "The default implementation of the IFruitConsumptionResponsePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("0B4D7CA8-181B-4AB5-9522-C826F6311CD8")]
public class FruitConsumptionResultPlugIn : IFruitConsumptionResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="FruitConsumptionResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public FruitConsumptionResultPlugIn(RemotePlayer player) => this._player = player;

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