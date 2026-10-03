# Concept: data-driven item price rules

**Status:** steps 1–4 of the implementation plan (§7) are implemented. §10
summarizes where the implementation deviates from the first draft of this
concept.
**Issue:** [#75 Rule engine for item price calculation](https://github.com/MUnique/OpenMU/issues/75)
**Subject:** `src/GameLogic/ItemPriceCalculator.cs`

## 1. Goal

From the issue:

> Pricing rules defined by data, referenced by the `ItemDefinition`. The
> calculation just works on these rules, without special logics which handles
> specific item numbers. Ideally, new items should never require code
> extensions.

And from the discussion in the issue:

> I'm a fan of implicit logic by convention where possible - that means e.g. if
> an item has excellent option, some kind of exc option rule should
> automatically apply. [...] we need some kind of explicit configuration, but
> only for all these special cases. I think when nothing is configured for an
> item, the automatic logic should apply - otherwise an explicit rule, which
> must be configured in another table(s).

So we need two tiers:

1. **Convention:** the "automatic" price of equipment, made of a base price
   from the drop level and modifiers for each kind of option the item carries.
   This logic stays in code but must not know item numbers or groups.
2. **Explicit rules:** a price definition in the configuration, referenced by
   the `ItemDefinition`. It is needed only for the special items: jewels,
   tickets, potions, pets, quest items and so on.

### Non-goals

* **Changing prices.** The first step must give exactly the same results as
  today. The original client calculates and shows the prices itself (see
  `ItemValue` in `ZzzInfomation.cpp` of
  [MuMain](https://github.com/sven-n/MuMain)), so a server that differs is
  visibly wrong. The known differences to the client are listed in §9 and
  belong in their own pull requests (coding rule 1).
* **Changing the repair price formula.** `CalculateRepairPrice` is generic
  already. It only consumes the buying price.
* **A generic expression or predicate engine for choosing rules.** This also
  answers the question asked in the issue thread: we don't store predicates in
  the database. An item *references* its rule, so nothing has to be matched at
  runtime.

## 2. Current state

`ItemPriceCalculator` (580 lines) mixes three things:

| What | How it is identified today | Count |
|---|---|---|
| Special items with their own price function | `SpecialItems` enum of `(number << 8) + group` ids, looked up in `SpecialItemDictionary` | 83 entries |
| "Old" reference prices for the crafting success rate | `SpecialItemOldValueDictionary`, by id | 5 jewels |
| Items whose `Value` is the price | `Group == 15 \|\| Group == 12` | scrolls, orbs |
| Potions and the antidote | `Group == 14 && Number <= 8`, in buying *and* selling | 9 (group 14, numbers 0–8) |
| Trainable pets | `IsTrainablePet()` (data-based), then `IsDarkRaven()` (`Group 13, Number 5`) | 2 |
| Accessories (`dropLevel³ + 100`) | `Group 12` number ranges, `Group 13`, `Group 15` | rings, pendants, capes, misc. |
| One-handed weapons and shields get 80 % | `Group < 6 && Width < 2`, `Group == 6` | |
| Skills which don't add value | `WorthlessSkills`: skill numbers 66, 223, 224, 225 | 4 skills |
| Equipment (automatic) | everything else | |

Problems:

* **The code knows the data.** A new special item, or a custom item on a private
  server, needs a code change. That is the point of the issue.
* **It assumes one numbering for all versions.** The ids follow the Season 6
  numbering, but the calculator is shared by the 0.75, 0.95d and Season 6
  configurations. Configuration data, on the other hand, is initialized per
  version anyway.
* **`ItemDefinition.Value` means two things.** For scrolls and orbs it is the
  price. For potions it is a factor in `value² * 10 / 12`. For jewels it is set
  (`25`) but not used for the price at all. The original item data keeps two
  separate fields for this (a value and a fixed "zen" price).
* **The tests depend on the ids.** `ItemPriceCalculatorTest` mocks
  `ItemDefinition`s with a group and number, so it tests the hard-coded
  dictionary instead of the configuration which is shipped.
* **The calculator is created with `new` at eight call sites**, see the
  `// TODO: DI? Calculator into gameContext?` in `SellItemToNpcAction`.

## 3. The calculation as a pipeline

Every price the calculator returns is derived from one **buying price**:

```text
buying price  = Cap(GlobalModifiers(OptionModifiers(
                    Quantity(BasePrice(item)))))
final buying  = Round(buying price)
selling price = Round(DurabilityLoss(buying price / 3))
                (per-rule rounding, see §4.1)
repair price  = unchanged, from final buying
crafting ref. = rule.CraftingReferencePrice ?? final buying
```

| Stage | Today | After |
|---|---|---|
| `BasePrice` | dictionary function, or one of the group-based branches | the item's **price rule**; without one, the **automatic equipment price** |
| `Quantity` | written into each dictionary function (`* item.Durability()`) | declared by the rule: none, per piece, or by fill ratio |
| `OptionModifiers` | the `else` branch: skill, luck, option level, wing options, excellent options | applied by convention; a rule decides which kinds apply |
| `GlobalModifiers` | guardian (380) option +16 % | unchanged, applied by convention |
| `Cap` | 3,000,000,000 | unchanged |

The order of the modifiers matters. Each one adds a truncated integer amount,
like the client does, so a different order can change the price by a few zen.
This is why the pipeline has fixed stages and isn't an unordered set of rules.

## 4. Data model

### 4.1 `ItemPriceDefinition` (new)

A shared configuration object. Many item definitions can reference the same
definition, for example all Halloween items, or all one-handed weapons.

```csharp
/// <summary>
/// Defines how the price of an item is calculated, if it differs
/// from the automatic price calculation of equipment.
/// </summary>
[Cloneable]
public partial class ItemPriceDefinition
{
    /// <summary>
    /// Gets or sets the name, e.g. "Jewel of Bless".
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the formula (mXparser syntax) for the base price
    /// of one unit. When it is <c>null</c>, the automatic equipment
    /// price is used. Variables: see the table below.
    /// </summary>
    public string? BasePriceFormula { get; set; }

    /// <summary>
    /// Gets or sets fixed base prices for specific item levels.
    /// They take precedence over the <see cref="BasePriceFormula"/>.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemLevelPrice> PricePerLevel { get; }

    /// <summary>
    /// Gets or sets how the quantity (durability) affects the price.
    /// </summary>
    public ItemPriceQuantityScaling QuantityScaling { get; set; }

    /// <summary>
    /// Gets or sets which option modifiers are applied.
    /// </summary>
    public ItemPriceModifiers Modifiers { get; set; }

    /// <summary>
    /// Gets or sets how the selling price is rounded.
    /// </summary>
    public ItemPriceRounding SellingPriceRounding { get; set; }

    /// <summary>
    /// Gets or sets the price which is used for the success rate of
    /// crafting instead of the buying price. <c>null</c> means that
    /// the buying price is used.
    /// </summary>
    public long? CraftingReferencePrice { get; set; }
}
```

Supporting types:

* `ItemLevelPrice { int Level; long Price; }`: a row of the per-level table
  (Devil's Eye, Blood Bone, Scroll of Archangel, ...).
* `enum ItemPriceQuantityScaling { None, PerPiece, ByFillRatio }`: `PerPiece`
  multiplies by `durability` (SD potions, Symbol of Kundun, ...), and
  `ByFillRatio` multiplies by `durability / maxDurability` (arrows, bolts).
  Like today, it multiplies first and then divides, as integers.
* `[Flags] enum ItemPriceModifiers { None = 0, Skill = 1, Luck = 2, Option = 4,
  WingOption = 8, Excellent = 16, All = ... }`. The guardian option isn't a
  flag: it's applied to every item which has one, like before.
* `enum ItemPriceRounding { Default, Tens }`: `Tens` is the potion rule, which
  only cuts the selling price down to a multiple of 10. `Default` is today's
  `RoundPrice`.

These enums are persisted, so their values are a contract (coding rule 5):
append only.

`GameConfiguration` gets `ICollection<ItemPriceDefinition> ItemPriceDefinitions`
(`[MemberOfAggregate]`, like `ItemLevelBonusTables`), and `ItemDefinition` gets
a reference:

```csharp
/// <summary>
/// Gets or sets the definition of how the price is calculated.
/// When it is <c>null</c>, the price is calculated automatically,
/// like for usual equipment.
/// </summary>
public virtual ItemPriceDefinition? PriceDefinition { get; set; }
```

A reference, not an owned object, because definitions are shared. Also add
`GameConfigurationHelper` entries for the two new types, the EF migration, and
the generated persistence model (coding rules 2 and 4).

### 4.2 Formula variables

The formula engine is [mXparser](https://mathparser.org/), which `GameLogic`
already references for `ItemDefinition.PetExperienceFormula`
(`PetLevelHelper`). It supports `if(cond, a, b)`, `floor(x)` and `^`.

| Variable | Meaning |
|---|---|
| `level` | item level |
| `durability` | current durability, which is the number of pieces for stackable items |
| `maxDurability` | `ItemDefinition.Durability` (full stack, or full quiver) |
| `value` | `ItemDefinition.Value` |
| `dropLevel` | effective drop level: `DropLevel + 3 * level`, plus 25 for excellent items |
| `optionLevel` | level of the item's normal option (type `Option`), or 0 |
| `optionCount` | number of normal options (Dinorant) |
| `healthRecoveryOptionLevel` | level of the normal option, if it's a health recovery option; otherwise 0 (see open question 5) |
| `automaticPrice` | the automatic equipment base price (see §5), before modifiers |

The engine works with `double`. The result is truncated to `long`. Where the
current code relies on integer division *between* steps, the formula has to
`floor` explicitly (see the potion example in the appendix). Prices stay well
below 2⁵³, so `double` is exact enough.

mXparser `Expression` instances keep the argument values as state, so they are
**not thread-safe**. Parse each distinct formula once and keep the parsed
expressions in a pool per formula, or evaluate them under a lock. Prices are
calculated much less often than, for example, damage. Still, the bot shopping
handler prices whole inventories, so we shouldn't parse a formula on every call.

### 4.3 Calculation with a definition

```text
if definition.PricePerLevel has a row for item.Level → base = row.Price
else if definition.BasePriceFormula is set → base = Evaluate(formula)
else → base = automaticPrice
base = Quantity(base, definition.QuantityScaling)
price = ApplyModifiers(base, definition.Modifiers)   // fixed order, see §3
price = price + 16 % if the item has a guardian option
price = Min(price, MaximumPrice)
```

Without a definition: `base = automaticPrice`, `Modifiers = All`, no quantity
scaling.

One rule for all cases: "a row for the level wins, otherwise the formula". It
covers the "price per level with a default" items (Devil's Eye: rows for 1–7,
formula `10000`) and the "special first level, then linear" items (Invisible
Cloak: row 1 = 150,000, formula `600000 + (level - 1) * 60000`).

## 5. The convention tier (no definition)

The automatic price stays code in `GameLogic`, but without any group or item
number:

* `automaticPrice`: drop level formula for normal equipment, and the wing
  formula when `IsWing()` is true. `IsWing()` is in `DataModel` and is based on
  the item's shape and slot, not on ids. The extra drop levels for items above
  +4 (`DropLevelIncreaseByLevel`) belong here.
* Option modifiers, each applied only when the item has an option of that
  `ItemOptionType`: skill +150 %, luck +25 %, normal option by level
  (60 % / 70 % · 2ⁿ⁻¹), +25 % per wing option, +100 % per excellent option.
* Guardian (380) option +16 %, then the cap.

What used to be group checks becomes explicit, shared data:

* **One-handed weapons and shields (80 %)**: a definition "One-handed weapons
  and shields" with `BasePriceFormula = "floor(automaticPrice * 80 / 100)"` and
  `Modifiers = All`. The initializers assign it, since they know which weapon
  is one-handed. Our model has no data-based way to tell (bows are two wide,
  some staffs are one wide), and adding a stat just for pricing would break
  coding rule 3.
* **Skills which don't add value** (Force Wave on scepters, the summoner
  books): items with such a skill get a definition whose `Modifiers` excludes
  `Skill`. Scepters are one-handed as well, so they share a "One-handed weapon,
  skill without value" definition. This is per item and not per skill: the
  same skill on another item (if any) may well add value.
* **Accessories (`dropLevel³ + 100`)**: rings, pendants, capes and the other
  group 13/15 items reference an "Accessory" definition. Today they get "+100 %
  per level of the normal option", but only if that option is health recovery.
  For rings and pendants, health recovery is their only normal option, but
  capes can also have a damage option, which doesn't count. So the formula is
  `(dropLevel^3 + 100) * (1 + healthRecoveryOptionLevel)` (see §9 and open
  question 5).

That removes all group and number checks from the calculator. `IsDarkRaven()`
is no longer needed for pricing either, because the two pets get their own
definitions.

### Later: make the conventions configurable too

Coding rule 3 prefers configuration over constants, and the factors above are
constants. A follow-up can move them into a global
`ItemPriceConfiguration` on `GameConfiguration`:

* the equipment and wing base formulas, and the drop level table above +4,
* the modifier factor per `ItemOptionType` (and for the skill), in their
  fixed order,
* the extra drop level of excellent items (25), the guardian bonus (16 %) and
  the cap.

With that, the calculator needs the `GameConfiguration`, so it should come from
the game context instead of `new` at every call site. That is a separate step
and doesn't block the main goal of the issue. The ids are already gone after
the steps of §7.

## 6. Existing databases

The data in `Persistence/Initialization` only reaches new databases, so the
change needs **both** (coding rule 4):

1. the initializers of every version, which create the definitions and assign
   them; **and**
2. an update plugin, `AddItemPriceDefinitionsPlugIn{075,095d,Season6}`
   with a shared base class, which does the same for existing databases.
   Group and number lookups are fine there: `Persistence.Initialization` is
   allowed to know items.

Once the dictionaries are removed, an existing database without the update
would price a Jewel of Bless as cheap equipment. That breaks the economy, so
the update should be **mandatory**. Before the update there was nothing to
customize, so a mandatory update doesn't override anybody's changes. It only
assigns definitions to items which don't have one yet. Custom items which a
server owner added by hand keep the automatic price, just like today.

Both use the same initializer (`ItemPriceDefinitions`), so a fresh database and
an updated one end up with the same definitions.

Configuration updates are applied by the server owner on the update page of the
admin panel, and take effect after a restart. Until then, an existing database
prices the special items like equipment. The draft proposed a warning for that
case, but the calculator has no access to the configuration, and the old
conditions would bring the item numbers back into the game logic. So the
implementation relies on the update page, which shows pending updates with a
badge. The release notes should mention the update.

## 7. Implementation plan

Each step is its own pull request.

1. **Golden master test, without any behaviour change.** Copy the current
   calculator into the test project as `LegacyItemPriceCalculator`. For each
   version (0.75, 0.95d, Season 6), initialize the configuration in memory. For
   every `ItemDefinition`, every level up to `MaximumItemLevel`, and a set of
   option combinations (none, luck, skill, option 1–4, 1–2 excellent, wing
   options, guardian) and durabilities (full, half, zero, partial stack),
   assert that the current calculator returns the same buying, selling, repair
   and crafting reference prices. This test is the safety net for everything
   below. It also compares the unrounded buying price, because the rounding
   hides small differences. Delete it, with the legacy copy, once the
   migration has been released.
2. **Data model.** `ItemPriceDefinition`, `ItemLevelPrice`, the enums, the
   `ItemDefinition.PriceDefinition` reference, the `GameConfiguration`
   collection, the generated model, the EF migration. No logic yet.
3. **Initializers and update plugin.** Create and assign the definitions for
   every version (see the appendix), plus the update plugin with tests in the
   style of `DarkHorseCanFlyTest`: one test for a fresh database, and one that
   removes the data, applies the update twice and checks the result.
4. **Calculator.** Evaluate the definitions, remove `SpecialItems`,
   `SpecialItemDictionary`, `SpecialItemOldValueDictionary`,
   `WorthlessSkills` and all group/number checks. The golden master test from
   step 1 must still pass. Move the special-item cases of
   `ItemPriceCalculatorTest` to the initialized Season 6 data, so they test the
   shipped configuration and not mocks. The equipment cases can keep their
   mocks, since they exercise the convention tier.
5. **Optional:** the configurable conventions from §5, and getting the
   calculator from the game context.

Steps 1–4 are implemented in one pull request, one commit per step. They are
only useful together, and step 1 makes the review mechanical.

The draft of this concept proposed an additional test: every item which isn't
wearable has a `PriceDefinition`. That isn't true today, see §9.

## 8. Admin panel

The admin panel edits configuration objects generically, so
`ItemPriceDefinition` should show up under the game configuration, and as a
lookup on the item definition, without extra UI code. Nice to have: a
validation of the formula syntax when saving, and a "price preview" which
calculates the price of the item for a given level. Neither is needed for the
first version.

The documentation page of the admin panel for item definitions gets a short
section on how to price a new item.

## 9. Known differences to the client (out of scope)

Found while preparing this concept, for separate issues. Each one needs a check
in the original client first (coding rule 9):

* **Socket items:** the client adds a price bonus per socket option
  (`CalcSocketBonusItemValue`). The server doesn't. With this concept it would
  be another modifier kind.
* **Capes:** the client applies the normal option table to the damage *and*
  the recovery option of capes and wings. The server prices capes as
  accessories, which only reward the recovery option, as noted in the code
  ("possibly a source bug").
* **Arrows and bolts:** their buying price already scales with the fill ratio,
  and selling then subtracts the durability loss again because they are
  wearable. Check whether the client does the same.
* **Items which aren't equipment but get the equipment price:** for example the
  Box of Luck, the Mirror and Sign of Dimensions, the chocolate boxes, the
  Cherry Blossom Play-Box and the GM Gift. They have neither a special price nor
  a `Value`, so the old calculator priced them like equipment by their drop
  level, and they keep that automatic price. Check their prices in the client;
  if they differ, they get a price definition.

## Open questions

1. **Naming:** `ItemPriceDefinition` (like `ItemDefinition` and
   `MagicEffectDefinition`) or `ItemPriceRule` (like the issue)?
2. **Formula vs. typed rules:** this concept uses one class with a formula, a
   level table and a few enums instead of a class hierarchy (`FixedPrice`,
   `LevelTablePrice`, `FormulaPrice`, ...). The persistence model generator and
   the generic admin panel work much better without inheritance, and the
   formula already covers every case in the appendix. Any objections?
3. **`ItemDefinition.Value`:** keep it as the `value` formula variable (scrolls
   and orbs then use the formula `value`, and the admin still edits their price
   in one place). Or, longer term, drop its double meaning and put fixed prices
   into the formula. The concept keeps it, for a smaller migration.
4. **Mandatory update:** agreed that it should be mandatory (see §6)?
5. **Capes:** today they only get the option bonus for a health recovery
   option, not for a damage option. To reproduce that exactly, the formula
   needs a variable for the level of a health recovery option, which ties the
   formula engine to one stat. The alternative is to fix the cape price to
   match the client first (§9), and apply the normal option table to capes,
   like the client does. The implementation adds the variable
   (`healthRecoveryOptionLevel`), so that it changes no price. It can be
   removed again with the fix.

## Appendix: the special items as definitions

This is how every entry of today's `SpecialItemDictionary` and the group-based
branches maps to a definition. "Rows" are `PricePerLevel` entries, and
"Scaling" is `QuantityScaling`. Unless noted otherwise, `Modifiers` is `None`
and `SellingPriceRounding` is `Default`.

**Fixed price** (formula is a constant):

| Item(s) | Formula | Crafting reference |
|---|---|---|
| Jewel of Bless | `9000000` | 100,000 |
| Jewel of Soul | `6000000` | 70,000 |
| Jewel of Chaos | `810000` | 40,000 |
| Jewel of Life | `45000000` | 450,000 |
| Jewel of Creation | `36000000` | 450,000 |
| Jewel of Guardian | `60000000` | |
| Gemstone, Jewel of Harmony, Lower/Higher Refine Stone | `18600` | |
| Fruits | `33000000` | |
| Fragment of Horn / Broken Horn / Horn of Fenrir | `30000` / `90000` / `150000` | |
| Flame of Condor, Feather of Condor | `3000000` | |
| Lost Map | `600000` | |
| Christmas Star, Firecracker | `200000` | |
| Armor of Guardsman | `5000` | |
| Remedy of Love / Ale | `900` / `750` | |

**Linear in the level:**

| Item(s) | Rows | Formula |
|---|---|---|
| Packed jewels (10 kinds) | | `(level + 1) * <jewel price> * 10` |
| Large Healing / Mana Potion | | `1500 * (level + 1)`, Scaling `PerPiece` |
| Devil's Invitation | 1: 60,000; 2: 84,000 | `(level - 1) * 60000` |
| Invisibility Cloak | 1: 150,000 | `600000 + (level - 1) * 60000` |
| Old Scroll, Illusion Sorcerer Covenant, Scroll of Blood | 1: 500,000 | `(level + 1) * 200000` |

**Per-level table:**

| Item(s) | Rows | Formula (other levels) |
|---|---|---|
| Devil's Eye | 1: 10,000 … 7: 1,000,000 | `10000` |
| Devil's Key | 1: 15,000 … 7: 1,500,000 | `15000` |
| Scroll of Archangel, Blood Bone | 1: 10,000 … 8: 1,200,000 | `10000` |
| Loch's Feather | 1: 7,500,000 | `180000` |
| Order Guardian Life Stone | 1: 2,400,000 | `1000000` |
| Contract (Summon) | 0: 1,500,000; 1: 1,200,000 | `0` |
| Spirit (of Dark Horse / Raven) | 0: 30,000,000; 1: 15,000,000 | `0` |
| Wizard's Ring | 0: 30,000 | `0` |
| Gem of Secret | 0: 60,000 | `0` |
| Siege Potion | 0: 900,000 | `450000`, Scaling `PerPiece` |
| Arrows | 1: 1,200; 2: 2,000; 3: 2,800 | `70`, Scaling `ByFillRatio` |
| Bolts | 1: 1,400; 2: 2,200; 3: 3,000 | `100`, Scaling `ByFillRatio` |

**Per piece** (Scaling `PerPiece`):

| Item(s) | Formula |
|---|---|
| Splinter of Armor / Bless of Guardian / Claw of Beast | `150` / `300` / `3000` |
| Small / normal / large Shield Potion | `2000` / `4000` / `6000` |
| Small / normal / large Complex Potion | `2500` / `5000` / `7500` |
| Symbol of Kundun, Suspicious Scrap of Paper, Gaion's Order, Secromicon fragments, Complete Secromicon | `30000` |
| Halloween items (6) | `150` |
| Cherry Blossom items (4) | `300` |

**Other formulas:**

| Item(s) | Formula | Notes |
|---|---|---|
| Rena | `if(level = 3, durability * 3900, 9000)` | |
| Dinorant | `960000 + 300000 * optionCount` | |
| Dark Raven / Dark Horse | `level * 1000000` / `level * 2000000` | durability loss doesn't apply to trainable pets (data-based already) |
| Apple, healing and mana potions, antidote | `floor(floor(value^2 * 10 / 12) * 2^level / 10) * 10` | Scaling `PerPiece`, SellingPriceRounding `Tens` |
| Scrolls, orbs (fixed `Value`) | `value` | |
| Rings, pendants, capes and other accessories | `(dropLevel^3 + 100) * (1 + healthRecoveryOptionLevel)` | open question 5 |
| One-handed weapons, shields | `floor(automaticPrice * 80 / 100)` | `Modifiers = All` |
| Scepters, summoner books | `floor(automaticPrice * 80 / 100)` | `Modifiers = All & ~Skill` |
| Other equipment with one of these skills | automatic | `Modifiers = All & ~Skill` |

## 10. Implementation notes

Where the implementation differs from the draft above, the sections were
updated. In short:

* **Guardian option:** not a modifier flag, but applied to every item with a
  guardian option, like before (§4.1, §4.3).
* **`healthRecoveryOptionLevel`:** an additional formula variable, so that the
  accessories, including capes, keep their prices (open question 5).
* **Selling rounding:** the large healing and mana potions and the siege
  potion are special items *and* potions, so their definitions use the
  `Tens` selling rounding, too.
* **Names:** the definitions are named after their items. Items with the same
  price share one definition (e.g. "Halloween items", "Illusion Temple
  tickets"). The general ones are "Fixed value", "Potions", "Value based",
  "Accessories", "Dark Raven", "Dark Horse", "One-handed weapons and shields",
  "One-handed weapons with a skill without value" and "Equipment with a skill
  without value".
* **Initialization:** the decisions which used item numbers and groups moved
  to `Persistence/Initialization/Items/ItemPriceDefinitions.cs`, which follows
  the same order of checks as the old calculator. It's used by the
  initializers of all versions and by the update plugin. It reuses existing
  definitions by name and skips items which already have one, so applying it
  twice doesn't add anything twice.
