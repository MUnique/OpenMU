// <copyright file="CastleSiegeNpcUpgradeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Upgrades Castle Siege gates and Guardian Statues.
/// </summary>
public static class CastleSiegeNpcUpgradeAction
{
    /// <summary>
    /// Tries to apply one NPC upgrade level.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="npcNumber">The target NPC number.</param>
    /// <param name="npcIndex">The target NPC instance identifier.</param>
    /// <param name="upgradeType">The requested upgrade type.</param>
    /// <param name="requestedLevel">The requested target level.</param>
    /// <returns>The operation result.</returns>
    public static async ValueTask<CastleSiegeNpcOperationResult> UpgradeAsync(
        Player player,
        CastleSiegeContext? context,
        uint npcNumber,
        uint npcIndex,
        CastleSiegeUpgradeType upgradeType,
        byte requestedLevel)
    {
        var result = await UpgradeCoreAsync(
                player,
                context,
                npcNumber,
                npcIndex,
                upgradeType,
                requestedLevel)
            .ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<ICastleSiegeNpcOperationResultPlugIn>(
                view => view.ShowUpgradeResultAsync(result, npcNumber, npcIndex, upgradeType, requestedLevel))
            .ConfigureAwait(false);
        return result;
    }

    private static async ValueTask<CastleSiegeNpcOperationResult> UpgradeCoreAsync(
        Player player,
        CastleSiegeContext? context,
        uint npcNumber,
        uint npcIndex,
        CastleSiegeUpgradeType upgradeType,
        byte requestedLevel)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return CastleSiegeNpcOperationResult.Failed;
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState == CastleSiegeState.Start)
            {
                return CastleSiegeNpcOperationResult.Failed;
            }

            if (!await CastleSiegeNpcAuthorization.IsOwnerOfficerAsync(player, context).ConfigureAwait(false))
            {
                return CastleSiegeNpcOperationResult.NotAuthorized;
            }

            if (context.NpcController.FindDefenseStructure(npcNumber, npcIndex) is not { IsAlive: true } runtime
                || runtime.PersistedState is not { } state)
            {
                return CastleSiegeNpcOperationResult.NpcNotFound;
            }

            var monsterNumber = runtime.Definition.MonsterDefinition?.Number ?? 0;
            if (context.Configuration.GetUpgrades(monsterNumber, upgradeType) is not { } definitions)
            {
                return CastleSiegeNpcOperationResult.InvalidUpgradeType;
            }

            var currentLevel = GetLevel(state, upgradeType);
            if (requestedLevel != currentLevel + 1
                || definitions.FirstOrDefault(definition => definition.Level == requestedLevel) is not { } upgrade)
            {
                return CastleSiegeNpcOperationResult.InvalidUpgradeValue;
            }

            var jewels = player.Inventory?.Items
                .Where(item => item.Definition is { } definition
                               && new ItemIdentifier(definition.Number, definition.Group) == ItemConstants.JewelOfGuardian)
                .Take(upgrade.RequiredJewelOfGuardianCount)
                .ToList()
                ?? [];
            if (player.Money < upgrade.RequiredZen)
            {
                return CastleSiegeNpcOperationResult.InsufficientMoney;
            }

            if (jewels.Count < upgrade.RequiredJewelOfGuardianCount)
            {
                return CastleSiegeNpcOperationResult.RequirementNotMet;
            }

            if (!player.TryRemoveMoney(upgrade.RequiredZen))
            {
                return CastleSiegeNpcOperationResult.InsufficientMoney;
            }

            foreach (var jewel in jewels)
            {
                await player.DestroyInventoryItemAsync(jewel).ConfigureAwait(false);
            }

            var oldMaximumHealth = context.NpcController.GetMaximumHealth(runtime);
            SetLevel(state, upgradeType, requestedLevel);
            if (runtime.SpawnedInstance is CastleSiegeAttackableNpc structure)
            {
                structure.ApplyPersistedUpgrades(true);
                state.CurrentHp = structure.Health;
            }
            else if (upgradeType == CastleSiegeUpgradeType.Life)
            {
                var newMaximumHealth = context.NpcController.GetMaximumHealth(runtime);
                var adjustedHealth = (long)state.CurrentHp + newMaximumHealth - oldMaximumHealth;
                state.CurrentHp = (int)Math.Clamp(adjustedHealth, 0L, newMaximumHealth);
            }
            else
            {
                // Defense and regeneration upgrades do not alter the persisted health of an unspawned structure.
            }

            await context.SaveNpcStatesAsync().ConfigureAwait(false);
            return CastleSiegeNpcOperationResult.Success;
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private static byte GetLevel(CastleSiegeNpcState state, CastleSiegeUpgradeType type)
    {
        return type switch
        {
            CastleSiegeUpgradeType.Defense => state.DefenseLevel,
            CastleSiegeUpgradeType.Regen => state.RegenLevel,
            CastleSiegeUpgradeType.Life => state.LifeLevel,
            _ => byte.MaxValue,
        };
    }

    private static void SetLevel(CastleSiegeNpcState state, CastleSiegeUpgradeType type, byte level)
    {
        switch (type)
        {
            case CastleSiegeUpgradeType.Defense:
                state.DefenseLevel = level;
                break;
            case CastleSiegeUpgradeType.Regen:
                state.RegenLevel = level;
                break;
            case CastleSiegeUpgradeType.Life:
                state.LifeLevel = level;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown Castle Siege upgrade type.");
        }
    }
}
