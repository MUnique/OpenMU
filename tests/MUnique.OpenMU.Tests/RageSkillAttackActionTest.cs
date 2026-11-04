// <copyright file="RageSkillAttackActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlayerActions.Skills;

/// <summary>
/// Tests the <see cref="RageSkillAttackAction"/>.
/// </summary>
[TestFixture]
internal class RageSkillAttackActionTest
{
    /// <summary>
    /// Tests that a rage attack during a map change (no current map) is ignored
    /// instead of throwing a <see cref="NullReferenceException"/>.
    /// </summary>
    [Test]
    public async ValueTask AttackWithoutMapIsIgnoredAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await player.SkillList!.AddLearnedSkillAsync(new Skill { Number = 263, Range = 4 }).ConfigureAwait(false);
        player.CurrentMap = null;

        Assert.DoesNotThrowAsync(async () => await new RageSkillAttackAction().AttackAsync(player, 0xFFFF, 263).ConfigureAwait(false));
    }
}
