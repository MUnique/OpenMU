// <copyright file="KanturuRuins.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The initialization for the Kanturu Ruins map.
    /// </summary>
    internal class KanturuRuins : BaseMapInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KanturuRuins"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public KanturuRuins(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc/>
        protected override byte MapNumber => 37;

        /// <inheritdoc/>
        protected override string MapName => "Kanturu_I"; // Kanturu Ruins (1, 2), Kanturu Ruins (3) Island

        /// <inheritdoc/>
        protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
        {
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 188, 011); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 151, 011); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 154, 041); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 150, 056); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 158, 057); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 165, 050); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 092, 034); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 085, 061); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 137, 047); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 061, 031); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 085, 021); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 071, 020); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 046, 034); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 064, 097); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 066, 099); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 048, 103); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 031, 087); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 050, 049); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 055, 104); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 038, 051); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 056, 044); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 065, 035); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 101, 068); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 122, 022); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 129, 019); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 137, 026); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 117, 040); // Berserker
            yield return this.CreateMonsterSpawn(this.NpcDictionary[350], 145, 047); // Berserker

            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 188, 218); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 159, 232); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 036, 234); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 042, 235); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 118, 232); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 055, 228); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 075, 231); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 069, 238); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 174, 196); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 094, 234); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 100, 240); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 114, 239); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 135, 223); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 143, 239); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 151, 206); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 155, 196); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 180, 225); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 172, 215); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 042, 227); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 170, 224); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 157, 204); // Splinter Wolf
            yield return this.CreateMonsterSpawn(this.NpcDictionary[351], 189, 211); // Splinter Wolf

            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 207, 187); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 187, 205); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 172, 205); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 207, 195); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 198, 179); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 180, 164); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 174, 170); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 185, 148); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 165, 157); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 165, 165); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 175, 150); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 214, 143); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 217, 145); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 204, 137); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 195, 156); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 195, 148); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 236, 135); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 223, 132); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 212, 125); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 222, 113); // Iron Rider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[352], 225, 141); // Iron Rider

            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 217, 141); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 226, 154); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 218, 164); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 213, 165); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 195, 152); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 193, 181); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 166, 162); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 182, 150); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 233, 123); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 234, 132); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 236, 091); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 231, 096); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 234, 101); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 220, 155); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 178, 168); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 225, 086); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 224, 082); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 230, 082); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 229, 058); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 226, 068); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 206, 038); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 213, 044); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 214, 035); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 215, 037); // Satyros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[353], 224, 043); // Satyros

            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 204, 035); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 221, 047); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 207, 040); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 189, 014); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 187, 007); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 175, 010); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 148, 019); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 155, 018); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 164, 022); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 165, 040); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 167, 043); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 152, 059); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 156, 059); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 148, 051); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 134, 039); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 138, 039); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 142, 041); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 120, 028); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 126, 018); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 124, 025); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 116, 043); // Blade Hunter
            yield return this.CreateMonsterSpawn(this.NpcDictionary[354], 122, 044); // Blade Hunter

            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 176, 041); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 181, 015); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 153, 015); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 163, 033); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 181, 027); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 137, 042); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 136, 051); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 117, 026); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 119, 055); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 115, 065); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 076, 053); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 072, 062); // Kentauros
            yield return this.CreateMonsterSpawn(this.NpcDictionary[355], 087, 071); // Kentauros

            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 087, 036); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 077, 028); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 066, 021); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 059, 040); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 090, 039); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 041, 037); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 051, 032); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 077, 019); // Gigantis
            yield return this.CreateMonsterSpawn(this.NpcDictionary[356], 068, 028); // Gigantis

            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 037, 059); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 045, 070); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 035, 069); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 063, 101); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 035, 055); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 032, 076); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 058, 104); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 058, 092); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 046, 102); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 047, 054); // Genocider
            yield return this.CreateMonsterSpawn(this.NpcDictionary[357], 044, 044); // Genocider

            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 068, 164); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 077, 157); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 079, 166); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 060, 156); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 058, 135); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 104, 157); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 124, 165); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 093, 118); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 131, 119); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 110, 140); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 098, 137); // Berserker Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[553], 069, 158); // Berserker Warrior

            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 074, 162); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 096, 156); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 102, 150); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 136, 161); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 133, 127); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 089, 124); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 061, 131); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 052, 153); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 094, 128); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 115, 162); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 140, 152); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 086, 119); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 091, 164); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 049, 140); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 132, 111); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 171, 111); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 145, 087); // Kentauros Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[554], 141, 133); // Kentauros Warrior

            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 120, 160); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 141, 159); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 128, 130); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 081, 125); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 107, 103); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 115, 104); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 126, 109); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 134, 107); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 142, 127); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 145, 091); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 167, 090); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 175, 113); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 129, 114); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 112, 098); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 145, 153); // Gigantis Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[555], 110, 106); // Gigantis Warrior

            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 145, 133); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 148, 129); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 167, 114); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 171, 106); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 191, 096); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 187, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 131, 081); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 119, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 181, 109); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 177, 089); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 141, 089); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 151, 087); // Genocider Warrior
            yield return this.CreateMonsterSpawn(this.NpcDictionary[556], 115, 098); // Genocider Warrior
        }

        /// <inheritdoc/>
        protected override void CreateMonsters()
        {
            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 350;
                monster.Designation = "Berserker";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1550 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 100 },
                    { Stats.MaximumHealth, 44370 },
                    { Stats.MinimumPhysBaseDmg, 555 },
                    { Stats.MaximumPhysBaseDmg, 590 },
                    { Stats.DefenseBase, 443 },
                    { Stats.AttackRatePvm, 728 },
                    { Stats.DefenseRatePvm, 255 },
                    { Stats.PoisonResistance, 25f / 255 },
                    { Stats.IceResistance, 25f / 255 },
                    { Stats.LightningResistance, 25f / 255 },
                    { Stats.FireResistance, 25f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 351;
                monster.Designation = "Splinter Wolf";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 80 },
                    { Stats.MaximumHealth, 16000 },
                    { Stats.MinimumPhysBaseDmg, 310 },
                    { Stats.MaximumPhysBaseDmg, 340 },
                    { Stats.DefenseBase, 230 },
                    { Stats.AttackRatePvm, 460 },
                    { Stats.DefenseRatePvm, 163 },
                    { Stats.PoisonResistance, 20f / 255 },
                    { Stats.IceResistance, 20f / 255 },
                    { Stats.LightningResistance, 20f / 255 },
                    { Stats.FireResistance, 20f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 352;
                monster.Designation = "Iron Rider";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 5;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 82 },
                    { Stats.MaximumHealth, 18000 },
                    { Stats.MinimumPhysBaseDmg, 335 },
                    { Stats.MaximumPhysBaseDmg, 365 },
                    { Stats.DefenseBase, 250 },
                    { Stats.AttackRatePvm, 490 },
                    { Stats.DefenseRatePvm, 168 },
                    { Stats.PoisonResistance, 21f / 255 },
                    { Stats.IceResistance, 21f / 255 },
                    { Stats.LightningResistance, 21f / 255 },
                    { Stats.FireResistance, 21f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 353;
                monster.Designation = "Satyros";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 85 },
                    { Stats.MaximumHealth, 22000 },
                    { Stats.MinimumPhysBaseDmg, 365 },
                    { Stats.MaximumPhysBaseDmg, 395 },
                    { Stats.DefenseBase, 280 },
                    { Stats.AttackRatePvm, 540 },
                    { Stats.DefenseRatePvm, 177 },
                    { Stats.PoisonResistance, 22f / 255 },
                    { Stats.IceResistance, 22f / 255 },
                    { Stats.LightningResistance, 22f / 255 },
                    { Stats.FireResistance, 22f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 354;
                monster.Designation = "Blade Hunter";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 88 },
                    { Stats.MaximumHealth, 32000 },
                    { Stats.MinimumPhysBaseDmg, 408 },
                    { Stats.MaximumPhysBaseDmg, 443 },
                    { Stats.DefenseBase, 315 },
                    { Stats.AttackRatePvm, 587 },
                    { Stats.DefenseRatePvm, 195 },
                    { Stats.PoisonResistance, 23f / 255 },
                    { Stats.IceResistance, 23f / 255 },
                    { Stats.LightningResistance, 23f / 255 },
                    { Stats.FireResistance, 23f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 355;
                monster.Designation = "Kentauros";
                monster.MoveRange = 3;
                monster.AttackRange = 1;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 93 },
                    { Stats.MaximumHealth, 38500 },
                    { Stats.MinimumPhysBaseDmg, 470 },
                    { Stats.MaximumPhysBaseDmg, 505 },
                    { Stats.DefenseBase, 370 },
                    { Stats.AttackRatePvm, 645 },
                    { Stats.DefenseRatePvm, 220 },
                    { Stats.PoisonResistance, 24f / 255 },
                    { Stats.IceResistance, 24f / 255 },
                    { Stats.LightningResistance, 24f / 255 },
                    { Stats.FireResistance, 24f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 356;
                monster.Designation = "Gigantis";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 98 },
                    { Stats.MaximumHealth, 43000 },
                    { Stats.MinimumPhysBaseDmg, 546 },
                    { Stats.MaximumPhysBaseDmg, 581 },
                    { Stats.DefenseBase, 430 },
                    { Stats.AttackRatePvm, 715 },
                    { Stats.DefenseRatePvm, 250 },
                    { Stats.PoisonResistance, 25f / 255 },
                    { Stats.IceResistance, 25f / 255 },
                    { Stats.LightningResistance, 25f / 255 },
                    { Stats.FireResistance, 25f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 357;
                monster.Designation = "Genocider";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 7;
                monster.MoveDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 105 },
                    { Stats.MaximumHealth, 48500 },
                    { Stats.MinimumPhysBaseDmg, 640 },
                    { Stats.MaximumPhysBaseDmg, 675 },
                    { Stats.DefenseBase, 515 },
                    { Stats.AttackRatePvm, 810 },
                    { Stats.DefenseRatePvm, 290 },
                    { Stats.PoisonResistance, 26f / 255 },
                    { Stats.IceResistance, 26f / 255 },
                    { Stats.LightningResistance, 26f / 255 },
                    { Stats.FireResistance, 26f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 553;
                monster.Designation = "Berserker Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 123 },
                    { Stats.MaximumHealth, 184370 },
                    { Stats.MinimumPhysBaseDmg, 915 },
                    { Stats.MaximumPhysBaseDmg, 990 },
                    { Stats.DefenseBase, 543 },
                    { Stats.AttackRatePvm, 1557 },
                    { Stats.DefenseRatePvm, 755 },
                    { Stats.PoisonResistance, 20f / 255 },
                    { Stats.IceResistance, 20f / 255 },
                    { Stats.LightningResistance, 150f / 255 },
                    { Stats.FireResistance, 20f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 554;
                monster.Designation = "Kentauros Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 4;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(1500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 126 },
                    { Stats.MaximumHealth, 198500 },
                    { Stats.MinimumPhysBaseDmg, 970 },
                    { Stats.MaximumPhysBaseDmg, 1005 },
                    { Stats.DefenseBase, 570 },
                    { Stats.AttackRatePvm, 1258 },
                    { Stats.DefenseRatePvm, 920 },
                    { Stats.PoisonResistance, 20f / 255 },
                    { Stats.IceResistance, 20f / 255 },
                    { Stats.LightningResistance, 150f / 255 },
                    { Stats.FireResistance, 20f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 555;
                monster.Designation = "Gigantis Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 128 },
                    { Stats.MaximumHealth, 203000 },
                    { Stats.MinimumPhysBaseDmg, 1046 },
                    { Stats.MaximumPhysBaseDmg, 1181 },
                    { Stats.DefenseBase, 630 },
                    { Stats.AttackRatePvm, 1575 },
                    { Stats.DefenseRatePvm, 750 },
                    { Stats.PoisonResistance, 20f / 255 },
                    { Stats.IceResistance, 20f / 255 },
                    { Stats.LightningResistance, 150f / 255 },
                    { Stats.FireResistance, 20f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }

            {
                var monster = this.Context.CreateNew<MonsterDefinition>();
                this.GameConfiguration.Monsters.Add(monster);
                monster.Number = 556;
                monster.Designation = "Genocider Warrior";
                monster.MoveRange = 3;
                monster.AttackRange = 2;
                monster.ViewRange = 9;
                monster.MoveDelay = new TimeSpan(700 * TimeSpan.TicksPerMillisecond);
                monster.AttackDelay = new TimeSpan(2500 * TimeSpan.TicksPerMillisecond);
                monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
                monster.Attribute = 2;
                monster.NumberOfMaximumItemDrops = 1;
                var attributes = new Dictionary<AttributeDefinition, float>
                {
                    { Stats.Level, 129 },
                    { Stats.MaximumHealth, 218500 },
                    { Stats.MinimumPhysBaseDmg, 1240 },
                    { Stats.MaximumPhysBaseDmg, 1375 },
                    { Stats.DefenseBase, 715 },
                    { Stats.AttackRatePvm, 1251 },
                    { Stats.DefenseRatePvm, 990 },
                    { Stats.PoisonResistance, 20f / 255 },
                    { Stats.IceResistance, 20f / 255 },
                    { Stats.LightningResistance, 150f / 255 },
                    { Stats.FireResistance, 20f / 255 },
                };

                monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            }
        }
    }
}
