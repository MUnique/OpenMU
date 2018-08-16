// <copyright file="CrywolfFortress.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The initialization for the Crywolf Fortress map.
    /// </summary>
    internal class CrywolfFortress : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the Crywolf Fortress map.
        /// </summary>
        public static readonly byte Number = 34;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Crywolf Fortress";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[406], 1, 1, SpawnTrigger.Automatic, 228, 228, 048, 048); // Apostle Devin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[407], 1, 1, SpawnTrigger.Automatic, 062, 062, 239, 239); // Werewolf Quarel
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[226], 1, 1, SpawnTrigger.Automatic, 135, 135, 047, 047); // Treiner
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[248], 1, 3, SpawnTrigger.Automatic, 099, 099, 040, 040); // Wandering Merchant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[256], 1, 3, SpawnTrigger.Automatic, 096, 096, 025, 025); // Lahap
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[251], 1, 3, SpawnTrigger.Automatic, 145, 145, 014, 014); // Hanzo the Blacksmith
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, 1, SpawnTrigger.Automatic, 113, 113, 056, 056); // Baz The Vault Keeper
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[224], 1, 3, SpawnTrigger.Automatic, 118, 118, 011, 011); // Guardsman

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[204], 1, 2, SpawnTrigger.Automatic, 121, 121, 031, 031); // Wolf Status
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[205], 1, 2, SpawnTrigger.Automatic, 125, 125, 027, 027); // Wolf Altar1
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[206], 1, 2, SpawnTrigger.Automatic, 126, 126, 035, 035); // Wolf Altar2
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[207], 1, 2, SpawnTrigger.Automatic, 120, 120, 038, 038); // Wolf Altar3
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[208], 1, 2, SpawnTrigger.Automatic, 115, 115, 035, 035); // Wolf Altar4
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[209], 1, 2, SpawnTrigger.Automatic, 117, 117, 027, 027); // Wolf Altar5

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 058, 058, 047, 047); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 060, 060, 038, 038); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 065, 065, 024, 024); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 070, 070, 037, 037); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 074, 074, 020, 020); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 077, 077, 032, 032); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 078, 078, 051, 051); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 079, 079, 043, 043); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 083, 083, 010, 010); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 088, 088, 050, 050); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 088, 088, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 094, 094, 054, 054); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 098, 098, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 150, 150, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 159, 159, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 162, 162, 041, 041); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 164, 164, 018, 018); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 169, 169, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 176, 176, 032, 032); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 178, 178, 022, 022); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 179, 179, 048, 048); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 185, 185, 035, 035); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 191, 191, 027, 027); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 194, 194, 044, 044); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 195, 195, 033, 033); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 199, 199, 060, 060); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 201, 201, 040, 040); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 205, 205, 046, 046); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 206, 206, 045, 045); // Hammer Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[310], 1, 0, SpawnTrigger.Automatic, 214, 214, 060, 060); // Hammer Scout

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 022, 022, 100, 100); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 027, 027, 122, 122); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 035, 035, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 047, 047, 053, 053); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 047, 047, 065, 065); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 079, 079, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 082, 082, 067, 067); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 095, 095, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 103, 103, 071, 071); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 113, 113, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 117, 117, 099, 099); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 122, 122, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 135, 135, 069, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 138, 138, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 146, 146, 068, 068); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 156, 156, 069, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 157, 157, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 159, 159, 049, 049); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 170, 170, 047, 047); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 170, 170, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 173, 173, 097, 097); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 174, 174, 054, 054); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 177, 177, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 197, 197, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 210, 210, 088, 088); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 211, 211, 069, 069); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 221, 221, 078, 078); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 223, 223, 120, 120); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 225, 225, 068, 068); // Lance Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[311], 1, 0, SpawnTrigger.Automatic, 231, 231, 078, 078); // Lance Scout

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 021, 021, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 023, 023, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 030, 030, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 056, 056, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 068, 068, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 078, 078, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 079, 079, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 083, 083, 101, 101); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 090, 090, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 100, 100, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 114, 114, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 122, 122, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 124, 124, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 135, 135, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 145, 145, 099, 099); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 146, 146, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 155, 155, 115, 115); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 156, 156, 084, 084); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 168, 168, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 178, 178, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 192, 192, 088, 088); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 201, 201, 089, 089); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 202, 202, 098, 098); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 217, 217, 094, 094); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 233, 233, 102, 102); // Bow Scout
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[312], 1, 0, SpawnTrigger.Automatic, 236, 236, 088, 088); // Bow Scout

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 019, 019, 131, 131); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 020, 020, 145, 145); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 033, 033, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 046, 046, 123, 123); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 071, 071, 109, 109); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 076, 076, 145, 145); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 082, 082, 107, 107); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 082, 082, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 088, 188, 144, 144); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 119, 119, 108, 108); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 123, 123, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 123, 123, 145, 145); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 149, 149, 110, 110); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 155, 155, 098, 098); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 163, 163, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 164, 164, 144, 144); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 197, 197, 145, 145); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 204, 204, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 206, 206, 110, 110); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 218, 218, 110, 110); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 226, 226, 130, 130); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 235, 235, 110, 110); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 237, 237, 144, 144); // Werewolf
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[313], 1, 0, SpawnTrigger.Automatic, 240, 240, 131, 131); // Werewolf

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 016, 016, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 028, 028, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 065, 065, 148, 148); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 092, 092, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 119, 119, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 159, 159, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 171, 171, 148, 148); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 177, 177, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 236, 236, 156, 156); // Scout(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[314], 1, 0, SpawnTrigger.Automatic, 236, 236, 156, 156); // Scout(Hero)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 030, 030, 172, 172); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 049, 049, 194, 194); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 051, 051, 213, 213); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 052, 052, 171, 171); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 059, 059, 211, 211); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 068, 068, 224, 224); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 073, 073, 210, 210); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 085, 085, 230, 230); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 092, 092, 219, 219); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 100, 100, 182, 182); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 102, 102, 171, 171); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 115, 115, 204, 204); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 117, 117, 193, 193); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 133, 133, 195, 195); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 133, 133, 214, 214); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 145, 145, 208, 208); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 148, 148, 195, 195); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 159, 159, 206, 206); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 170, 170, 208, 208); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 175, 175, 197, 197); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 199, 199, 200, 200); // Werewolf(Hero)
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[315], 1, 0, SpawnTrigger.Automatic, 222, 222, 189, 189); // Werewolf(Hero)

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 047, 047, 184, 184); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 084, 084, 201, 201); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 104, 104, 194, 194); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 117, 117, 184, 184); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 146, 146, 184, 184); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 161, 161, 194, 194); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 173, 173, 184, 184); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 212, 212, 193, 193); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 228, 228, 180, 180); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 054, 054, 217, 217); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 066, 066, 217, 217); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 083, 083, 217, 217); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 100, 100, 217, 217); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 134, 134, 215, 215); // Balram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[316], 1, 0, SpawnTrigger.Automatic, 153, 153, 215, 215); // Balram

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 014, 014, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 025, 025, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 041, 041, 171, 171); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 056, 056, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 065, 065, 234, 234); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 096, 096, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 114, 114, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 116, 116, 172, 172); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 140, 140, 230, 230); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 145, 145, 174, 174); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 147, 147, 164, 164); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 168, 168, 229, 229); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 177, 177, 167, 167); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 233, 233, 169, 169); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 115, 115, 221, 221); // Soram
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[317], 1, 0, SpawnTrigger.Automatic, 103, 103, 206, 206); // Soram
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 204;
                monster.Designation = "Wolf Status";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 205;
                monster.Designation = "Wolf Altar1";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 206;
                monster.Designation = "Wolf Altar2";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 207;
                monster.Designation = "Wolf Altar3";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 208;
                monster.Designation = "Wolf Altar4";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 209;
                monster.Designation = "Wolf Altar5";
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 2 },
                    { Stats.MaximumHealth, 50 },
                    { Stats.MinimumPhysBaseDmg, 15 },
                    { Stats.MaximumPhysBaseDmg, 30 },
                    { Stats.DefenseBase, 70 },
                    { Stats.AttackRatePvm, 10 },
                    { Stats.DefenseRatePvm, 30 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 0 },
                    { Stats.IceResistance, 0 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 0 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 310;
                monster.Designation = "Hammer Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 15 },
                    { Stats.IceResistance, 15 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 15 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 311;
                monster.Designation = "Lance Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 3;
                monster.ViewRange = 6;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 17 },
                    { Stats.IceResistance, 17 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 17 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 312;
                monster.Designation = "Bow Scout";
                monster.MoveRange = 6;
                monster.AttackRange = 6;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 97 },
                    { Stats.MaximumHealth, 45000 },
                    { Stats.MinimumPhysBaseDmg, 530 },
                    { Stats.MaximumPhysBaseDmg, 560 },
                    { Stats.DefenseBase, 420 },
                    { Stats.AttackRatePvm, 670 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 19 },
                    { Stats.IceResistance, 19 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 19 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 313;
                monster.Designation = "Werewolf";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 118 },
                    { Stats.MaximumHealth, 110000 },
                    { Stats.MinimumPhysBaseDmg, 830 },
                    { Stats.MaximumPhysBaseDmg, 850 },
                    { Stats.DefenseBase, 680 },
                    { Stats.AttackRatePvm, 950 },
                    { Stats.DefenseRatePvm, 355 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 21 },
                    { Stats.IceResistance, 21 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 21 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 314;
                monster.Designation = "Scout(Hero)";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 123 },
                    { Stats.MaximumHealth, 120000 },
                    { Stats.MinimumPhysBaseDmg, 890 },
                    { Stats.MaximumPhysBaseDmg, 910 },
                    { Stats.DefenseBase, 740 },
                    { Stats.AttackRatePvm, 980 },
                    { Stats.DefenseRatePvm, 370 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 23 },
                    { Stats.IceResistance, 23 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 23 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 315;
                monster.Designation = "Werewolf(Hero)";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 127 },
                    { Stats.MaximumHealth, 123000 },
                    { Stats.MinimumPhysBaseDmg, 964 },
                    { Stats.MaximumPhysBaseDmg, 1015 },
                    { Stats.DefenseBase, 800 },
                    { Stats.AttackRatePvm, 1027 },
                    { Stats.DefenseRatePvm, 397 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 25 },
                    { Stats.IceResistance, 25 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 25 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 316;
                monster.Designation = "Balram";
                monster.MoveRange = 6;
                monster.AttackRange = 3;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 132 },
                    { Stats.MaximumHealth, 140000 },
                    { Stats.MinimumPhysBaseDmg, 1075 },
                    { Stats.MaximumPhysBaseDmg, 1140 },
                    { Stats.DefenseBase, 885 },
                    { Stats.AttackRatePvm, 1100 },
                    { Stats.DefenseRatePvm, 440 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 27 },
                    { Stats.IceResistance, 27 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 27 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }

            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 317;
                monster.Designation = "Soram";
                monster.MoveRange = 6;
                monster.AttackRange = 7;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 134 },
                    { Stats.MaximumHealth, 164000 },
                    { Stats.MinimumPhysBaseDmg, 1200 },
                    { Stats.MaximumPhysBaseDmg, 1300 },
                    { Stats.DefenseBase, 982 },
                    { Stats.AttackRatePvm, 1173 },
                    { Stats.DefenseRatePvm, 500 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 29 },
                    { Stats.IceResistance, 29 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 29 },
                }.Select(kvp =>
                {
                    var attribute = context.CreateNew<MonsterAttribute>();
                    attribute.AttributeDefinition = gameConfiguration.Attributes.First(a => a == kvp.Key);
                    attribute.Value = kvp.Value;
                    return attribute;
                }).ToList().ForEach(monster.Attributes.Add);
            }
        }
    }
}
