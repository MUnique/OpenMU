// <copyright file="ICastleSiegeNpcOperationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A view which reports Castle Siege NPC management results.
/// </summary>
public interface ICastleSiegeNpcOperationResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows a defense-structure purchase result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="npcNumber">The NPC number.</param>
    /// <param name="npcIndex">The NPC instance identifier.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowBuyResultAsync(CastleSiegeNpcOperationResult result, uint npcNumber, uint npcIndex);

    /// <summary>
    /// Shows a defense-structure repair result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="npcNumber">The NPC number.</param>
    /// <param name="npcIndex">The NPC instance identifier.</param>
    /// <param name="currentHealth">The current NPC health.</param>
    /// <param name="maximumHealth">The maximum NPC health.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowRepairResultAsync(
        CastleSiegeNpcOperationResult result,
        uint npcNumber,
        uint npcIndex,
        int currentHealth,
        int maximumHealth);

    /// <summary>
    /// Shows a defense-structure upgrade result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="npcNumber">The NPC number.</param>
    /// <param name="npcIndex">The NPC instance identifier.</param>
    /// <param name="upgradeType">The upgrade type.</param>
    /// <param name="upgradeLevel">The requested upgrade level.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowUpgradeResultAsync(
        CastleSiegeNpcOperationResult result,
        uint npcNumber,
        uint npcIndex,
        CastleSiegeUpgradeType upgradeType,
        byte upgradeLevel);

    /// <summary>
    /// Opens the gate-operation interface.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="gateIndex">The gate identifier.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowGateInterfaceAsync(CastleSiegeNpcOperationResult result, ushort gateIndex);

    /// <summary>
    /// Shows a gate operation result.
    /// </summary>
    /// <param name="result">The operation result.</param>
    /// <param name="isOpen">Whether the gate is open after the operation.</param>
    /// <param name="gateIndex">The gate identifier.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowGateOperationResultAsync(
        CastleSiegeNpcOperationResult result,
        bool isOpen,
        ushort gateIndex);

    /// <summary>
    /// Updates the visible state of a gate.
    /// </summary>
    /// <param name="isOpen">Whether the gate is open.</param>
    /// <param name="gateIndex">The gate identifier.</param>
    /// <returns>A task that represents the asynchronous view update.</returns>
    ValueTask ShowGateStateAsync(bool isOpen, ushort gateIndex);
}
