// <copyright file="PetCommandRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Pet;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for pet command request packets.
/// </summary>
[PlugIn(nameof(PetCommandRequestHandlerPlugIn), "Handler for pet command request packets.")]
[Guid("9C084343-72D2-4517-9267-4A270CB53146")]
[MinimumClient(0, 97, ClientLanguage.Invariant)]
internal class PetCommandRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <summary>
    /// The offset of the first <see cref="PetCommandMode"/>, which is used by the client.
    /// I guess it's the same number range as the skills, because when you send
    /// a skill list with id 120 upwards, you get these attacks.
    /// When logging off, the clients sends us a 120, too.
    /// </summary>
    private const byte SkillOffset = 120;

    private readonly SetPetBehaviourRequestAction _requestAction = new();

    /// <inheritdoc />
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => PetCommandRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PetCommandRequest message = packet;

        // The pet type could be interesting here as well.
        // However, the game currently only supports the dark raven and only one offensive pet per player.
        await this._requestAction.RequestChangeAsync(player, message.TargetId, Convert(message.CommandMode)).ConfigureAwait(false);
    }

    private static PetBehaviour Convert(PetCommandMode mode)
    {
        return (PetCommandMode)((byte)mode % SkillOffset) switch
        {
            PetCommandMode.Normal => PetBehaviour.Idle,
            PetCommandMode.AttackRandom => PetBehaviour.AttackRandom,
            PetCommandMode.AttackTarget => PetBehaviour.AttackTarget,
            PetCommandMode.AttackWithOwner => PetBehaviour.AttackWithOwner,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }
}