// <copyright file="Karutan1.cs" company="MUnique">
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
    /// The initialization for the Karutan1 map.
    /// </summary>
    internal class Karutan1 : BaseMapInitializer
    {
        /// <summary>
        /// The default number of the karutan 1 map.
        /// </summary>
        public static readonly byte Number = 80;

        /// <inheritdoc/>
        protected override byte MapNumber => Number;

        /// <inheritdoc/>
        protected override string MapName => "Karutan1";

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateSpawns(IContext context, GameMapDefinition mapDefinition, GameConfiguration gameConfiguration)
        {
            var npcDictionary = gameConfiguration.Monsters.ToDictionary(npc => npc.Number, npc => npc);

            // NPCs:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[577], 1, Direction.SouthEast, SpawnTrigger.Automatic, 121, 121, 102, 102); // Leina the General Goods Merchant
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[578], 1, Direction.SouthEast, SpawnTrigger.Automatic, 117, 117, 126, 126); // Weapons Merchant Bolo
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.South, SpawnTrigger.Automatic, 123, 123, 132, 132); // Safety Guardian
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.SouthEast, SpawnTrigger.Automatic, 122, 122, 096, 096); // Safety Guardian
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[240], 1, Direction.SouthWest, SpawnTrigger.Automatic, 158, 158, 126, 126); // Safety Guardian

            // Monsters:
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 129, 129, 059, 059); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 066, 066); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 136, 136, 065, 065); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 139, 139, 073, 073); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 148, 148, 081, 081); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 166, 166, 066, 066); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 043, 043); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 194, 194, 042, 042); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 189, 189, 072, 072); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 179, 179, 089, 089); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 192, 192, 109, 109); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 122, 122); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 110, 110); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 213, 213, 097, 097); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 089, 089); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 185, 185, 100, 100); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 175, 175, 116, 116); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 152, 152, 070, 070); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 214, 214, 034, 034); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 103, 103); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 157, 157); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 172, 172, 156, 156); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 209, 209, 170, 170); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 178, 178); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 131, 131, 164, 164); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 112, 112, 184, 184); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 099, 099, 177, 177); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 185, 185); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 079, 079, 175, 175); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 062, 062, 123, 123); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 091, 091, 072, 072); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 089, 089, 067, 067); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 072, 072, 131, 131); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 089, 089, 118, 118); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[569], 1, Direction.Undefined, SpawnTrigger.Automatic, 108, 108, 193, 193); // Venomous Chain Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 132, 132, 067, 067); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 167, 167, 048, 048); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 158, 158, 075, 075); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 174, 174, 057, 057); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 047, 047); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 191, 191, 035, 035); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 038, 038); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 031, 031); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 063, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 188, 188, 063, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 184, 184, 114, 114); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 204, 204, 104, 114); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 201, 201, 094, 094); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 226, 226, 111, 111); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 162, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 176, 176, 181, 181); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 146, 146, 165, 165); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 102, 102, 188, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 098, 098, 188, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 169, 169); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 109, 109, 176, 176); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 139, 139, 156, 156); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 161, 161, 174, 174); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 161, 161, 159, 159); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 210, 210, 188, 188); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 219, 219, 172, 172); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 230, 230, 162, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 223, 223, 162, 162); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 225, 225, 168, 168); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 182, 182, 175, 175); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 193, 193, 192, 192); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 165, 165, 092, 092); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 173, 173, 086, 086); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 120, 120, 180, 180); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 127, 127, 167, 167); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 071, 071, 126, 126); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 066, 066, 117, 117); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 086, 086, 086, 086); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 092, 092, 077, 077); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 095, 095, 069, 069); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 088, 088, 176, 176); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 154, 154, 167, 167); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 163, 163, 054, 054); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 169, 169, 070, 070); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 170, 170, 063, 063); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 198, 198, 100, 100); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[570], 1, Direction.Undefined, SpawnTrigger.Automatic, 215, 215, 103, 103); // Bone Scorpion
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 181, 181, 189, 189); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 187, 187, 181, 181); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 202, 202, 155, 155); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 081, 081, 127, 127); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 093, 093, 112, 112); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 183, 183, 053, 053); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 206, 206, 034, 034); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 180, 180, 102, 102); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 200, 200, 166, 166); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[571], 1, Direction.Undefined, SpawnTrigger.Automatic, 207, 207, 182, 182); // Orcus
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 039, 039, 186, 186); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 033, 033, 162, 162); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 137, 137); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 048, 048, 153, 153); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 063, 063, 167, 167); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 090, 090); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 035, 035, 070, 070); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 056, 056); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 051, 051, 066, 066); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 043, 043, 083, 083); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 068, 068); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 144, 144); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 180, 180); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 167, 167); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 042, 042, 105, 105); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 132, 132); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 148, 148); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[573], 1, Direction.Undefined, SpawnTrigger.Automatic, 058, 058, 178, 178); // Crypta
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 040, 040, 074, 074); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 049, 049, 058, 058); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 085, 085); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 151, 151); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 143, 143); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 055, 055, 160, 160); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 029, 029, 157, 157); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 044, 044, 160, 160); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 038, 038, 173, 173); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 036, 036, 088, 088); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 053, 053, 070, 070); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 041, 041, 096, 096); // Crypos
            yield return this.CreateMonsterSpawn(context, mapDefinition, npcDictionary[574], 1, Direction.Undefined, SpawnTrigger.Automatic, 032, 032, 133, 133); // Crypos
        }

        /// <inheritdoc/>
        protected override void CreateMonsters(IContext context, GameConfiguration gameConfiguration)
        {
            {
                var monster = context.CreateNew<MonsterDefinition>();
                gameConfiguration.Monsters.Add(monster);
                monster.Number = 569;
                monster.Designation = "Venomous Chain Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 99 },
                    { Stats.MaximumHealth, 50000 },
                    { Stats.MinimumPhysBaseDmg, 555 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 445 },
                    { Stats.AttackRatePvm, 845 },
                    { Stats.DefenseRatePvm, 248 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
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
                monster.Number = 570;
                monster.Designation = "Bone Scorpion";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 103 },
                    { Stats.MaximumHealth, 60000 },
                    { Stats.MinimumPhysBaseDmg, 595 },
                    { Stats.MaximumPhysBaseDmg, 635 },
                    { Stats.DefenseBase, 283 },
                    { Stats.AttackRatePvm, 915 },
                    { Stats.DefenseRatePvm, 363 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 100 },
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
                monster.Number = 571;
                monster.Designation = "Orcus";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 65000 },
                    { Stats.MinimumPhysBaseDmg, 618 },
                    { Stats.MaximumPhysBaseDmg, 655 },
                    { Stats.DefenseBase, 518 },
                    { Stats.AttackRatePvm, 965 },
                    { Stats.DefenseRatePvm, 293 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 100 },
                    { Stats.IceResistance, 100 },
                    { Stats.WaterResistance, 0 },
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
                monster.Number = 573;
                monster.Designation = "Crypta";
                monster.MoveRange = 6;
                monster.AttackRange = 1;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 111 },
                    { Stats.MaximumHealth, 78000 },
                    { Stats.MinimumPhysBaseDmg, 705 },
                    { Stats.MaximumPhysBaseDmg, 755 },
                    { Stats.DefenseBase, 560 },
                    { Stats.AttackRatePvm, 1080 },
                    { Stats.DefenseRatePvm, 340 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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
                monster.Number = 574;
                monster.Designation = "Crypos";
                monster.MoveRange = 6;
                monster.AttackRange = 2;
                monster.ViewRange = 10;
                monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 114 },
                    { Stats.MaximumHealth, 83000 },
                    { Stats.MinimumPhysBaseDmg, 720 },
                    { Stats.MaximumPhysBaseDmg, 770 },
                    { Stats.DefenseBase, 575 },
                    { Stats.AttackRatePvm, 11400 },
                    { Stats.DefenseRatePvm, 375 },
                    { Stats.WindResistance, 0 },
                    { Stats.PoisonResistance, 254 },
                    { Stats.IceResistance, 150 },
                    { Stats.WaterResistance, 0 },
                    { Stats.FireResistance, 150 },
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