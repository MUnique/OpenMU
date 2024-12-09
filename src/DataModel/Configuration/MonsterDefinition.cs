// <copyright file="MonsterDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Type of the window which will be openend when talking to the npc.
/// TODO: Maybe change to a class and do it data-driven.
/// </summary>
public enum NpcWindow
{
    /// <summary>
    /// No window defined.
    /// </summary>
    Undefined,

    /// <summary>
    /// A merchant window.
    /// </summary>
    Merchant,

    /// <summary>
    /// Another merchant window.
    /// </summary>
    Merchant1,

    /// <summary>
    /// A storage window.
    /// </summary>
    Storage,

    /// <summary>
    /// A vault storage.
    /// </summary>
    VaultStorage,

    /// <summary>
    /// A chaos machine window.
    /// </summary>
    ChaosMachine,

    /// <summary>
    /// A devil square window.
    /// </summary>
    DevilSquare,

    /// <summary>
    /// A blood castle window.
    /// </summary>
    BloodCastle,

    /// <summary>
    /// The pet trainer window.
    /// </summary>
    PetTrainer,

    /// <summary>
    /// The lahap window.
    /// </summary>
    Lahap,

    /// <summary>
    /// The castle senior window.
    /// </summary>
    CastleSeniorNPC,

    /// <summary>
    /// The elphis refinery window.
    /// </summary>
    ElphisRefinery,

    /// <summary>
    /// The refine stone making window.
    /// </summary>
    RefineStoneMaking,

    /// <summary>
    /// The jewel of harmony option removal window.
    /// </summary>
    RemoveJohOption,

    /// <summary>
    /// The illusion temple window.
    /// </summary>
    IllusionTemple,

    /// <summary>
    /// The chaos card combination window.
    /// </summary>
    ChaosCardCombination,

    /// <summary>
    /// The cherry blossom branches assembly window.
    /// </summary>
    CherryBlossomBranchesAssembly,

    /// <summary>
    /// The seed master window.
    /// </summary>
    SeedMaster,

    /// <summary>
    /// The seed researcher window.
    /// </summary>
    SeedResearcher,

    /// <summary>
    /// The stat reinitializer window.
    /// </summary>
    StatReInitializer,

    /// <summary>
    /// The delgado lucky coin registration window.
    /// </summary>
    DelgadoLuckyCoinRegistration,

    /// <summary>
    /// The doorkeeper titus duel watch window.
    /// </summary>
    DoorkeeperTitusDuelWatch,

    /// <summary>
    /// The lugard doppelganger entry window.
    /// </summary>
    LugardDoppelgangerEntry,

    /// <summary>
    /// The jerint gaion event entry window.
    /// </summary>
    JerintGaionEvententry,

    /// <summary>
    /// The julia warp market server window.
    /// </summary>
    JuliaWarpMarketServer,

    /// <summary>
    /// The guild master window.
    /// </summary>
    GuildMaster,

    /// <summary>
    /// The dialog window which allows to exchange or refine Lucky Item.
    /// Used by NPC "David".
    /// </summary>
    CombineLuckyItem,

    /// <summary>
    /// The specific npc dialog. The client knows which dialog should be shown.
    /// </summary>
    /// <remarks>
    /// Npc Numbers: 257, 543, 544, 566, 567, 568, 581.
    /// Warning: If the game client doesn't have a dialog for this npc, it will crash.
    /// </remarks>
    NpcDialog,

    /// <summary>
    /// The dialog for the legacy quest system.
    /// </summary>
    LegacyQuest,
}

/// <summary>
/// Type of a non-player-character object.
/// </summary>
public enum NpcObjectKind
{
    /// <summary>
    /// The npc is a monster.
    /// </summary>
    Monster,

    /// <summary>
    /// The npc is passive, e.g. a merchant.
    /// </summary>
    PassiveNpc,

    /// <summary>
    /// The npc is a guard.
    /// </summary>
    Guard,

    /// <summary>
    /// The npc is a trap.
    /// </summary>
    Trap,

