// <copyright file="GameVersion.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A version of the original game, e.g. a season and its episode.
/// It's used to describe in which version a part of the configuration, like a map or an item, was introduced.
/// </summary>
/// <remarks>
/// The values are persisted and ordered by the release of the version. They are spaced on purpose,
/// so that further versions can be added in between later, without changing the existing values.
/// The versions before Season 1 (also known as Season 0) are named by their client version,
/// and each of them includes the content which was introduced after the previous one.
/// </remarks>
public enum GameVersion
{
    /// <summary>
    /// The version is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The version 0.75 and before.
    /// </summary>
    Version075 = 75,

    /// <summary>
    /// The versions after 0.75 up to 0.95d.
    /// </summary>
    Version095d = 95,

    /// <summary>
    /// The versions after 0.95d up to 0.97d, e.g. the second classes and Blood Castle.
    /// </summary>
    Version097d = 97,

    /// <summary>
    /// The versions 0.98 and 0.99, e.g. the Dark Lord, Chaos Castle and Kalima.
    /// </summary>
    Version099 = 99,

    /// <summary>
    /// The versions 1.00 before Season 1, e.g. the Castle Siege.
    /// </summary>
    Version100 = 100,

    /// <summary>
    /// The Season 1, e.g. Aida, Crywolf and Kalima 7.
    /// </summary>
    Season1 = 1000,

    /// <summary>
    /// The Season 2, e.g. Kanturu, the third classes and Illusion Temple.
    /// </summary>
    Season2 = 2000,

    /// <summary>
    /// The Season 3 Episode 1, e.g. the Summoner and the master level system.
    /// </summary>
    Season3Episode1 = 3100,

    /// <summary>
    /// The Season 3 Episode 2, also known as Season 3+, e.g. the Swamp of Calmness.
    /// </summary>
    Season3Episode2 = 3200,

    /// <summary>
    /// The Season 4 (Episode 1), e.g. the socket items and Raklion.
    /// </summary>
    Season4Episode1 = 4100,

    /// <summary>
    /// The Season 4 Episode 2, also known as Season 4.5, e.g. Vulcanus.
    /// </summary>
    Season4Episode2 = 4200,

    /// <summary>
    /// The Season 5 Episode 1, e.g. the Doppelganger and Imperial Guardian events.
    /// </summary>
    Season5Episode1 = 5100,

    /// <summary>
    /// The Season 5 Episode 2, e.g. the Gens system.
    /// </summary>
    Season5Episode2 = 5200,

    /// <summary>
    /// The Season 5 Episode 3, e.g. Loren Market.
    /// </summary>
    Season5Episode3 = 5300,

    /// <summary>
    /// The Season 5 Episode 4.
    /// </summary>
    Season5Episode4 = 5400,

    /// <summary>
    /// The Season 6 Episode 1, e.g. the Rage Fighter and Karutan.
    /// </summary>
    Season6Episode1 = 6100,

    /// <summary>
    /// The Season 6 Episode 2.
    /// </summary>
    Season6Episode2 = 6200,

    /// <summary>
    /// The Season 6 Episode 3.
    /// </summary>
    Season6Episode3 = 6300,
}
