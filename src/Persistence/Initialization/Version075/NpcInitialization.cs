﻿// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal partial class NpcInitialization : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpcInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public NpcInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
        this.ItemHelper = new ItemHelper(this.Context, this.GameConfiguration);
    }

    /// <summary>
    /// Gets the item helper.
    /// </summary>
    protected ItemHelper ItemHelper { get; }

    /// <summary>
    /// Creates all NPCs.
    /// </summary>
    /// <remarks>
    /// Extracted from Monsters.txt by Regex: (?m)^(\d+)\t1\t"(.*?)".*?$
    /// Replace by: yield return new MonsterDefinition() { Number = $1, Designation="$2" };
    /// yield return new (\w*) { Number = (\d+), Designation = (".*?").*?(, NpcWindow = (.*) ){0,1}};
    /// Replace by: <![CDATA[ {\n    var def = this.Context.CreateNew<$1>();\n    def.Number = $2;\n    def.Designation = $3;\n    def.NpcWindow = $5;\n    this.GameConfiguration.Monsters.Add(def);\n}\n ]]>.
    /// </remarks>
    public override void Initialize()
    {
        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 238;
            def.Designation = "Chaos Goblin";
            def.NpcWindow = NpcWindow.ChaosMachine;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 239;
            def.Designation = "Arena Guard";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 240;
            def.Designation = "Baz The Vault Keeper";
            def.NpcWindow = NpcWindow.VaultStorage;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 241;
            def.Designation = "Guild Master";
            def.NpcWindow = NpcWindow.GuildMaster;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 242;
            def.Designation = "Elf Lala";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateElfLalaStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 243;
            def.Designation = "Eo the Craftsman";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateEoTheCraftsmanStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 244;
            def.Designation = "Caren the Barmaid";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateCarenTheBarmaidStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 245;
            def.Designation = "Izabel The Wizard";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateIzabelTheWizardStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 246;
            def.Designation = "Zienna The Weapons Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateZiennaStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 247;
            def.Designation = "Guard";
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 248;
            def.Designation = "Wandering Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateWanderingMerchant();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 249;
            def.Designation = "Guard";
            def.ObjectKind = NpcObjectKind.Guard;
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 250;
            def.Designation = "Wandering Merchant";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateWanderingMerchant();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 251;
            def.Designation = "Hanzo The Blacksmith";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateHanzoTheBlacksmith();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 253;
            def.Designation = "Potion Girl";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePotionGirlItemStorage();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 254;
            def.Designation = "Pasi The Mage";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreatePasiTheMageStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 255;
            def.Designation = "Lumen the Barmaid";
            def.NpcWindow = NpcWindow.Merchant;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.MerchantStore = this.CreateLumenTheBarmaidStore();
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 200;
            def.Designation = "Soccerball";
            def.ObjectKind = NpcObjectKind.SoccerBall;
            this.GameConfiguration.Monsters.Add(def);
        }
    }
}