// <copyright file="ImperialGuardianRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;
using MUnique.OpenMU.GameServer.RemoteView.MiniGames;
using ImperialGuardianEnterResult = MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian.ImperialGuardianEnterResult;
using ImperialGuardianResult = MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian.ImperialGuardianResult;

/// <summary>
/// Tests the packets which are sent to the client during the imperial guardian event.
/// </summary>
[TestFixture]
public class ImperialGuardianRemoteViewTests
{
    /// <summary>
    /// Tests the packets of the view, which have to match the (not packed) structures of the client.
    /// </summary>
    [Test]
    public async Task PacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ImperialGuardianViewPlugIn(player);

        await view.ShowEnterResultAsync(ImperialGuardianEnterResult.Success, 7, 2, ImperialGuardianWeather.Storm, TimeSpan.FromSeconds(60)).ConfigureAwait(false);
        await view.ShowTimerAsync(ImperialGuardianTimerType.TimeAttack, TimeSpan.FromMilliseconds(599_500), 300).ConfigureAwait(false);
        await view.ShowResultAsync(ImperialGuardianResult.Success, 123_456).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data.Length, Is.EqualTo(12 + 16 + 12));

        // The padding bytes of the not packed client structures aren't read by the client, so they're not checked.
        // F7 02: result, day, zone (starting at 1), weather, remaining milliseconds at offset 8.
        Assert.That(data[..12], Is.EqualTo(new byte[] { 0xC1, 12, 0xF7, 0x02, 0, 7, 3, 3, 0x60, 0xEA, 0x00, 0x00 }));

        // F7 04: type, remaining milliseconds at offset 8, monster count (at most 255) at offset 12.
        var timer = data[12..28];
        Assert.That(timer[..5], Is.EqualTo(new byte[] { 0xC1, 16, 0xF7, 0x04, 2 }));
        Assert.That(timer[8..13], Is.EqualTo(new byte[] { 0xCC, 0x25, 0x09, 0x00, 255 }));

        // F7 06: result, experience at offset 8.
        var result = data[28..];
        Assert.That(result[..5], Is.EqualTo(new byte[] { 0xC1, 12, 0xF7, 0x06, 2 }));
        Assert.That(result[8..], Is.EqualTo(new byte[] { 0x40, 0xE2, 0x01, 0x00 }));
    }
}
