// <copyright file="GameVersion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A version of the original game, e.g. a version number like 0.97d, or a season and its episode.
/// It's used to describe in which version a part of the configuration, like a map or an item, was introduced.
/// </summary>
/// <remarks>
/// The values are persisted and ordered by the release of the version. Each version includes the content
/// which was introduced after the previous one. The values are calculated, so that further versions can be
/// added later without changing the existing values:
/// <list type="bullet">
///   <item>The versions before Season 1 (also known as Season 0) are numbered by their version number
///   <c>major.minor.patch</c> as <c>major * 10000 + minor * 100 + patch</c>. The patch is the number of the letter
///   of the version, e.g. 0.95d is 0.95.4 and 0.99G+ is 0.99.33.</item>
///   <item>The seasons are numbered as <c>season * 100000 + episode * 1000</c>.</item>
/// </list>
/// The version numbers and their content are the ones of the original Korean game, as listed by https://github.com/Khdoop/mu-online-history.
/// Content of patches without a known version number is attributed to the next version with a known number.
/// </remarks>
public enum GameVersion
{
    /// <summary>
    /// The version is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The version 0.29 (2001), the first one with a known version number, e.g. Lorencia, Dungeon, Devias, the Dark Knight and the Dark Wizard.
    /// The content of the closed alpha and the open beta test is attributed to it.
    /// </summary>
    Version029 = 2900,

    /// <summary>
    /// The version 0.34 (03.08.2001), e.g. the Fairy Elf and Noria.
    /// </summary>
    Version034 = 3400,

    /// <summary>
    /// The version 0.43 (02.11.2001), e.g. the guilds.
    /// </summary>
    Version043 = 4300,

    /// <summary>
    /// The version 0.45 (14.11.2001), e.g. Lost Tower and the Dragon, Legendary and Guardian sets.
    /// </summary>
    Version045 = 4500,

    /// <summary>
    /// The version 0.48 (18.12.2001), e.g. the Red Dragon invasion and the Box of Luck.
    /// </summary>
    Version048 = 4800,

    /// <summary>
    /// The version 0.60 (12.03.2002), e.g. the vault.
    /// </summary>
    Version060 = 6000,

    /// <summary>
    /// The version 0.64 (14.05.2002), e.g. Atlans and the transformation rings.
    /// </summary>
    Version064 = 6400,

    /// <summary>
    /// The version 0.66 (09.07.2002), e.g. the Arena and Battle Soccer.
    /// </summary>
    Version066 = 6600,

    /// <summary>
    /// The version 0.68 (08.08.2002), e.g. the Chaos Machine and the first wings.
    /// </summary>
    Version068 = 6800,

    /// <summary>
    /// The version 0.75, which is the version of the 0.75 data initialization.
    /// </summary>
    Version075 = 7500,

    /// <summary>
    /// The version 0.84 (07.01.2003), e.g. Tarkan, the excellent items and the Jewel of Life.
    /// </summary>
    Version084 = 8400,

    /// <summary>
    /// The version 0.89c (11.02.2003), e.g. Devil Square 1 to 4.
    /// </summary>
    Version089c = 8903,

    /// <summary>
    /// The version 0.94b (06.06.2003), e.g. Icarus and the Jewel of Creation.
    /// </summary>
    Version094b = 9402,

    /// <summary>
    /// The version 0.95d, which is the version of the 0.95d data initialization, e.g. the Magic Gladiator.
    /// </summary>
    Version095d = 9504,

    /// <summary>
    /// The version 0.95k (26.08.2003), e.g. the second classes and wings, and the Golden Dragon invasion.
    /// </summary>
    Version095k = 9511,

    /// <summary>
    /// The version 0.96y (27.10.2003), e.g. Blood Castle 1 to 6.
    /// </summary>
    Version096y = 9625,

    /// <summary>
    /// The version 0.97d.
    /// </summary>
    Version097d = 9704,

    /// <summary>
    /// The version 0.97p (11.12.2003), e.g. Marlon's quest.
    /// </summary>
    Version097p = 9716,

    /// <summary>
    /// The version 0.97r (16.12.2003), e.g. the White Wizard invasion.
    /// </summary>
    Version097r = 9718,

    /// <summary>
    /// The version 0.98r (09.04.2004), e.g. Blood Castle 7.
    /// </summary>
    Version098r = 9818,

    /// <summary>
    /// The version 0.99 (11.05.2004), e.g. Chaos Castle 1 to 6 and the ancient sets.
    /// </summary>
    Version099 = 9900,

    /// <summary>
    /// The version 0.99G+ (23.11.2004), e.g. the Dark Lord and Kalima.
    /// </summary>
    Version099GPlus = 9933,

    /// <summary>
    /// The version 1.00s (11.03.2005), e.g. the Castle Siege.
    /// </summary>
    Version100s = 10019,

    /// <summary>
    /// The Season 1 (1.01b, 17.08.2005), e.g. Aida, Crywolf and Kalima 7.
    /// The content of the patches between the version 1.00s and Season 1, like Devil Square 5 and 6, is attributed to it.
    /// </summary>
    Season1 = 100000,

    /// <summary>
    /// The Season 2, e.g. Kanturu, the third classes and Illusion Temple.
    /// </summary>
    Season2 = 200000,

    /// <summary>
    /// The Season 3 Episode 1, e.g. the Summoner and the master level system.
    /// </summary>
    Season3Episode1 = 301000,

    /// <summary>
    /// The Season 3 Episode 2, also known as Season 3+, e.g. the Swamp of Calmness.
    /// </summary>
    Season3Episode2 = 302000,

    /// <summary>
    /// The Season 4 (Episode 1), e.g. the socket items and Raklion.
    /// </summary>
    Season4Episode1 = 401000,

    /// <summary>
    /// The Season 4 Episode 2, also known as Season 4.5, e.g. Vulcanus.
    /// </summary>
    Season4Episode2 = 402000,

    /// <summary>
    /// The Season 5 Episode 1, e.g. the Doppelganger and Imperial Guardian events.
    /// </summary>
    Season5Episode1 = 501000,

    /// <summary>
    /// The Season 5 Episode 2, e.g. the Gens system.
    /// </summary>
    Season5Episode2 = 502000,

    /// <summary>
    /// The Season 5 Episode 3, e.g. Loren Market.
    /// </summary>
    Season5Episode3 = 503000,

    /// <summary>
    /// The Season 5 Episode 4, e.g. the expansions of Aida, Kanturu, the Swamp of Calmness and Raklion.
    /// </summary>
    Season5Episode4 = 504000,

    /// <summary>
    /// The Season 6 Episode 1, e.g. the Rage Fighter and Karutan.
    /// </summary>
    Season6Episode1 = 601000,

    /// <summary>
    /// The Season 6 Episode 2, e.g. the level 380 items of the Summoner and the Rage Fighter.
    /// </summary>
    Season6Episode2 = 602000,

    /// <summary>
    /// The Season 6 Episode 3.
    /// </summary>
    Season6Episode3 = 603000,
}
