// <copyright file="AddExperiencePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IAddExperiencePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("AddExperiencePlugIn", "The default implementation of the IAddExperiencePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("cc400edd-3540-4727-9b23-8c0ded4f0b00")]
public class AddExperiencePlugIn : IAddExperiencePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddExperiencePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AddExperiencePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AddExperienceAsync(int exp, IAttackable? obj, ExperienceType experienceType)
    {
        var remainingExperience = exp;
        ushort damage = 0;
        if (obj is not null && obj.Id != obj.LastDeath?.KillerId)
        {
            damage = (ushort)Math.Min(obj.LastDeath?.FinalHit.HealthDamage ?? 0, ushort.MaxValue);
        }

        var id = (ushort)(obj.GetId(this._player) | 0x8000);
        while (remainingExperience > 0)
        {
            // We send multiple exp packets if the value is bigger than ushort.MaxValue, because that's all what the packet can carry.
            // On a normal exp server this should never be an issue, but with higher settings, it fixes the problem that the exp bar
            // shows less exp than the player actually gained.
            ushort sendExp = remainingExperience > ushort.MaxValue ? ushort.MaxValue : (ushort)remainingExperience;
            await this._player.Connection.SendExperienceGainedAsync(id, sendExp, damage).ConfigureAwait(false);
            damage = 0; // don't send damage again
            remainingExperience -= sendExp;
        }
    }
}