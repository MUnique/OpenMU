// <copyright file="PlayerMoneyExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions to move money between the inventory and the vault of a <see cref="Player"/>.
/// </summary>
public static class PlayerMoneyExtensions
{
    /// <summary>
    /// Tries to remove the money from the player inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="value">The value that should be removed.</param>
    /// <returns><c>True</c>, if the player inventory had enough money to remove; Otherwise, <c>false</c>.</returns>
    public static bool TryRemoveMoney(this Player player, int value)
    {
        if (player.Money < value)
        {
            return false;
        }

        player.Money = checked(player.Money - value);
        return true;
    }

    /// <summary>
    /// Tries to add the money to the player inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="value">The value that should be added.</param>
    /// <returns><c>True</c>, if the player inventory had space to add money; Otherwise, <c>false</c>.</returns>
    public static bool TryAddMoney(this Player player, int value)
    {
        if (player.Money + value > player.GameContext?.Configuration?.MaximumInventoryMoney)
        {
            return false;
        }

        if (player.Money + value < 0)
        {
            return false;
        }

        player.Money = checked(player.Money + value);
        return true;
    }

    /// <summary>
    /// Tries to deposit the money from the player inventory.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="value">The value that should be moved to the vault.</param>
    /// <returns><c>True</c>, if the player inventory had enough money to move; Otherwise, <c>false</c>.</returns>
    public static bool TryDepositVaultMoney(this Player player, int value)
    {
        if (player.Vault is null)
        {
            return false;
        }

        if (player.Vault.ItemStorage.Money + value > player.GameContext?.Configuration?.MaximumVaultMoney)
        {
            return false;
        }

        if (player.TryRemoveMoney(value))
        {
            return player.Vault.TryAddMoney(value);
        }

        return false;
    }

    /// <summary>
    /// Tries to take the money from the vault.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="value">The value that should be retrieved from the vault.</param>
    /// <returns><c>True</c>, if the vault had enough money to move and player inventory isn't maximum; Otherwise, <c>false</c>.</returns>
    public static bool TryTakeVaultMoney(this Player player, int value)
    {
        if (player.Vault is null)
        {
            return false;
        }

        if (player.Money + value > player.GameContext?.Configuration?.MaximumInventoryMoney)
        {
            return false;
        }

        if (player.Vault.TryRemoveMoney(value))
        {
            return player.TryAddMoney(value);
        }

        return false;
    }
}
