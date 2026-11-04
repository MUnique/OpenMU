// <copyright file="IAttackableGotKilledPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when an <see cref="IAttackable"/> object got killed.
/// </summary>
[Guid("89CC1180-8FB4-4194-B895-D1F8D88124F9")]
[PlugInPoint("Attackable got killed", "Plugins which will be executed when an attackable object got killed by an attacker.")]
public interface IAttackableGotKilledPlugIn
{
    /// <summary>
    /// Is called when an <see cref="IAttackable"/> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable"/>.</param>
    /// <param name="killer">The killer.</param>
    ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer);
}