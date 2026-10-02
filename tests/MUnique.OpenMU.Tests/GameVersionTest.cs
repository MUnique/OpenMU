// <copyright file="GameVersionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Text.RegularExpressions;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Tests the values of the <see cref="GameVersion"/>, which are calculated from the version they describe.
/// </summary>
[TestFixture]
public class GameVersionTest
{
    /// <summary>
    /// Tests that the value of each version matches its name, e.g. <c>Version095d</c> is 0.95.4 = 9504,
    /// <c>Version099GPlus</c> is 0.99.33 = 9933 and <c>Season3Episode2</c> is 302000.
    /// </summary>
    [Test]
    public void ValuesMatchNames()
    {
        foreach (var version in Enum.GetValues<GameVersion>().Where(v => v != GameVersion.Unknown))
        {
            Assert.That((int)version, Is.EqualTo(CalculateValue(version.ToString())), version.ToString());
        }
    }

    /// <summary>
    /// Tests that the versions are ordered by their release.
    /// </summary>
    [Test]
    public void VersionsBeforeSeasonOneAreOlderThanTheSeasons()
    {
        var versions = Enum.GetValues<GameVersion>().Where(v => v != GameVersion.Unknown).ToList();
        Assert.That(versions.Where(v => v.ToString().StartsWith("Version", StringComparison.Ordinal)), Is.All.LessThan(GameVersion.Season1));
        Assert.That(GameVersion.Version100s, Is.LessThan(GameVersion.Season1));
        Assert.That(GameVersion.Season6Episode3, Is.EqualTo(versions.Max()));
    }

    private static int CalculateValue(string name)
    {
        if (Regex.Match(name, @"^Season(\d+)(?:Episode(\d+))?$") is { Success: true } season)
        {
            var episode = season.Groups[2].Success ? int.Parse(season.Groups[2].Value) : 0;
            return (int.Parse(season.Groups[1].Value) * 100000) + (episode * 1000);
        }

        var match = Regex.Match(name, @"^Version(\d)(\d\d)([a-z]|[A-Z]Plus)?$");
        Assert.That(match.Success, Is.True, name);
        var patch = match.Groups[3].Value switch
        {
            "" => 0,
            var lowercase when lowercase.Length == 1 => lowercase[0] - 'a' + 1,
            var uppercase => uppercase[0] - 'A' + 27,
        };

        return (int.Parse(match.Groups[1].Value) * 10000) + (int.Parse(match.Groups[2].Value) * 100) + patch;
    }
}
