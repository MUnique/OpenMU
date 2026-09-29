// <copyright file="ImperialGuardianViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.MiniGames;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using ImperialGuardianEnterResult = MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian.ImperialGuardianEnterResult;
using ImperialGuardianResult = MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian.ImperialGuardianResult;

/// <summary>
/// The default implementation of the <see cref="IImperialGuardianViewPlugIn"/> which
/// sends the imperial guardian event packets (0xF7 0x02, 0x04 and 0x06) to the client.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ImperialGuardianViewPlugIn_Name), Description = nameof(PlugInResources.ImperialGuardianViewPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9B47E2C1-6D35-4A80-B9F2-4E1C8D7A3056")]
public sealed class ImperialGuardianViewPlugIn : IImperialGuardianViewPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImperialGuardianViewPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ImperialGuardianViewPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask ShowEnterResultAsync(ImperialGuardianEnterResult result, byte day, int zone, ImperialGuardianWeather weather, TimeSpan remainingTime)
    {
        // The values of the packet enums are the same as the ones of the game logic. The client shows the zone starting at 1.
        await this._player.Connection.SendImperialGuardianEnterResultAsync(
            (Network.Packets.ServerToClient.ImperialGuardianEnterResult.EnterResult)result,
            day,
            (byte)(zone + 1),
            (Network.Packets.ServerToClient.ImperialGuardianEnterResult.WeatherType)weather,
            ToMilliseconds(remainingTime)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowTimerAsync(ImperialGuardianTimerType type, TimeSpan remainingTime, int monsterCount)
    {
        await this._player.Connection.SendImperialGuardianTimerAsync(
            (ImperialGuardianTimer.TimerType)type,
            ToMilliseconds(remainingTime),
            (byte)Math.Clamp(monsterCount, byte.MinValue, byte.MaxValue)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowResultAsync(ImperialGuardianResult result, int experience)
    {
        await this._player.Connection.SendImperialGuardianResultAsync(
            (Network.Packets.ServerToClient.ImperialGuardianResult.ResultType)result,
            (uint)Math.Max(0, experience)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ShowMonsterSkillAsync(IAttacker monster, IAttackable target, short skillNumber)
    {
        // The client searches the target by the id as it is, so the flag for a successful skill can't be set.
        await this._player.Connection.SendMonsterSkillAnimationAsync((ushort)skillNumber, monster.GetId(this._player), target.GetId(this._player)).ConfigureAwait(false);
    }

    private static uint ToMilliseconds(TimeSpan time)
    {
        return (uint)Math.Clamp(time.TotalMilliseconds, 0, uint.MaxValue);
    }
}
