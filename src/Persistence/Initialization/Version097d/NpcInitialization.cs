// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal class NpcInitialization : Version095d.NpcInitialization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpcInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public NpcInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 232;
            def.Designation = "Archangel";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 233;
            def.Designation = "Messenger of Arch.";
            def.NpcWindow = NpcWindow.BloodCastle;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 131;
            def.Designation = "Castle Gate";
            def.ObjectKind = NpcObjectKind.Destructible;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 132;
            def.Designation = "Statue of Saint";
            def.ObjectKind = NpcObjectKind.Destructible;
            def.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.MaximumHealth, 5000000 },
            };
            def.AddAttributes(attributes, this.Context, this.GameConfiguration);
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);

            var questItemDrop = this.Context.CreateNew<DropItemGroup>();
            questItemDrop.SetGuid(132);
            questItemDrop.Chance = 1;
            questItemDrop.Description = "Archangel Weapon (Blood Castle)";
            questItemDrop.Monster = def;
            questItemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.IsArchangelQuestItem()));
            def.DropItemGroups.Add(questItemDrop);
            this.GameConfiguration.DropItemGroups.Add(questItemDrop);
        }
    }
}
