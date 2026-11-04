// <copyright file="GameMapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Initializes the <see cref="GameMapDefinition"/>s.
/// </summary>
public class GameMapsInitializer : GameMapsInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMapsInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameMapsInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<Type> MapInitializerTypes
    {
        get
        {
            yield return typeof(Lorencia);
            yield return typeof(Dungeon);
            yield return typeof(Devias);
            yield return typeof(Noria);
            yield return typeof(LostTower);
            yield return typeof(Version075.Maps.Exile);
            yield return typeof(Version075.Maps.Arena);
            yield return typeof(Atlans);
            yield return typeof(Version095d.Maps.Tarkan);
            yield return typeof(Icarus);
            yield return typeof(Elvenland);
            yield return typeof(Karutan1);
            yield return typeof(Karutan2);
            yield return typeof(Aida);
            yield return typeof(Vulcanus);
            yield return typeof(CrywolfFortress);
            yield return typeof(LandOfTrials);
            yield return typeof(LorenMarket);
            yield return typeof(SantaVillage);
            yield return typeof(SilentMap);
            yield return typeof(ValleyOfLoren);
            yield return typeof(BarracksOfBalgass);
            yield return typeof(BalgassRefuge);
            yield return typeof(Kalima1);
            yield return typeof(Kalima2);
            yield return typeof(Kalima3);
            yield return typeof(Kalima4);
            yield return typeof(Kalima5);
            yield return typeof(Kalima6);
            yield return typeof(Kalima7);
            yield return typeof(KanturuRelics);
            yield return typeof(KanturuRuins);
            yield return typeof(KanturuEvent);
            yield return typeof(Raklion);
            yield return typeof(RaklionBoss);
            yield return typeof(SwampOfCalmness);
            yield return typeof(DuelArena);
            yield return typeof(BloodCastle1);
            yield return typeof(BloodCastle2);
            yield return typeof(BloodCastle3);
            yield return typeof(BloodCastle4);
            yield return typeof(BloodCastle5);
            yield return typeof(BloodCastle6);
            yield return typeof(BloodCastle7);
            yield return typeof(BloodCastle8);
            yield return typeof(ChaosCastle1);
            yield return typeof(ChaosCastle2);
            yield return typeof(ChaosCastle3);
            yield return typeof(ChaosCastle4);
            yield return typeof(ChaosCastle5);
            yield return typeof(ChaosCastle6);
            yield return typeof(ChaosCastle7);
            yield return typeof(IllusionTemple1);
            yield return typeof(IllusionTemple2);
            yield return typeof(IllusionTemple3);
            yield return typeof(IllusionTemple4);
            yield return typeof(IllusionTemple5);
            yield return typeof(IllusionTemple6);
            yield return typeof(Version095d.Maps.DevilSquare1);
            yield return typeof(Version095d.Maps.DevilSquare2);
            yield return typeof(Version095d.Maps.DevilSquare3);
            yield return typeof(Version095d.Maps.DevilSquare4);
            yield return typeof(DevilSquare5);
            yield return typeof(DevilSquare6);
            yield return typeof(DevilSquare7);
            yield return typeof(Doppelgaenger1);
            yield return typeof(Doppelgaenger2);
            yield return typeof(Doppelgaenger3);
            yield return typeof(Doppelgaenger4);
            yield return typeof(FortressOfImperialGuardian1);
            yield return typeof(FortressOfImperialGuardian2);
            yield return typeof(FortressOfImperialGuardian3);
            yield return typeof(FortressOfImperialGuardian4);
        }
    }
}