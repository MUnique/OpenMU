// <copyright file="Karutan2.cs" company="MUnique">
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
    /// The initialization for the Karutan2 map.
    /// </summary>
    internal class Karutan2 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the karutan 2 map.
        /// </summary>
        public const byte Number = 81;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Karutan2";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 058, 058, 160, 160); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 053, 053, 165, 165); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 067, 067, 180, 180); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 070, 070, 169, 169); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 061, 061, 172, 172); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 134, 134, 141, 141); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 143, 143, 149, 149); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 143, 143, 139, 139); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 131, 131, 146, 146); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 136, 136, 156, 156); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 197, 197, 157, 157); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 205, 205, 158, 158); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 211, 211, 157, 157); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 211, 211, 167, 167); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 204, 204, 170, 170); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 198, 198, 162, 162); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 064, 064, 134, 134); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 126, 126, 124, 124); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 132, 132, 117, 117); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 214, 214, 133, 133); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 208, 208, 126, 126); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, 0, SpawnTrigger.Automatic, 067, 067, 175, 175); // Orcus

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 065, 065, 163, 163); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 067, 067, 171, 171); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 058, 058, 173, 173); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 138, 138, 138, 138); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 140, 140, 154, 154); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 148, 148, 144, 144); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 201, 201, 158, 158); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 208, 208, 171, 171); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 214, 214, 162, 162); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 202, 202, 166, 166); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 206, 206, 134, 134); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 216, 216, 137, 137); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 140, 140, 117, 117); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 061, 061, 145, 145); // Gollock
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[572], 1, 0, SpawnTrigger.Automatic, 066, 066, 142, 142); // Gollock

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 078, 078, 042, 042); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 071, 071, 043, 043); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 076, 076, 060, 060); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 058, 058, 059, 059); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 060, 060, 046, 046); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 082, 082, 048, 048); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 094, 094, 056, 056); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 114, 114, 061, 061); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 115, 115, 049, 049); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 127, 127, 060, 060); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 127, 127, 053, 053); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 130, 130, 049, 049); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 152, 152, 047, 047); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 160, 160, 035, 035); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 186, 186, 041, 041); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 201, 201, 037, 037); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 207, 207, 051, 051); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 207, 207, 076, 076); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 186, 186, 060, 060); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 171, 171, 068, 068); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 137, 137, 081, 081); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 140, 140, 063, 063); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 097, 097, 078, 078); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 126, 126, 089, 089); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 106, 106, 082, 082); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 098, 098, 092, 092); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 081, 081, 095, 095); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 069, 069, 088, 088); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 100, 100, 104, 104); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 119, 119, 081, 081); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 156, 156, 080, 080); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 200, 200, 083, 083); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 203, 203, 062, 062); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 206, 206, 040, 040); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 169, 169, 039, 039); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 169, 169, 030, 030); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 101, 101, 057, 057); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 076, 076, 053, 053); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 078, 078, 103, 103); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 097, 097, 085, 085); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 138, 138, 113, 113); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 145, 145, 108, 108); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 178, 178, 064, 064); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 201, 201, 058, 058); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 201, 201, 087, 087); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 221, 221, 048, 048); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 211, 211, 065, 065); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 074, 074, 105, 105); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 071, 071, 131, 131); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 059, 059, 121, 121); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 054, 054, 136, 136); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 058, 058, 140, 140); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 062, 062, 138, 138); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 130, 130, 121, 121); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 132, 132, 103, 103); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 152, 152, 105, 105); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 200, 200, 108, 108); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 213, 213, 126, 126); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 211, 211, 095, 095); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 182, 182, 072, 072); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 185, 185, 047, 047); // Condra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[575], 1, 0, SpawnTrigger.Automatic, 164, 164, 030, 030); // Condra

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 087, 087, 042, 042); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 081, 081, 054, 054); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 107, 107, 062, 062); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 110, 110, 055, 055); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 122, 122, 058, 058); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 092, 092, 080, 080); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 141, 141, 056, 056); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 170, 170, 035, 035); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 190, 192, 038, 038); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 195, 195, 061, 061); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 210, 210, 043, 043); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 202, 202, 080, 080); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 140, 140, 074, 074); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 133, 133, 083, 083); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 125, 125, 085, 085); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 104, 104, 089, 089); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 078, 078, 098, 098); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 065, 065, 090, 090); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 066, 066, 126, 126); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 078, 078, 109, 109); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 132, 132, 109, 109); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 187, 187, 081, 081); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 197, 197, 099, 099); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 206, 206, 110, 110); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 217, 217, 122, 122); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 210, 210, 130, 130); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 148, 148, 120, 120); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 153, 153, 113, 113); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 069, 069, 140, 140); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 053, 053, 129, 129); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 095, 095, 095, 095); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 084, 084, 100, 100); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 150, 150, 084, 084); // Narcondra
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[576], 1, 0, SpawnTrigger.Automatic, 181, 181, 057, 057); // Narcondra
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 572;
                monster.Designation = "Gollock";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 108 },
                    { Stats.MaximumHealth, 72000 },
                    { Stats.MinimumPhysBaseDmg, 685 },
                    { Stats.MaximumPhysBaseDmg, 735 },
                    { Stats.DefenseBase, 545 },
                    { Stats.AttackRatePvm, 1020 },
                    { Stats.DefenseRatePvm, 315 },
                    { Stats.PoisonResistance, 100 },
                    { Stats.IceResistance, 100 },
                    { Stats.LightningResistance, 100 },
                    { Stats.FireResistance, 240 },
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
                monster.Number = 575;
                monster.Designation = "Condra";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 117 },
                    { Stats.MaximumHealth, 90000 },
                    { Stats.MinimumPhysBaseDmg, 735 },
                    { Stats.MaximumPhysBaseDmg, 790 },
                    { Stats.DefenseBase, 610 },
                    { Stats.AttackRatePvm, 1200 },
                    { Stats.DefenseRatePvm, 406 },
                    { Stats.PoisonResistance, 255 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 255 },
                    { Stats.FireResistance, 255 },
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
                monster.Number = 576;
                monster.Designation = "Narcondra";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 120 },
                    { Stats.MaximumHealth, 96000 },
                    { Stats.MinimumPhysBaseDmg, 750 },
                    { Stats.MaximumPhysBaseDmg, 815 },
                    { Stats.DefenseBase, 640 },
                    { Stats.AttackRatePvm, 1265 },
                    { Stats.DefenseRatePvm, 425 },
                    { Stats.PoisonResistance, 255 },
                    { Stats.IceResistance, 150 },
                    { Stats.LightningResistance, 255 },
                    { Stats.FireResistance, 255 },
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