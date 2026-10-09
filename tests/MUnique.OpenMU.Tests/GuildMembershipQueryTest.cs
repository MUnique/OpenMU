// <copyright file="GuildMembershipQueryTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Tests for the query of the guild membership of a character.
/// </summary>
[TestFixture]
public class GuildMembershipQueryTest
{
    /// <summary>
    /// Tests that the query can be translated to SQL.
    /// </summary>
    [Test]
    public void QueryIsTranslatable()
    {
        if (!ConnectionConfigurator.IsInitialized)
        {
            ConnectionConfigurator.Initialize(new ConfigFileDatabaseConnectionStringProvider());
        }

        using var context = new GuildContext();

        var sql = GuildServerContext.CreateGuildMembershipQuery(context, "Hero").ToQueryString();

        Assert.That(sql, Does.Contain("JOIN"));
        Assert.That(sql, Does.Contain("\"AllianceGuildId\""));
    }
}
