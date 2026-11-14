// <copyright file="HitAttributeLifetimeTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>Accepted-hit continuations retain their publication obligations across ordinary character removal.</summary>
[TestFixture]
public class HitAttributeLifetimeTests
{
    /// <summary>Logout retains the attribute object; reselection replaces it rather than setting it null.</summary>
    /// <param name="reselect">Whether a new selected life enters before the accepted view completes.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task AcceptedHitAcrossLogoutDoesNotLoseAttributeObjectAsync(bool reselect)
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        var character = target.SelectedCharacter!;
        character.Name = "Victim";
        Mock.Get(target.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        target.Attributes![Stats.CurrentHealth] = 100;
        target.Attributes[Stats.CurrentShield] = 0;
        var original = target.Attributes;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(target.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Setup(v => v.ShowHitAsync(target, It.IsAny<HitInfo>())).Returns(async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        });
        var hit = Task.Run(async () => await target.ReflectDamageAsync(attacker, 10).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(original[Stats.CurrentHealth], Is.EqualTo(90));
            Assert.That(target.IsApplyingHitOnCurrentThread, Is.False);
            await target.RunPersistenceExclusiveAsync(() => new LogoutAction().LogoutAsync(target, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
            Assert.That(target.SelectedCharacter, Is.Null);
            Assert.That(target.Attributes, Is.SameAs(original), "Logout does not assign Attributes=null.");
            if (reselect)
            {
                await target.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(target)).ConfigureAwait(false);
                await target.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(target, character.Name)).ConfigureAwait(false);
                Assert.That(target.Attributes, Is.Not.SameAs(original));
                target.Attributes![Stats.CurrentHealth] = 37;
            }

            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.Attributes, Is.Not.Null);
            Assert.That(target.Attributes![Stats.CurrentHealth], Is.EqualTo(reselect ? 37 : 90));
            Mock.Get(attacker.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Verify(v => v.ShowHitAsync(target, It.IsAny<HitInfo>()), Times.Once);
        }
        finally
        {
            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
        }
    }
}
