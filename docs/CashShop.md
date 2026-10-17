# Cash shop (in-game shop)

**Status:** in progress on the branch `feature/cash-shop`; see [Roadmap](#roadmap)
for what's missing.
**Packet group:** `0xD2` ([`PacketType.CashShopGroup`](../src/GameServer/PacketType.cs))
**Operator documentation:** [Cash shop](../docs-website/docs/server-features/cash-shop.md),
[granting coins in the admin panel](../docs-website/docs/admin-panel/accounts.md#granting-cash-shop-coins)
and [through the API](../docs-website/docs/admin-panel/authentication.md#granting-cash-shop-coins)

## Purpose

The cash shop of the Season 6 client (key `X`) sells items for three coins:

| Coin | Packet field | Typically |
|---|---|---|
| WCoin (C) | `CashCredit` | bought with real money |
| WCoin (P) | `CashPrepaid` | bought with prepaid cards |
| Goblin Points | `TotalMileage` | earned by playing, or with "[+GP]" purchases |

Every account has a balance of each coin and a **storage** with two tabs (normal
and gift), which holds bought and gifted items until the player *uses* them.
Using a storage item puts its items into the inventory.

## Client constraints

These facts come from the open source client
[MuMain](https://github.com/sven-n/MuMain) (`src/source/GameShop`,
`src/source/Network/Server/WSclient.cpp`), as protocol facts — see rule 9 of the
[coding rules](CODING_RULES.md).

### The catalog is local

The client reads the catalog from script files in
`Data\InGameShopScript\[SaleZone].[Year].[YearId]\` (`IBSCategory.txt`,
`IBSPackage.txt`, `IBSProduct.txt`) and the banner from `Data\InGameShopBanner\...`.
The server only sends the version to load; the download from Webzen's server
doesn't work. The client ships with the script `512.2012.084` and the banner
`583.2011.001`.

* The shop stays **locked** until the client receives the script version
  (`D2 0C`).
* Names, prices, periods and descriptions shown to the player come from the
  script. The server can't display anything that isn't in it.
* Requests refer to packages and products by the script's **sequence numbers**,
  so the server catalog must use the same numbers.

### Script format

One record per line, fields separated by `@`, CRLF, CP949 (only the Korean
property names are non-ASCII). Sub-lists are separated by `|`.

`IBSPackage.txt` (27 fields), relevant fields:

| Index | Field | Meaning |
|---|---|---|
| 0 | ProductDisplaySeq | category; a package can be in several categories |
| 2 | PackageProductSeq | package sequence number |
| 3 | PackageProductName | name |
| 5 | Price | price of the package (used by bundles) |
| 8 | SalesFlag | `182` for sale, `183` not |
| 9 | GiftFlag | `184` giftable, `185` not |
| 19 | ProductSeqList | products of the package |
| 20 | InGamePackageID | item code `group * 512 + number` |
| 23 | PriceSeqList | price options; **empty for bundles** |
| 25 | CashType | `508` WCoin (C), `509` WCoin (P), `0` Goblin Points |

`IBSProduct.txt` (17 fields) has one line per property of a price option:
`0` ProductSeq, `1` name, `2` property name, `3` value, `4` unit, `5` price,
`6` PriceSeq, `13` item code, `14` PropertySeq, `16` UnitType. A price sequence
belongs to exactly one product.

* Quantity: value of property 7, 8, 9, 11, 30 or 31.
* Usable period: value of property 2, 10, 12, 28 or 58, in unit 386 (seconds),
  174 (minutes) or 172 (hours).

### Protocol

All `0xD2` packets use a C1 header with sub code. The client structures are
packed (`#pragma pack(1)`), so e.g. the doubles of the point info are at
unaligned offsets.

| Sub | Client → server | Server → client | State |
|---|---|---|---|
| 01 | `CashShopPointInfoRequest` | `CashShopPointInfo` (5 doubles) | done |
| 02 | `CashShopOpenState` (`IsClosed`) | `CashShopOpenStateResponse` (`IsAllowed`) | done |
| 03 | `CashShopItemBuyRequest` | `CashShopItemBuyResult` | done |
| 04 | `CashShopItemGiftRequest` | `CashShopItemGiftResult` | done |
| 05 | `CashShopStorageListRequest` (page, `'S'`/`'G'`) | `06` list header, then one `0D` (storage) or `0E` (gift) per item | done |
| 0A | `CashShopDeleteStorageItemRequest` | — | never sent by the client |
| 0B | `CashShopStorageItemConsumeRequest` | `CashShopStorageItemConsumeResult` | done |
| 0C | — | `CashShopScriptVersion` | done |
| 11/12 | — | period item count / expire time | not implemented |
| 13/14 | `CashShopEventItemListRequest` | event item count / list | not implemented |
| 15 | — | `CashShopBannerVersion` | done |

Client behaviour the server depends on:

* **Opening:** only while standing still in a safezone. The client sends
  `02(IsClosed=0)` and, if allowed, requests the points (`01`) and the first
  storage page (`05`). Closing sends `02(IsClosed=1)` and expects no answer.
* **Points** are requested on every opening, after a purchase and after a use.
* **Storage paging:** 9 items per page, one-based page numbers.
* **Buy request:** `PackageMainIndex` is the package sequence,
  `ProductMainIndex` the chosen price sequence — **0 when the package has only
  one price option** — and `CoinIndex` the script's cash type. `MileageFlag` is
  always 0.
* **Use request:** `BaseItemCode` and `MainItemCode` echo the storage index and
  item sequence of `0D`/`0E`. **It doesn't say which tab the item is on.**
* **Gift:** the client only refuses gifting to the own character.
* **Result codes** of buy, gift and use are inline enums in the packet XML.

## Design

### Components

| Layer | Files |
|---|---|
| Packets | `src/Network/Packets/ServerToClient/ServerToClientPackets.xml` (`D2` packets) |
| Data model | `src/DataModel/Configuration/CashShop*.cs`, `src/DataModel/Entities/CashShopStorageItem.cs`, `CashShopCoinGrant.cs`, balances on `Account.cs` |
| Persistence | `src/Persistence/EntityFramework/Extensions/ModelBuilder/CashShopExtensions.cs`, queries on `IPlayerContext` (EF and in-memory), migration `AddCashShop` |
| Initialization | `src/Persistence/Initialization/VersionSeasonSix/CashShopInitializer.cs`, `VersionSeasonSix/Items/CashShopItems.cs`, `Updates/AddCashShopUpdatePlugIn.cs` |
| Game logic | `src/GameLogic/PlayerActions/CashShop/CashShopActions.cs`; `src/GameLogic/CashShop/` (`CashShopFeaturePlugIn` with `CashShopSettings`, `CashShopVersionPlugIn`, `CashShopCoinExtensions`, `CashShopProductExtensions`, `CashShopProductDelivery` with `InventoryProductDelivery` and the delivery plugins); `src/GameLogic/PlugIns/ICashShop*PlugIn.cs`; `Player.IsCashShopOpen` |
| Views | `src/GameLogic/Views/CashShop/` (`ICashShopViewPlugIn`, result enums), implemented by `src/GameServer/RemoteView/CashShop/CashShopViewPlugIn.cs` (`[MinimumClient(6, 0)]`) |
| Handlers | `src/GameServer/MessageHandler/CashShop/` — a group handler and one sub handler per request |
| Admin panel / API | `src/Web/AdminPanel/`: `Services/CashShopCoinService.cs`, `API/CashShopController.cs`, `Pages/CashShopCoins.razor`, `Pages/CashShopCatalog.razor`, `Services/CashShopCatalogCheck.cs` |

### Data model

* **`Account.WCoinC`, `WCoinP`, `GoblinPoints`:** whole numbers; the view
  converts them to the protocol's doubles. Hidden from the generic editor
  (`[Browsable(false)]`).
* **`GameConfiguration.CashShopConfiguration`** (nullable; `null` means no cash
  shop): script and banner versions, and the packages.
  * **`CashShopPackage`:** sequence, name (admin panel only), price, coin type,
    `IsForSale`, `IsGiftable`, `IsBundle`, products.
  * **`CashShopProduct`:** product and price sequence, price, `ItemDefinition`,
    `ItemLevel`, `Quantity`, `Duration`.
  * A **bundle** delivers all its products for the package price; otherwise the
    products are **price options**, of which one is bought.
  * A product (price sequence) can belong to several packages, e.g. two bundles;
    it's unique within a package only.
* **`CashShopStorageItem`** (aggregate root): account id, product and price
  sequence, `IsGift`, sender name, message, `AddedAt`.
* **`CashShopCoinGrant`** (aggregate root): account id, coin type, amount
  (negative takes coins), reason, granted by, optional unique `Reference`,
  `CreatedAt`, `AppliedAt` (`null` = pending). Applied grants remain as history.

### Feature plugin and settings

`CashShopFeaturePlugIn` (`IFeaturePlugIn`) is the master switch: while it's
deactivated, `CashShopFeaturePlugIn.GetSettings` returns `null`, the versions
aren't sent and the shop can't be opened. Its custom configuration,
`CashShopSettings`, holds the behaviour settings: gifting on/off, gifts to the own
account, and the maximum number of storage items per account (0 = unlimited).
The catalog stays in the game configuration, because it refers to item
definitions.

### Plugin points

Custom plugins can hook into purchases (and gifts):

* `ICashShopPackageBuyingPlugIn` is called after the standard checks and before
  the coins are taken, under the player's persistence lock. Setting
  `CancelEventArgs.Cancel` refuses the purchase; the client shows "cannot be
  bought" (or "cannot be gifted"). Use it for custom restrictions, e.g. levels,
  VIP packages or purchase limits.
* `ICashShopPackageBoughtPlugIn` is called after the purchase is saved, with the
  package, the products, the paid price and the gift recipient. Use it for
  bonuses, logging or notifications. The packet handler still holds the player's
  persistence lock, so it must not save other players.

`ICashShopProductDeliveryPlugIn` delivers products when they're used, see
[Product delivery](#product-delivery).

`GoblinPointsForPlayTimePlugIn` (`IPeriodicTaskPlugIn`, disabled by default)
gives Goblin Points for play time. As it runs on the game server, which owns the
balance, it changes the balance directly instead of adding a grant.

### Product delivery

A product is delivered when the player uses it from the storage, not when it's
bought. The storage is the delivery queue: gifts need nothing extra, and a
refused delivery loses nothing.

`CashShopProductDelivery.GetDelivery` selects the delivery by the product's item:

1. the active `ICashShopProductDeliveryPlugIn` (`IStrategyPlugIn<ItemIdentifier>`)
   of the item, then the one of its item group (key without number);
2. none, if a delivery plugin of the item or group is known but inactive: the
   product can't be bought or used until it's active again, instead of putting a
   useless item into the inventory. The plugin manager only knows the types of
   inactive plugins, so their keys are read from new instances;
3. otherwise `InventoryProductDelivery`, which puts the items into the inventory.
   It isn't a plugin and can't be deactivated.

The selected delivery's `CanDeliver(product)` must agree as well; all delivery
plugins refuse time-limited products. A plugin's `DeliverAsync` runs under the
persistence lock before the storage item is removed, and changes nothing unless
it succeeds. `DeliveredAsync` runs after the lock, for messages and view updates.

Built-in delivery plugins, active by default:

| Plugin | Item | Use does | Refused when |
|---|---|---|---|
| `VaultExtensionDeliveryPlugIn` | Vault Expansion Certificate (14/163) | `Account.IsVaultExtended` | the vault is extended |
| `InventoryExtensionDeliveryPlugIn` | Magic Backpack (14/162) | `Character.InventoryExtensions + 1` | the configured maximum (default and limit 4, like the client) is reached |
| `SummonerCharacterCardDeliveryPlugIn` | Summoner Character Card (14/91) | unlocks the Summoner class | it's unlocked or not configured |
| `RageFighterCharacterCardDeliveryPlugIn` | Rage Fighter Character Card (14/169) | unlocks the Rage Fighter class | it's unlocked or not configured |

The client learns the inventory extensions, and the server builds the inventory,
when the character enters the world, so a new extension can be used after
selecting the character again; the player is told so. The plugin
`UnlockSummonerAtLevel1` unlocks the Summoner for every account, so the
Summoner card only makes sense when that plugin is deactivated.

### Flows

* **Entering the game:** `CashShopVersionPlugIn` (CharacterSelection →
  EnteredWorld) sends the script and banner versions, which unlocks the shop.
* **Opening:** allowed when the configuration exists, the feature plugin is
  active, and the player is in
  `EnteredWorld` (not trading, not talking to an NPC), alive and in a safezone.
  Sets `Player.IsCashShopOpen`, which points, storage, buy, gift and use require.
  They check the opening conditions again, because the player may start trading
  or the feature may be deactivated while the shop is open.
* **Points:** applies pending coin grants, then sends the balances.
* **Storage page:** loads all storage items of the account from the database
  and passes them to the view, which filters the tab and pages them.
* **Buy and gift:** share `PurchaseAsync`, which runs under the player's
  persistence lock: find the package, check the coin type, resolve the products
  (bundle, chosen option, or the single option for price sequence 0), check that
  they can be delivered and — for gifts — that the package is giftable, check the
  storage limit of the receiving account, apply
  pending grants, take the coins, create the storage items and save. A failed
  save restores the coins and deletes the storage items.
* **Gift recipient:** looked up by character name with
  `GetAccountIdByCharacterNameAsync`. The settings can refuse gifts in general
  or to the own account. The storage items get `IsGift`, the
  sender's character name and the message.
* **Use:** reloads the storage, checks that the storage index still refers to an
  item with the sent price sequence, delivers the product (see
  [Product delivery](#product-delivery)) and removes the storage item in one save.
  The inventory delivery adds stackable items as stacks up to the maximum
  durability; if one item doesn't fit, nothing is added. If the save fails, the
  changes are committed by the next save; until then, the used storage item is
  skipped, because the database still contains it.

### Administration

* **Coin grants:** the admin panel and the API add a `CashShopCoinGrant`; the
  game server applies it when it shows the points and before a purchase. Pending
  grants of an offline player wait for the next opening of the shop.
* **Role:** `AdminRoles.CashShop` stands outside the
  Viewer < Operator < Administrator hierarchy, so an API key (e.g. of a payment
  provider) can have it alone. Administrators have it implicitly. Policy:
  `AdminPolicies.CashShop`.
* **API:** `GET /api/accounts/{loginName}/cash-shop` and
  `POST /api/accounts/{loginName}/cash-shop/grants`; requests and status codes
  are documented in the
  [operator documentation](../docs-website/docs/admin-panel/authentication.md#granting-cash-shop-coins).
* **Coins page:** `/accounts/{loginName}/cash-shop` shows balances, pending
  amounts and the latest 50 grants, and grants coins. Page and API share
  `CashShopCoinService`.
* **Catalog editor:** `/cash-shop-catalog` (administrators) edits the versions,
  packages and products, and shows why a package can't be sold
  (`CashShopCatalogCheck`). It saves through the configuration data source.

### Catalog initialization

New Season 6 databases get the catalog of the script `512.2012.084`; existing
ones get it with the optional update *Add cash shop*. The initializer does
nothing when a configuration exists. The data follows the script with these
rules:

* one package per `PackageProductSeq` (a package appears in several categories
  with identical data): 205 packages, 487 products;
* coin type from the cash type, `IsForSale` from 182, `IsGiftable` from 184,
  `IsBundle` when the price sequence list is empty;
* price options from the price sequence list; bundle products from the product
  sequence list, each with its first price sequence;
* item from the item code; quantity and duration from the product properties;
* item level from the name, because the script doesn't contain it:
  "Box of Kundun +N" → level N + 7, "Jewel … (10/20/30)" → level 0/1/2.

`CashShopItems` defines the items of the delivery plugins (1x1), before the
catalog is initialized. They have no item rules (`ItemRules`), because they never
reach the inventory: a delivery plugin applies them, or they stay in the storage.

Packages with a product that can't be delivered are initialized as not for sale.
That leaves 21 packages for sale: jewel bundles, Kundun boxes and Cherry Blossom
items for Goblin Points, and the character cards, the Magic Backpack and the
Vault Expansion Certificate for WCoin (C) and (P). Most cash shop items, like
seals, scrolls and talismans, aren't defined in the item configuration yet.

The update also adds the items of the delivery plugins to an existing
configuration, assigns them to its products which have no item, and puts those
packages on sale.

## Rules

* **Balances are only changed by the game server.** It holds the account of an
  online player in memory and would overwrite other changes; other services add
  a coin grant instead.
* **A grant is applied once:** the balance change and `AppliedAt` are saved
  together. Balances never go below zero.
* **Storage items and grants aren't part of the `Account` aggregate** and refer
  to it by `Guid` (the exception of rule 6 of the coding rules): other contexts
  write them while a game server may hold the account.
* **The gift recipient's account is never loaded** into the sender's context, so
  the sender's save can't overwrite it.
* **The storage index sent to the client is the item's index in all storage
  items of the account**, because the use request doesn't name the tab. The item
  sequence is the price sequence, which the server checks on use.
* **One delivery decision:** `CashShopProductDelivery.CanDeliver` decides for
  purchases and use. Initialization and the catalog editor don't know the plugins
  of a game server and use `CashShopProductExtensions.CanBeDelivered` (item
  defined, not time-limited) as approximation. Time-limited products aren't
  delivered, because items can't expire yet.
* **Sequence numbers must match the client script** of the configured version.

## Decisions

| Decision | Reason |
|---|---|
| Pre-fill the catalog from the client's `512.2012.084` | the stock client works out of the box |
| Coins as whole numbers | the values are integral; doubles only exist in the protocol |
| The *Add cash shop* update is optional | not every server offers a cash shop |
| The catalog editor only edits the server side | the client script stays the source of truth for the stock client |
| No item templates (options) for products | the client can't show options of its script items; revisit with a server-side catalog |
| Deliver products on use, select the delivery by the product's item | the storage is the queue, gifts work; the client identifies products by item, like the consume handlers |
| A known but inactive delivery plugin blocks its products | they'd be useless items in the inventory |
| A delivery is refused only on use, not on purchase | a gift recipient's account isn't loaded; the product stays in the storage |

## Roadmap

1. **Items which expire:** expiry on `Item`, the period packets `D2 11/12`, then
   selling time-limited products. This unlocks most of the catalog.
2. **Missing cash shop items:** item definitions for seals, scrolls, talismans,
   tickets etc., with their effects.
3. **Event item list:** `D2 13/14`.
4. **More ways to earn coins:** e.g. Goblin Points for "[+GP]" purchases (an
   `ICashShopPackageBoughtPlugIn`), kills or events.
5. **More delivery plugins:** e.g. Premium Service, once there is a premium model.
6. **Server-side catalog:** the client queries or receives the catalog from the
   server instead of reading its script, with new `…Extended` packets for the
   open source client (rule 2); the original packets stay for the stock client.
   This needs display data in the catalog (categories, names, descriptions) and
   is where item templates become useful.

Before the pull request(s):

* split into focused pull requests along the commits of the branch, if the
  maintainers prefer: the three fixes; packets; data model; game logic;
  initialization; administration; documentation;
* test the delivery plugins with the game client, and on PostgreSQL that a
  catalog change reaches a running game server without a restart.

## Testing

| Tests | Covers |
|---|---|
| `tests/MUnique.OpenMU.Tests/CashShopActionsTest.cs` | opening, points, storage, buy, use (incl. rollback on a full inventory), gift, coin grants |
| `tests/MUnique.OpenMU.Tests/CashShopProductDeliveryTest.cs` | delivery selection, inactive plugins, the built-in delivery plugins |
| `tests/MUnique.OpenMU.Tests/GoblinPointsForPlayTimePlugInTest.cs` | Goblin Points for play time |
| `tests/MUnique.OpenMU.Tests/CashShopRemoteViewTests.cs` | packet bytes, paging, storage indexes |
| `tests/MUnique.OpenMU.Persistence.Initialization.Tests/CashShopUpdateTest.cs` | fresh database, update plugin incl. a catalog without the delivery items, idempotence |
| `tests/MUnique.OpenMU.Web.Tests/Api/CashShopControllerTests.cs` | API status codes, references |
| `tests/MUnique.OpenMU.Web.Tests/Pages/CashShopCoinsPageTests.cs` | coins page |
| `tests/MUnique.OpenMU.Web.Tests/Pages/CashShopCatalogPageTests.cs` | catalog editor, `CashShopCatalogCheck` |
| `tests/MUnique.OpenMU.Network.Packets.Tests` | generated packet structure tests |

The tests use the in-memory persistence. Verified with the game client on
PostgreSQL: the shop opens and lists the items; buying with WCoin (C), WCoin (P)
and Goblin Points; using an own purchase, which moves it into the inventory;
sending a gift and using it from the gift storage of the recipient; granting
coins in the admin panel. Not verified yet: the delivery plugins, and changes of
the catalog or the plugin settings in the admin panel. Loading the catalog from
PostgreSQL needs `IntervalAsTimeSpanJsonConverter`, because PostgreSQL writes a
product duration of a day or more as e.g. `"7 days"` into the JSON.

What can be bought: 21 of 205 packages (see
[Catalog initialization](#catalog-initialization)). The others have items OpenMU
doesn't define, or are time-limited.

## Working on it

* **Packets:** edit the XML, then build `MUnique.OpenMU.Network.Packets` and the
  packet test project and commit the generated files. An inline enum must not
  have the name of its packet.
* **Data model:** build `Persistence/SourceGenerator` first (the persistence
  projects run it with `--no-build`), then the persistence projects; the first
  build after adding a type may fail until the generated files exist.
* **Migrations:** `dotnet ef migrations add <Name> --context EntityDataContext`
  in `src/Persistence/EntityFramework` (`dotnet-ef` 10.0.2). Don't redo a
  migration with `migrations remove`: it rebuilds the snapshot from an older
  designer model that lacks some columns. Delete the migration files, restore
  the snapshot from `master` and add the migration again.
* **Resources:** the `Designer.cs` files are maintained by hand; change them
  together with the `.resx`.
* **Plugin GUIDs** are generated, never typed or copied (rule 5).
* **QuickGrid and `@key`** compare rows by `Id`. Objects from a persistence
  context have one; objects created with `new` in tests need one.
