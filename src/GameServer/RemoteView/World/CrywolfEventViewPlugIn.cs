// <copyright file="CrywolfEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="ICrywolfEventViewPlugIn"/> which
/// sends the crywolf event packets (0xBD) to the client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CrywolfEventViewPlugIn_Name), Description = nameof(PlugInResources.CrywolfEventViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7B2E9D41-5C63-4F8A-A1D7-2E9F4B6C8A15")]
public sealed class CrywolfEventViewPlugIn : ICrywolfEventViewPlugIn
{
    /// <summary>
    /// The key of the first altar, which the client expects. It uses the key minus this value as index into its altar states.
    /// </summary>
    private const ushort FirstAltarKey = 317;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrywolfEventViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public CrywolfEventViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowStateAsync(CrywolfOccupationState occupation, CrywolfState state)
    {
        // The states of the packet have the same values as the ones of the game logic.
        await this._player.Connection.SendCrywolfInfoAsync((CrywolfInfo.OccupationState)(byte)occupation, (CrywolfInfo.CrywolfState)(byte)state).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowStatueAndAltarsAsync(int statueShieldPercentage, IReadOnlyList<byte> altarStates)
    {
        byte GetAltarState(int index) => index < altarStates.Count ? altarStates[index] : (byte)0;

        await this._player.Connection.SendCrywolfStatueAndAltarInfoAsync(
            (uint)Math.Clamp(statueShieldPercentage, 0, 100),
            GetAltarState(0),
            GetAltarState(1),
            GetAltarState(2),
            GetAltarState(3),
            GetAltarState(4)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowContractResultAsync(bool success, int altarIndex, byte altarState)
    {
        await this._player.Connection.SendCrywolfContractResultAsync(success, altarState, (ushort)(FirstAltarKey + Math.Clamp(altarIndex, 0, 4))).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowRemainingTimeAsync(TimeSpan remainingTime)
    {
        // The client counts down the seconds of the current minute by itself, so it gets the started minutes.
        var remainingSeconds = Math.Max(0, (int)Math.Ceiling(remainingTime.TotalSeconds));
        var minutes = remainingSeconds > 0 ? (remainingSeconds - 1) / 60 : 0;
        await this._player.Connection.SendCrywolfLeftTimeAsync((byte)Math.Min(minutes / 60, byte.MaxValue), (byte)(minutes % 60)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBossMonsterInfoAsync(int balgassHealthPercentage, int darkElfCount)
    {
        await this._player.Connection.SendCrywolfBossMonsterInfoAsync(unchecked((uint)balgassHealthPercentage), (byte)Math.Clamp(darkElfCount, 0, byte.MaxValue)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowPersonalRankAsync(int rank, int experience)
    {
        // The client shows images of the ranks D to S, so it must not get other values.
        await this._player.Connection.SendCrywolfPersonalRankAsync((CrywolfPersonalRank.CrywolfRank)Math.Clamp(rank, 0, 4), (uint)Math.Max(0, experience)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowHeroListAsync(IReadOnlyList<CrywolfHero> heroes)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        // The client uses the rank as array index of five entries.
        var count = Math.Min(heroes.Count, 5);

        int Write()
        {
            var size = CrywolfHeroListRef.GetRequiredSize(count);
            var packet = new CrywolfHeroListRef(connection.Output.GetSpan(size)[..size])
            {
                HeroCount = (byte)count,
            };

            for (var i = 0; i < count; i++)
            {
                var hero = heroes[i];
                var entry = packet[i];
                entry.Rank = (byte)i;
                entry.Name = hero.Name;
                entry.Score = (uint)Math.Max(0, hero.Score);
                entry.CharacterClass = hero.CharacterClassNumber;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowChaosRateBenefitAsync(byte rate)
    {
        await this._player.Connection.SendCrywolfBenefitPlusChaosRateAsync(rate).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowBallistaAttackAsync(NonPlayerCharacter ballista, Point target)
    {
        await this._player.Connection.SendCrywolfRegionMonsterAttackAsync((ushort)ballista.Definition.Number, ballista.Position.X, ballista.Position.Y, target.X, target.Y).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowEffectAsync(NonPlayerCharacter npc, CrywolfEffect effect, bool isActive)
    {
        await this._player.Connection.SendMagicEffectStatusAsync(isActive, npc.GetId(this._player), (byte)effect).ConfigureAwait(false);
    }
}
