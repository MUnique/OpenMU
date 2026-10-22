// <copyright file="CastleSiegeWeaponExplosionHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Accepts the weapon explosion request, which the client sends when the projectile of a fired warfare machine lands.
/// </summary>
/// <remarks>
/// The original server applied the impact damage when this request arrived. OpenMU applies it on its own,
/// after the projectile's flight time (see <see cref="CastleSiegeMachineUseAction"/>), so the request has no effect:
/// <list type="bullet">
///   <item>The damage doesn't depend on the client, so a modified client can't trigger, repeat or suppress it.</item>
///   <item>The impact still happens when the operator disconnects while the projectile is in flight.</item>
///   <item>MuMain doesn't send a usable machine identifier: it packs <c>MAKELONG(o->Skill, o->Kind)</c> into the
///         16-bit field, so only the low byte of the object id arrives, and it skips the request when either byte is zero.</item>
/// </list>
/// The handler exists so that the request is known and documented as intentionally handled by the server-side impact.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeWeaponExplosionHandlerPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeWeaponExplosionHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("22FD0175-4E27-4286-9BED-2C08CE99FD2A")]
[BelongsToGroup(CastleSiegeMachineGroupHandlerPlugIn.GroupKey)]
internal sealed class CastleSiegeWeaponExplosionHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc />
    public byte Key => WeaponExplosionRequest.SubCode;

    /// <inheritdoc />
    public ValueTask HandlePacketAsync(Player player, Memory<byte> packet) => ValueTask.CompletedTask;
}
