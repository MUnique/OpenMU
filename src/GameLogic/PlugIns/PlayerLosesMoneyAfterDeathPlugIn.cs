// <copyright file="PlayerLosesMoneyAfterDeathPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin decreases the money after the player has been killed by a monster.
/// </summary>
[PlugIn("Player money loss after death", "This plugin decreases the money (zen) after the player has been killed by a monster.")]
[Guid("E3A4D8DD-C017-4A44-853B-569B1F12C350")]
public class PlayerLosesMoneyAfterDeathPlugIn : IAttackableGotKilledPlugIn, ISupportCustomConfiguration<PlayerLosesMoneyAfterDeathPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc/>
    public PlayerLosesMoneyAfterDeathPlugInConfiguration? Configuration { get; set; }

    /// <summary>
    /// Is called when an <see cref="IAttackable" /> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable" />.</param>
    /// <param name="killer">The killer.</param>
    public async ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killer is not Monster || killed is not Player player
                                  || player.SelectedCharacter is not { } selectedCharacter
                                  || selectedCharacter.CharacterClass is not { } characterClass
                                  || player.Attributes is not { } attributes)
        {
            return;
        }

        if (player.CurrentMiniGame is not null)
        {
            return;
        }

        this.Configuration ??= CreateDefaultConfiguration();
        var losses = this.GetLossPercentage((int)attributes[Stats.Level], characterClass.IsMasterClass);
        var inventoryLoss = (int)(player.Money * losses.InventoryLoss / 100.0);
        var vaultLoss = (int)((player.Account?.Vault?.Money ?? 0) * losses.VaultLoss / 100.0);

        inventoryLoss = Math.Min(inventoryLoss, player.Money);
        vaultLoss = Math.Min(vaultLoss, player.Account?.Vault?.Money ?? 0);
        player.TryRemoveMoney(inventoryLoss);
        player.TryTakeVaultMoney(vaultLoss);
    }

    /// <inheritdoc/>
    public object CreateDefaultConfig()
    {
        return CreateDefaultConfiguration();
    }

    private static PlayerLosesMoneyAfterDeathPlugInConfiguration CreateDefaultConfiguration()
    {
        return new()
        {
            LossesPerLevel =
            [
                new(10, false, 1, 1),
                new(150, false, 2, 2),
                new(220, false, 3, 3),
                new(0, true, 4, 4),
            ],
        };
    }

    private (double InventoryLoss, double VaultLoss) GetLossPercentage(int playerLevel, bool isMaster)
    {
        var lossEntry = this.Configuration?.LossesPerLevel
            .Where(l => l.IsMaster == isMaster)
            .OrderByDescending(l => l.MinimumLevel)
            .FirstOrDefault(l => l.MinimumLevel <= playerLevel);
        return (lossEntry?.InventoryMoneyLossPercentage ?? 0, lossEntry?.VaultMoneyLossPercentage ?? 0);
    }
}