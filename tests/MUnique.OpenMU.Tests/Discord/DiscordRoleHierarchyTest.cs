// <copyright file="DiscordRoleHierarchyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Discord;

using MUnique.OpenMU.Discord;

/// <summary>
/// Tests for the <see cref="DiscordRoleHierarchy"/>.
/// </summary>
[TestFixture]
public class DiscordRoleHierarchyTest
{
    private const ulong BotRoleId = 100;
    private const ulong LinkedRoleId = 200;

    /// <summary>
    /// Tests the state of the role of linked players by the positions of the roles.
    /// </summary>
    /// <param name="botRolePosition">The position of the role of the bot.</param>
    /// <param name="linkedRolePosition">The position of the role of linked players.</param>
    /// <param name="expected">The expected state.</param>
    [TestCase(2, 1, DiscordRoleState.Assignable)]
    [TestCase(1, 2, DiscordRoleState.AboveBot)]
    [TestCase(1, 1, DiscordRoleState.Assignable, TestName = "The older role of the bot is higher, when the positions are the same")]
    public void StateDependsOnTheOrder(int botRolePosition, int linkedRolePosition, DiscordRoleState expected)
    {
        var roles = new List<(ulong Id, string Name, int Position)>
        {
            (1, "@everyone", 0),
            (BotRoleId, "OpenMU", botRolePosition),
            (LinkedRoleId, "Linked Player", linkedRolePosition),
        };

        Assert.That(DiscordRoleHierarchy.GetState(roles, [BotRoleId], "linked player"), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that a role which doesn't exist is reported as missing.
    /// </summary>
    [Test]
    public void MissingRole()
    {
        var roles = new List<(ulong Id, string Name, int Position)> { (BotRoleId, "OpenMU", 1) };

        Assert.That(DiscordRoleHierarchy.GetState(roles, [BotRoleId], "Linked Player"), Is.EqualTo(DiscordRoleState.Missing));
    }
}
