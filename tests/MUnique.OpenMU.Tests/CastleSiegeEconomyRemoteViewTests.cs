// <copyright file="CastleSiegeEconomyRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests Castle Siege economy remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeEconomyRemoteViewTests
{
    /// <summary>
    /// Verifies tax, treasury, and Land of Trials response packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeEconomyResponsesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await new CastleSiegeTaxInfoPlugIn(player)
            .ShowTaxInfoAsync(CastleSiegeRequestResult.Success, 3, 2, 123_456_789)
            .ConfigureAwait(false);
        var taxChangeView = new CastleSiegeTaxChangeResultPlugIn(player);
        await taxChangeView
            .ShowTaxChangeResultAsync(
                CastleSiegeRequestResult.Success,
                MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeTaxType.HuntZone,
                300_000)
            .ConfigureAwait(false);
        await taxChangeView
            .ShowTaxRateUpdateAsync(
                MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeTaxType.Store,
                3)
            .ConfigureAwait(false);
        await new CastleSiegeTributeWithdrawResultPlugIn(player)
            .ShowTributeWithdrawResultAsync(CastleSiegeRequestResult.Success, 10_000)
            .ConfigureAwait(false);
        await new CastleSiegeHuntZoneGuardInfoPlugIn(player)
            .ShowHuntZoneGuardInfoAsync(
                CastleSiegeHuntZoneAccessType.OwnerGuildMaster,
                true,
                300_000,
                300_000,
                10_000)
            .ConfigureAwait(false);
        var huntZoneResultView = new CastleSiegeHuntZoneResultPlugIn(player);
        await huntZoneResultView
            .ShowEntranceSettingResultAsync(CastleSiegeRequestResult.Success, true)
            .ConfigureAwait(false);
        await huntZoneResultView.ShowEnterResultAsync(true).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(
            data.Length,
            Is.EqualTo(
                CastleSiegeTaxInfoResponse.Length
                + CastleSiegeTaxChangeResponse.Length
                + CastleSiegeTaxRateNotification.Length
                + CastleSiegeTributeWithdrawResponse.Length
                + CastleSiegeHuntingZoneGuardInfo.Length
                + CastleSiegeHuntingZoneEntranceSettingResponse.Length
                + CastleSiegeHuntingZoneEnterResponse.Length));

        var taxInfo = (CastleSiegeTaxInfoResponse)data[..CastleSiegeTaxInfoResponse.Length];
        Assert.Multiple(() =>
        {
            Assert.That(taxInfo.Result, Is.EqualTo((byte)CastleSiegeRequestResult.Success));
            Assert.That(taxInfo.TaxRateChaosMachine, Is.EqualTo(3));
            Assert.That(taxInfo.TaxRateNormal, Is.EqualTo(2));
            Assert.That(taxInfo.Treasury, Is.EqualTo(123_456_789));
        });

        var offset = CastleSiegeTaxInfoResponse.Length;
        var taxChange = (CastleSiegeTaxChangeResponse)data.Slice(
            offset,
            CastleSiegeTaxChangeResponse.Length);
        Assert.Multiple(() =>
        {
            Assert.That(taxChange.Result, Is.EqualTo(1));
            Assert.That(
                taxChange.TaxType,
                Is.EqualTo(MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeTaxType.HuntingZoneEntranceFee));
            Assert.That(taxChange.TaxValue, Is.EqualTo(300_000));
        });

        offset += CastleSiegeTaxChangeResponse.Length;
        var taxRateNotification = (CastleSiegeTaxRateNotification)data.Slice(
            offset,
            CastleSiegeTaxRateNotification.Length);
        Assert.Multiple(() =>
        {
            Assert.That(
                taxRateNotification.TaxType,
                Is.EqualTo(MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeTaxType.Store));
            Assert.That(taxRateNotification.TaxRate, Is.EqualTo(3));
        });

        offset += CastleSiegeTaxRateNotification.Length;
        var withdrawal = (CastleSiegeTributeWithdrawResponse)data.Slice(
            offset,
            CastleSiegeTributeWithdrawResponse.Length);
        Assert.Multiple(() =>
        {
            Assert.That(withdrawal.Result, Is.EqualTo(1));
            Assert.That(withdrawal.Money, Is.EqualTo(10_000));
        });

        offset += CastleSiegeTributeWithdrawResponse.Length;
        var guard = (CastleSiegeHuntingZoneGuardInfo)data.Slice(
            offset,
            CastleSiegeHuntingZoneGuardInfo.Length);
        Assert.Multiple(() =>
        {
            Assert.That(guard.Result, Is.EqualTo((byte)CastleSiegeHuntZoneAccessType.OwnerGuildMaster));
            Assert.That(guard.IsEnabled, Is.True);
            Assert.That(guard.CurrentPrice, Is.EqualTo(300_000));
            Assert.That(guard.MaxPrice, Is.EqualTo(300_000));
            Assert.That(guard.UnitPrice, Is.EqualTo(10_000));
        });

        offset += CastleSiegeHuntingZoneGuardInfo.Length;
        var entranceSetting = (CastleSiegeHuntingZoneEntranceSettingResponse)data.Slice(
            offset,
            CastleSiegeHuntingZoneEntranceSettingResponse.Length);
        Assert.Multiple(() =>
        {
            Assert.That(entranceSetting.Result, Is.EqualTo((byte)CastleSiegeRequestResult.Success));
            Assert.That(entranceSetting.IsPublic, Is.True);
        });

        offset += CastleSiegeHuntingZoneEntranceSettingResponse.Length;
        var enterResult = (CastleSiegeHuntingZoneEnterResponse)data.Slice(
            offset,
            CastleSiegeHuntingZoneEnterResponse.Length);
        Assert.That(enterResult.Result, Is.EqualTo(1));
    }
}