    /// <summary>
    /// The npc is a gate
    /// </summary>
    Gate,

    /// <summary>
    /// The npc is a statue.
    /// </summary>
    Statue,

    /// <summary>
    /// The npc is a soccer ball.
    /// </summary>
    SoccerBall,

    /// <summary>
    /// The npc is a destructible.
    /// </summary>
    Destructible,
}

/// <summary>
/// A definition for a monster (or NPC in general).
/// </summary>
[Cloneable]
public partial class MonsterDefinition
{
    /// <summary>
    /// Gets or sets the unique number of this monster.
    /// </summary>
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the designation of this monster.
    /// Not relevant for the server, however helpful for debugging/logging.
    /// </summary>
    public string Designation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the range in which a monster will move randomly?
    /// It is not used yet. TODO: Find out what it's really good for. Remove, if not needed.
    /// </summary>
    public byte MoveRange { get; set; }

    /// <summary>
    /// Gets or sets the attack range in which the monster can attack without moving closer to the target.
    /// </summary>
    /// <value>
    /// The attack range.
    /// </value>
    public byte AttackRange { get; set; }

    /// <summary>
    /// Gets or sets the view range in which the monster can recognize its targets.
    /// </summary>
    public short ViewRange { get; set; }

    /// <summary>
    /// Gets or sets the move delay for each step.
    /// </summary>
    public TimeSpan MoveDelay { get; set; }

    /// <summary>
    /// Gets or sets the attack delay, which is the time between attacks.
    /// </summary>
    public TimeSpan AttackDelay { get; set; }

    /// <summary>
    /// Gets or sets the delay which is waited until a died instance respawns.
    /// </summary>
    public TimeSpan RespawnDelay { get; set; }

    /// <summary>
    /// Gets or sets the attribute.
    /// Not sure what this is.
    /// Maybe the maximum numbers of concurrent additional attributes / magic effects?
    /// TODO.
    /// </summary>
    public byte Attribute { get; set; }

    /// <summary>
    /// Gets or sets the number of maximum item drops after an instance of this monster died.
    /// </summary>
    public int NumberOfMaximumItemDrops { get; set; }

    /// <summary>
    /// Gets or sets the id of the npc window.
    /// </summary>
    public NpcWindow NpcWindow { get; set; }

    /// <summary>
    /// Gets or sets the kind of the object.
    /// </summary>
    public NpcObjectKind ObjectKind { get; set; }

    /// <summary>
    /// Gets or sets the name of the intelligence type, if this npc/monster uses a specific implementation of an INpcIntelligence.
    /// </summary>
    public string? IntelligenceTypeName { get; set; }

    /// <summary>
    /// Gets or sets the skill with which this monster is attacking. Also known as "Attack type".
    /// </summary>
    /// <remarks>The additional damage of the skill is usually NOT applied; However, magic effects are.</remarks>
    public virtual Skill? AttackSkill { get; set; }

    /// <summary>
    /// Gets or sets the items of the merchant store. Is only relevant for merchant NPCs.
    /// </summary>
    [MemberOfAggregate]
    public virtual ItemStorage? MerchantStore { get; set; }

    /// <summary>
    /// Gets or sets the item craftings. Is only relevant for crafting NPCs (chaos goblin etc.).
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemCrafting.ItemCrafting> ItemCraftings { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the drop item groups.
    /// Some monsters drop special items. Examples: Kundun has the chance to drop ancient items.
    /// </summary>
    public virtual ICollection<DropItemGroup> DropItemGroups { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the attributes of this monster.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<MonsterAttribute> Attributes { get; protected set; } = null!;

    /// <summary>
    /// Gets or sets the quests which can be started through this npc.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<QuestDefinition> Quests { get; protected set; } = null!;

    /// <summary>
    /// Attribute default accessor.
    /// </summary>
    /// <param name="key">The attribute.</param>
    /// <returns>The value of the attribute.</returns>
    public float this[AttributeDefinition key]
    {
        get
        {
            return this.Attributes.First(a => a.AttributeDefinition == key).Value;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Designation} ({this.Number})";
    }
}