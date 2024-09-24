using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

namespace MUnique.OpenMU.GameServer.RemoteView.Character;

/// <summary>
/// The extended implementation of the <see cref="IAddExperiencePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(AddExperienceExtendedPlugIn), "The extended implementation of the IAddExperiencePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("2C5AAEF1-47C9-498F-B3C1-D0F1B9AF0496")]
[MinimumClient(106, 3, ClientLanguage.Invariant)]
public class AddExperienceExtendedPlugIn : IAddExperiencePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddExperiencePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AddExperienceExtendedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask AddExperienceAsync(int exp, IAttackable? obj, ExperienceType experienceType)
    {
        uint damage = 0;
        if (obj is not null
            && this._player.Id != obj.LastDeath?.KillerId) // Show Damage only for party members.
        {
            damage = (uint)Math.Min(obj.LastDeath?.FinalHit.HealthDamage ?? 0, uint.MaxValue);
        }

        var killedId = obj?.GetId(this._player) ?? 0;
        var killerId = obj?.LastDeath?.KillerId ?? 0;
        if (killerId == this._player.Id)
        {
            killerId = ViewExtensions.ConstantPlayerId;
        }

        await this._player.Connection.SendExperienceGainedExtendedAsync((byte)experienceType,
                (uint)exp,
                (uint)damage,
                killedId,
                killerId)
            .ConfigureAwait(false);
    }
}