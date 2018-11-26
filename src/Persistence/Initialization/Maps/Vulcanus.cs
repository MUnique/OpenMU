// <copyright file="Vulcanus.cs" company="MUnique">
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
    /// The initialization for the Exile map.
    /// </summary>
    internal class Vulcanus : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the vulcanus map.
        /// </summary>
        public static readonly byte Number = 63;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Vulcanus";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[479], 1, Direction.SouthWest, SpawnTrigger.Automatic, 122, 122, 135, 135); // Gatekeeper Titus

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[481], 3, Direction.Undefined, SpawnTrigger.Automatic, 064, 124, 054, 089); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[481], 3, Direction.Undefined, SpawnTrigger.Automatic, 065, 105, 012, 047); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[481], 4, Direction.Undefined, SpawnTrigger.Automatic, 022, 064, 010, 098); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[483], 4, Direction.Undefined, SpawnTrigger.Automatic, 021, 077, 128, 186); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[483], 4, Direction.Undefined, SpawnTrigger.Automatic, 028, 105, 189, 232); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[485], 2, Direction.Undefined, SpawnTrigger.Automatic, 123, 174, 186, 219); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[485], 2, Direction.Undefined, SpawnTrigger.Automatic, 172, 230, 215, 234); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[485], 2, Direction.Undefined, SpawnTrigger.Automatic, 175, 238, 171, 194); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[485], 1, Direction.Undefined, SpawnTrigger.Automatic, 203, 243, 195, 214); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[488], 3, Direction.Undefined, SpawnTrigger.Automatic, 198, 226, 106, 148); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[488], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 248, 107, 133); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[488], 1, Direction.Undefined, SpawnTrigger.Automatic, 168, 198, 101, 131); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[491], 2, Direction.Undefined, SpawnTrigger.Automatic, 143, 178, 008, 063); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[491], 2, Direction.Undefined, SpawnTrigger.Automatic, 182, 210, 013, 070); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[491], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 244, 039, 072); // Ruthless Lava Giant

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 107, 107, 060, 060); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 060, 060); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 115, 115, 067, 067); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 078, 078); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 078, 078, 085, 085); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 085, 085, 081, 081); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 084, 084); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 118, 118, 082, 082); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 117, 117, 088, 088); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 088, 088, 059, 059); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 091, 091, 066, 066); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 095, 095, 026, 026); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 030, 030); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 079, 079, 044, 044); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 085, 085, 043, 043); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 020, 020); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 079, 079, 023, 023); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 018, 018); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 014, 014); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 044, 044); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 038, 038); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 062, 062); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 061, 061); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 081, 081); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 039, 088, 088); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 091, 091); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 055, 055, 064, 064); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 065, 065, 068, 068); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 080, 080); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 072, 072); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 042, 042); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 055, 055, 039, 039); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 058, 058); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 020, 020); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 065, 065, 049, 049); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 098, 098); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 055, 055); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 035, 035); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 104, 104, 077, 077); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 089, 089, 087, 087); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 090, 090); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 052, 052, 057, 057); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 076, 076); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 043, 043); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 061, 061, 038, 038); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 085, 085); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 064, 064); // Zombie Fighter
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[480], 1, Direction.Undefined, SpawnTrigger.Automatic, 121, 121, 072, 072); // Zombie Fighter

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 079, 079, 078, 078); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 110, 110, 065, 065); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 024, 024); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 040, 040); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 024, 024); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 042, 042); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 028, 028, 059, 059); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 031, 031, 086, 086); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 068, 068); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 063, 063); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 076, 076, 063, 063); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 060, 060); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 018, 018); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 047, 047, 054, 054); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 040, 040, 073, 073); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 024, 024); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 048, 048, 019, 019); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 022, 022); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 027, 027, 139, 139); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 055, 055, 145, 145); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 139, 139); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 159, 159); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 090, 090, 180, 180); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 175, 175); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 198, 198); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 067, 067, 185, 185); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 050, 050, 159, 159); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 132, 132); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 149, 149); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 152, 152); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 132, 132); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 080, 080, 152, 152); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 175, 175); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 045, 045, 164, 164); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 212, 212); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 039, 230, 230); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 057, 057, 203, 203); // Resurrected Gladiator
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[482], 1, Direction.Undefined, SpawnTrigger.Automatic, 091, 091, 012, 012); // Resurrected Gladiator

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 132, 132); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 141, 141); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 178, 178); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 172, 172); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 034, 034, 155, 155); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 056, 056, 138, 138); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 060, 060, 143, 143); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 165, 165); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 161, 161); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 068, 068, 169, 169); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 144, 144); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 097, 097, 182, 182); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 177, 177); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 179, 179); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 084, 084, 165, 165); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 221, 221); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 227, 227); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 046, 046, 218, 218); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 202, 202); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 059, 059, 210, 210); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 069, 069, 215, 215); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 082, 082, 217, 217); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 080, 080, 200, 200); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 084, 084, 207, 207); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 199, 199); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 094, 094, 216, 216); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 101, 101, 220, 220); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 223, 223); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 037, 037, 204, 204); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 205, 205); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 064, 064, 202, 202); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 073, 073, 177, 177); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 140, 140, 215, 215); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 147, 147, 195, 195); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 195, 195); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 183, 183); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 190, 190, 184, 184); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 227, 227); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 225, 225); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 229, 229, 196, 196); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 210, 210); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 227, 227); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 179, 179); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 106, 106, 228, 228); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 224, 224); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 226, 226); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 180, 180); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 144, 144, 206, 206); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 166, 166, 203, 203); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 100, 100, 213, 213); // Ash Slaughterer
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[484], 1, Direction.Undefined, SpawnTrigger.Automatic, 075, 075, 218, 218); // Ash Slaughterer

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 137, 137, 202, 202); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 154, 154, 204, 204); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 148, 148, 214, 214); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 133, 133, 213, 213); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 156, 156, 188, 188); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 193, 193); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 173, 173); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 178, 178); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 190, 190); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 228, 228, 182, 182); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 232, 232, 191, 191); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 203, 203); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 203, 203); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 174, 174, 228, 228); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 223, 223); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 224, 224); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 219, 219); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 231, 231); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 221, 221); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 208, 208, 208, 208); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 111, 111); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 108, 108); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 125, 125); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 113, 113); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 199, 199, 107, 107); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 132, 132); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 113, 113); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 118, 118); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 241, 241, 126, 126); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 143, 143); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 138, 138, 195, 195); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 200, 200); // Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[486], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 125, 125); // Blood Assassin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 115, 115); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 106, 106); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 124, 124); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 127, 127); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 218, 218, 116, 116); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 240, 240, 119, 119); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 236, 236, 122, 122); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 134, 134); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 123, 123); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 136, 136); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 149, 149); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 129, 129); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 118, 118); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 189, 189); // Cruel Blood Assassin
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[487], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 183, 183); // Cruel Blood Assassin

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 178, 178, 073, 073); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 056, 056); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 160, 160, 050, 050); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 157, 157, 031, 031); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 150, 150, 018, 018); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 015, 015); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 196, 196, 012, 012); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 025, 025); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 028, 028); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 026, 026); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 212, 212, 051, 051); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 217, 217, 044, 044); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 234, 234, 050, 050); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 066, 066); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 233, 233, 061, 061); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 211, 211, 066, 066); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 222, 222, 066, 066); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 063, 063); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 186, 186, 039, 039); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 046, 046); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 027, 027); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 143, 143, 078, 078); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 089, 089); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 070, 070); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 061, 061); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 056, 056); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 195, 195, 047, 047); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 166, 166, 042, 042); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 011, 011); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 154, 154, 064, 064); // Burning Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[489], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 086, 086); // Burning Lava Giant

            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 019, 019); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 023, 023); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 216, 216, 050, 050); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 237, 237, 052, 052); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 235, 235, 058, 058); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 205, 205, 066, 066); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 174, 175, 068, 068); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 161, 161, 057, 057); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 154, 154, 052, 052); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 151, 151, 012, 012); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 162, 162, 032, 032); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 042, 042); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 061, 061); // Ruthless Lava Giant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[490], 1, Direction.Undefined, SpawnTrigger.Automatic, 164, 164, 078, 078); // Ruthless Lava Giant
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 480;
                monster.Designation = "Zombie Fighter";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 90 },
                    { Stats.MaximumHealth, 20000 },
                    { Stats.MinimumPhysBaseDmg, 300 },
                    { Stats.MaximumPhysBaseDmg, 345 },
                    { Stats.DefenseBase, 200 },
                    { Stats.AttackRatePvm, 750 },
                    { Stats.DefenseRatePvm, 170 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 481;
                monster.Designation = "Zombie Fighter";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 120 },
                    { Stats.MaximumHealth, 5000000 },
                    { Stats.MinimumPhysBaseDmg, 850 },
                    { Stats.MaximumPhysBaseDmg, 950 },
                    { Stats.DefenseBase, 210 },
                    { Stats.AttackRatePvm, 1190 },
                    { Stats.DefenseRatePvm, 820 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 200 },
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
                monster.Number = 482;
                monster.Designation = "Resurrected Gladiator";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 93 },
                    { Stats.MaximumHealth, 23000 },
                    { Stats.MinimumPhysBaseDmg, 320 },
                    { Stats.MaximumPhysBaseDmg, 360 },
                    { Stats.DefenseBase, 220 },
                    { Stats.AttackRatePvm, 780 },
                    { Stats.DefenseRatePvm, 200 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 483;
                monster.Designation = "Resurrected Gladiator";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 121 },
                    { Stats.MaximumHealth, 513000 },
                    { Stats.MinimumPhysBaseDmg, 860 },
                    { Stats.MaximumPhysBaseDmg, 960 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 1210 },
                    { Stats.DefenseRatePvm, 825 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 484;
                monster.Designation = "Ash Slaughterer";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 27000 },
                    { Stats.MinimumPhysBaseDmg, 335 },
                    { Stats.MaximumPhysBaseDmg, 370 },
                    { Stats.DefenseBase, 270 },
                    { Stats.AttackRatePvm, 810 },
                    { Stats.DefenseRatePvm, 230 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 485;
                monster.Designation = "Ash Slaughterer";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 122 },
                    { Stats.MaximumHealth, 520000 },
                    { Stats.MinimumPhysBaseDmg, 870 },
                    { Stats.MaximumPhysBaseDmg, 970 },
                    { Stats.DefenseBase, 280 },
                    { Stats.AttackRatePvm, 1230 },
                    { Stats.DefenseRatePvm, 830 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 486;
                monster.Designation = "Blood Assassin";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 107 },
                    { Stats.MaximumHealth, 37000 },
                    { Stats.MinimumPhysBaseDmg, 550 },
                    { Stats.MaximumPhysBaseDmg, 600 },
                    { Stats.DefenseBase, 475 },
                    { Stats.AttackRatePvm, 900 },
                    { Stats.DefenseRatePvm, 300 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 487;
                monster.Designation = "Cruel Blood Assassin";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 109 },
                    { Stats.MaximumHealth, 40000 },
                    { Stats.MinimumPhysBaseDmg, 570 },
                    { Stats.MaximumPhysBaseDmg, 620 },
                    { Stats.DefenseBase, 490 },
                    { Stats.AttackRatePvm, 950 },
                    { Stats.DefenseRatePvm, 325 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 488;
                monster.Designation = "Cruel Blood Assassin";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1200 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 123 },
                    { Stats.MaximumHealth, 523000 },
                    { Stats.MinimumPhysBaseDmg, 880 },
                    { Stats.MaximumPhysBaseDmg, 980 },
                    { Stats.DefenseBase, 500 },
                    { Stats.AttackRatePvm, 1250 },
                    { Stats.DefenseRatePvm, 835 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 489;
                monster.Designation = "Burning Lava Giant";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 111 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 600 },
                    { Stats.MaximumPhysBaseDmg, 650 },
                    { Stats.DefenseBase, 520 },
                    { Stats.AttackRatePvm, 970 },
                    { Stats.DefenseRatePvm, 330 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 490;
                monster.Designation = "Ruthless Lava Giant";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 113 },
                    { Stats.MaximumHealth, 55000 },
                    { Stats.MinimumPhysBaseDmg, 630 },
                    { Stats.MaximumPhysBaseDmg, 680 },
                    { Stats.DefenseBase, 535 },
                    { Stats.AttackRatePvm, 1000 },
                    { Stats.DefenseRatePvm, 340 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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
                monster.Number = 491;
                monster.Designation = "Ruthless Lava Giant";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(300 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 124 },
                    { Stats.MaximumHealth, 530000 },
                    { Stats.MinimumPhysBaseDmg, 890 },
                    { Stats.MaximumPhysBaseDmg, 990 },
                    { Stats.DefenseBase, 545 },
                    { Stats.AttackRatePvm, 1270 },
                    { Stats.DefenseRatePvm, 840 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 200 },
                    { Stats.IceResistance, 200 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 250 },
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