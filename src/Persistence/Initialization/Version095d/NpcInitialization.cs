// <copyright file="NpcInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal partial class NpcInitialization : Version075.NpcInitialization
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
        base.Initialize();
        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 235;
            def.Designation = "Sevina the Priestess";
            def.NpcWindow = NpcWindow.LegacyQuest;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 236;
            def.Designation = "Golden Archer";
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }

        {
            var def = this.Context.CreateNew<MonsterDefinition>();
            def.Number = 237;
            def.Designation = "Charon";
            def.NpcWindow = NpcWindow.DevilSquare;
            def.ObjectKind = NpcObjectKind.PassiveNpc;
            def.SetGuid(def.Number);
            this.GameConfiguration.Monsters.Add(def);
        }
    }
}