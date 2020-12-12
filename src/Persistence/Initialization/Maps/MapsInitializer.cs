// <copyright file="MapsInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Initializes the <see cref="GameMapDefinition"/>s.
    /// </summary>
    public class MapsInitializer : InitializerBase
    {
        private readonly IList<Maps.IMapInitializer> mapInitializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsInitializer"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public MapsInitializer(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
            var mapInitializerTypes = new[]
            {
                typeof(Lorencia),
                typeof(Dungeon),
                typeof(Devias),
                typeof(Noria),
                typeof(LostTower),
                typeof(Exile),
                typeof(Arena),
                typeof(Atlans),
                typeof(Tarkan),
                typeof(DevilSquare1To4),
                typeof(Icarus),
                typeof(Elvenland),
                typeof(Karutan1),
                typeof(Karutan2),
                typeof(Aida),
                typeof(Vulcanus),
                typeof(CrywolfFortress),
                typeof(LandOfTrials),
                typeof(LorenMarket),
                typeof(SantaVillage),
                typeof(SilentMap),
                typeof(ValleyOfLoren),
                typeof(BarracksOfBalgass),
                typeof(BalgassRefuge),
                typeof(Kalima1),
                typeof(Kalima2),
                typeof(Kalima3),
                typeof(Kalima4),
                typeof(Kalima5),
                typeof(Kalima6),
                typeof(Kalima7),
                typeof(KanturuRelics),
                typeof(KanturuRuins),
                typeof(KanturuEvent),
                typeof(Raklion),
                typeof(RaklionBoss),
                typeof(SwampOfCalmness),
                typeof(DuelArena),
                typeof(BloodCastle1),
                typeof(BloodCastle2),
                typeof(BloodCastle3),
                typeof(BloodCastle4),
                typeof(BloodCastle5),
                typeof(BloodCastle6),
                typeof(BloodCastle7),
                typeof(BloodCastle8),
                typeof(ChaosCastle1),
                typeof(ChaosCastle2),
                typeof(ChaosCastle3),
                typeof(ChaosCastle4),
                typeof(ChaosCastle5),
                typeof(ChaosCastle6),
                typeof(ChaosCastle7),
                typeof(IllusionTemple1),
                typeof(IllusionTemple2),
                typeof(IllusionTemple3),
                typeof(IllusionTemple4),
                typeof(IllusionTemple5),
                typeof(IllusionTemple6),
                typeof(DevilSquare5To7),
                typeof(Doppelgaenger1),
                typeof(Doppelgaenger2),
                typeof(Doppelgaenger3),
                typeof(Doppelgaenger4),
                typeof(FortressOfImperialGuardian1),
                typeof(FortressOfImperialGuardian2),
                typeof(FortressOfImperialGuardian3),
                typeof(FortressOfImperialGuardian4),
            };
            var parameters = new object[] { this.Context, this.GameConfiguration };
            this.mapInitializers = mapInitializerTypes
                .Select(type => type.GetConstructors().First().Invoke(parameters))
                .OfType<IMapInitializer>()
                .ToList();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            foreach (var mapInitializer in this.mapInitializers)
            {
                mapInitializer.Initialize();
            }
        }

        /// <summary>
        /// Sets the safezone maps.
        /// Needs to be called after the context has been saved, otherwise referencing between the maps will fail.
        /// </summary>
        public void SetSafezoneMaps()
        {
            foreach (var mapInitializer in this.mapInitializers)
            {
                mapInitializer.SetSafezoneMap();
            }
        }
    }
}