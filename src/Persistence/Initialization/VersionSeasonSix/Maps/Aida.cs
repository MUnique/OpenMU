﻿// <copyright file="Aida.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization for the Aida map.
/// </summary>
internal class Aida : BaseMapInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Aida"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Aida(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override byte MapNumber => 33;

    /// <inheritdoc/>
    protected override string MapName => "Aida";

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateNpcSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[369], 078, 013, Direction.SouthEast); // Osbourne
        yield return this.CreateMonsterSpawn(this.NpcDictionary[370], 086, 014, Direction.SouthWest); // Jerridon
    }

    /// <inheritdoc/>
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 88, 73); // Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 106, 049); // Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 120, 097); // Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 125, 082); // Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 137, 125); // Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[304], 141, 076); // Witch Queen

        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 081, 080); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 083, 121); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 088, 062); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 092, 045); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 092, 086); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 094, 158); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 095, 150); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 102, 175); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 104, 042); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 104, 160); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 111, 094); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 113, 155); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 114, 067); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 125, 112); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 126, 073); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 131, 094); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 133, 079); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 133, 133); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 134, 106); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 136, 066); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 146, 072); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 148, 093); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 157, 166); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 167, 127); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 168, 140); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 168, 168); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 169, 174); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 180, 040); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 185, 147); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 194, 019); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 197, 148); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 208, 168); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 235, 101); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 195, 153); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 117, 073); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 114, 101); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 086, 155); // Blue Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[305], 230, 132); // Blue Golem

        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 215, 032); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 217, 036); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 085, 145); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 088, 126); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 089, 109); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 093, 173); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 100, 058); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 119, 056); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 126, 156); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 136, 144); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 145, 087); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 149, 112); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 161, 117); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 167, 152); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 176, 124); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 186, 089); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 189, 179); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 190, 144); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 191, 138); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 207, 054); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 209, 075); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 221, 060); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 223, 158); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 227, 052); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 230, 012); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 233, 061); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 234, 071); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 234, 146); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 235, 090); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 214, 035); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 228, 017); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 236, 033); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 237, 038); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 169, 085); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 193, 150); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 204, 172); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 090, 203); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 188, 027); // Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[306], 228, 060); // Death Rider

        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 168, 082); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 166, 073); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 234, 036); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 086, 171); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 126, 020); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 126, 026); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 148, 010); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 148, 019); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 149, 035); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 157, 016); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 158, 046); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 166, 057); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 172, 011); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 176, 060); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 183, 096); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 187, 017); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 189, 055); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 190, 040); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 190, 079); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 194, 033); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 196, 045); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 199, 108); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 210, 122); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 212, 062); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 213, 082); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 213, 137); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 214, 010); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 225, 149); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 226, 067); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 226, 140); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 233, 125); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 234, 111); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 234, 155); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 236, 135); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 232, 038); // Forest Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[307], 171, 082); // Forest Orc

        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 213, 038); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 216, 038); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 103, 016); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 113, 008); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 118, 016); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 136, 023); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 137, 013); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 150, 046); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 157, 023); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 158, 010); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 162, 070); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 164, 018); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 166, 048); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 174, 023); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 177, 016); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 189, 038); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 189, 095); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 195, 026); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 196, 099); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 200, 071); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 204, 012); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 205, 031); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 206, 091); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 214, 114); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 219, 091); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 222, 075); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 223, 130); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 228, 111); // Death Tree
        yield return this.CreateMonsterSpawn(this.NpcDictionary[308], 226, 011); // Death Tree

        yield return this.CreateMonsterSpawn(this.NpcDictionary[309], 108, 147); // Hell Maine
        yield return this.CreateMonsterSpawn(this.NpcDictionary[309], 112, 047); // Hell Maine
        yield return this.CreateMonsterSpawn(this.NpcDictionary[309], 123, 103); // Hell Maine

        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 211, 204); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 199, 200); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 208, 210); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 192, 207); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 221, 236); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 215, 238); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 228, 219); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 218, 224); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 232, 217); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 240, 230); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 237, 236); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 186, 207); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 171, 212); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 158, 216); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 162, 194); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 148, 198); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 158, 235); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 163, 237); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 067, 227); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 073, 222); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 082, 228); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 040, 224); // Bloody Orc
        yield return this.CreateMonsterSpawn(this.NpcDictionary[549], 038, 206); // Bloody Orc

        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 203, 213); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 154, 193); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 152, 211); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 170, 203); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 165, 215); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 155, 231); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 159, 228); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 138, 200); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 126, 184); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 131, 227); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 127, 224); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 137, 218); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 122, 200); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 082, 216); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 033, 222); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 054, 155); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 042, 179); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 026, 134); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 026, 118); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 047, 125); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 046, 212); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 020, 181); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 206, 197); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 026, 054); // Bloody Death Rider
        yield return this.CreateMonsterSpawn(this.NpcDictionary[550], 053, 053); // Bloody Death Rider

        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 128, 190); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 143, 198); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 135, 196); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 127, 230); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 130, 218); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 123, 195); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 138, 188); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 136, 184); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 087, 209); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 077, 212); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 066, 209); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 022, 190); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 028, 191); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 040, 184); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 044, 176); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 039, 178); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 055, 120); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 022, 127); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 035, 116); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 025, 061); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 025, 049); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 051, 048); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 051, 060); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 199, 207); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 218, 232); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 236, 232); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 160, 232); // Bloody Golem
        yield return this.CreateMonsterSpawn(this.NpcDictionary[551], 039, 222); // Bloody Golem

        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 045, 227); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 044, 219); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 032, 205); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 026, 182); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 050, 158); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 056, 160); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 024, 186); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 043, 134); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 054, 117); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 039, 119); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 020, 133); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 026, 146); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 039, 148); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 051, 123); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 035, 107); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 036, 093); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 037, 077); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 046, 054); // Bloody Witch Queen
        yield return this.CreateMonsterSpawn(this.NpcDictionary[552], 031, 053); // Bloody Witch Queen
    }

    /// <inheritdoc/>
    protected override void CreateMonsters()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 304;
            monster.Designation = "Witch Queen";
            monster.MoveRange = 4;
            monster.AttackRange = 3;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1700 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 94 },
                { Stats.MaximumHealth, 38500 },
                { Stats.MinimumPhysBaseDmg, 408 },
                { Stats.MaximumPhysBaseDmg, 442 },
                { Stats.DefenseBase, 357 },
                { Stats.AttackRatePvm, 637 },
                { Stats.DefenseRatePvm, 230 },
                { Stats.PoisonResistance, 22f / 255 },
                { Stats.IceResistance, 22f / 255 },
                { Stats.FireResistance, 22f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 305;
            monster.Designation = "Blue Golem";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 84 },
                { Stats.MaximumHealth, 25000 },
                { Stats.MinimumPhysBaseDmg, 288 },
                { Stats.MaximumPhysBaseDmg, 322 },
                { Stats.DefenseBase, 277 },
                { Stats.AttackRatePvm, 507 },
                { Stats.DefenseRatePvm, 170 },
                { Stats.PoisonResistance, 21f / 255 },
                { Stats.IceResistance, 21f / 255 },
                { Stats.FireResistance, 21f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 306;
            monster.Designation = "Death Rider";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 78 },
                { Stats.MaximumHealth, 14000 },
                { Stats.MinimumPhysBaseDmg, 260 },
                { Stats.MaximumPhysBaseDmg, 294 },
                { Stats.DefenseBase, 222 },
                { Stats.AttackRatePvm, 402 },
                { Stats.DefenseRatePvm, 144 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.FireResistance, 20f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 307;
            monster.Designation = "Forest Orc";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 74 },
                { Stats.MaximumHealth, 11000 },
                { Stats.MinimumPhysBaseDmg, 258 },
                { Stats.MaximumPhysBaseDmg, 282 },
                { Stats.DefenseBase, 197 },
                { Stats.AttackRatePvm, 390 },
                { Stats.DefenseRatePvm, 127 },
                { Stats.PoisonResistance, 19f / 255 },
                { Stats.IceResistance, 19f / 255 },
                { Stats.FireResistance, 19f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 308;
            monster.Designation = "Death Tree";
            monster.MoveRange = 4;
            monster.AttackRange = 1;
            monster.ViewRange = 5;
            monster.MoveDelay = new TimeSpan(450 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 72 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 248 },
                { Stats.MaximumPhysBaseDmg, 272 },
                { Stats.DefenseBase, 187 },
                { Stats.AttackRatePvm, 362 },
                { Stats.DefenseRatePvm, 120 },
                { Stats.PoisonResistance, 18f / 255 },
                { Stats.IceResistance, 18f / 255 },
                { Stats.FireResistance, 18f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 309;
            monster.Designation = "Hell Maine";
            monster.MoveRange = 6;
            monster.AttackRange = 5;
            monster.ViewRange = 10;
            monster.MoveDelay = new TimeSpan(600 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(150 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 98 },
                { Stats.MaximumHealth, 50000 },
                { Stats.MinimumPhysBaseDmg, 550 },
                { Stats.MaximumPhysBaseDmg, 600 },
                { Stats.DefenseBase, 520 },
                { Stats.AttackRatePvm, 850 },
                { Stats.DefenseRatePvm, 300 },
                { Stats.PoisonResistance, 50f / 255 },
                { Stats.IceResistance, 50f / 255 },
                { Stats.FireResistance, 50f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 549;
            monster.Designation = "Bloody Orc";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(8 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 114 },
                { Stats.MaximumHealth, 117000 },
                { Stats.MinimumPhysBaseDmg, 675 },
                { Stats.MaximumPhysBaseDmg, 710 },
                { Stats.DefenseBase, 420 },
                { Stats.AttackRatePvm, 900 },
                { Stats.DefenseRatePvm, 820 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 550;
            monster.Designation = "Bloody Death Rider";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 115 },
                { Stats.MaximumHealth, 88000 },
                { Stats.MinimumPhysBaseDmg, 795 },
                { Stats.MaximumPhysBaseDmg, 830 },
                { Stats.DefenseBase, 460 },
                { Stats.AttackRatePvm, 1100 },
                { Stats.DefenseRatePvm, 430 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 551;
            monster.Designation = "Bloody Golem";
            monster.MoveRange = 4;
            monster.AttackRange = 2;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 117 },
                { Stats.MaximumHealth, 129700 },
                { Stats.MinimumPhysBaseDmg, 715 },
                { Stats.MaximumPhysBaseDmg, 750 },
                { Stats.DefenseBase, 470 },
                { Stats.AttackRatePvm, 900 },
                { Stats.DefenseRatePvm, 960 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 552;
            monster.Designation = "Bloody Witch Queen";
            monster.MoveRange = 4;
            monster.AttackRange = 3;
            monster.ViewRange = 9;
            monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 120 },
                { Stats.MaximumHealth, 85700 },
                { Stats.MinimumPhysBaseDmg, 835 },
                { Stats.MaximumPhysBaseDmg, 870 },
                { Stats.DefenseBase, 480 },
                { Stats.AttackRatePvm, 1100 },
                { Stats.DefenseRatePvm, 430 },
                { Stats.PoisonResistance, 20f / 255 },
                { Stats.IceResistance, 20f / 255 },
                { Stats.FireResistance, 150f / 255 },
            };

            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }
    }
}