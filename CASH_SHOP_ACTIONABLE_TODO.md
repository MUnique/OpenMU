# Cash Shop Implementation - Actionable TODO List

**Last Updated:** 2025-11-01
**Build Status:** ✅ Compiles Successfully (0 errors, warnings are StyleCop only)
**Implementation Status:** 90% Complete - Server can process requests but some responses are incomplete

---

## How to Use This List

1. **Tell me which task to do:** Just say "Do task A1" or "Fix the gift message issue"
2. **I'll complete it and update this file:** Marking it as ✅ DONE
3. **Tasks are prioritized:** 🔴 Critical → 🟡 Medium → 🟢 Low
4. **Each task has:** File location, line number, what to do, and why

---

## 🔴 CRITICAL TASKS (Must Fix for Production)

### A1. Gift Message Never Saved 🔴
**Status:** ❌ TODO
**File:** `src/GameLogic/Player.cs:944`
**Line:** 944
**Issue:** Method accepts `string message` parameter but NEVER uses or persists it
**Impact:** Players cannot send messages with gifts
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 15-20 minutes

**What to do:**
1. Add `GiftMessage` string property to `Item` entity OR create metadata
2. Store message when creating gift item (line ~1096)
3. Display message to receiver when viewing storage

**Tell me:** "Do task A1" or "Fix gift message"

---

### A2. Cash Shop Storage List Not Implemented 🔴
**Status:** ❌ TODO
**File:** `src/GameServer/RemoteView/CashShop/ShowCashShopStorageListPlugIn.cs:35`
**Line:** 28-38
**Issue:** Returns empty task - doesn't send storage items to client
**Impact:** Players cannot see items in their cash shop storage
**Difficulty:** ⭐⭐⭐⭐ Very Hard
**Estimated Time:** 2-3 hours

**What to do:**
1. Study `ShowVaultPlugIn` or similar inventory view plugins
2. Use `ItemSerializer` to serialize each item in storage
3. Build variable-length C2 packet manually
4. Write header + count + serialized items
5. Send via connection

**Dependencies:**
- Requires understanding of `ItemSerializer` class
- Requires manual packet building (no auto-generated method for variable arrays)

**Tell me:** "Do task A2" or "Implement storage list view"

---

### A3. Event Item List Not Implemented 🔴
**Status:** ❌ TODO
**File:** `src/GameServer/RemoteView/CashShop/ShowCashShopEventItemListPlugIn.cs:35`
**Line:** 28-37
**Issue:** Returns empty task - doesn't send event items to client
**Impact:** Players cannot see event items available for purchase
**Difficulty:** ⭐⭐⭐ Hard
**Estimated Time:** 1-2 hours

**What to do:**
1. Query products: `GameConfiguration.CashShopProducts.Where(p => p.IsEventItem)`
2. Build C2 packet with CashShopProduct structure (16 bytes each)
3. Calculate length: `5 + (count * 16)`
4. Manually write: Header + SubCode + Count + Products array
5. Send via connection

**Tell me:** "Do task A3" or "Implement event item list"

---

### A4. Delete Item Slot Mapping Broken 🔴
**Status:** ❌ TODO
**File:** `src/GameServer/MessageHandler/CashShop/CashShopDeleteStorageItemRequestHandlerPlugIn.cs:31`
**Line:** 29-36
**Issue:** Always passes slot `0` - doesn't use packet data to find item
**Impact:** Can only delete item in slot 0
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 15-20 minutes

**Packet Fields Available:**
- `BaseItemCode` (int) - First part of item ID
- `MainItemCode` (int) - Second part of item ID
- `ProductType` (byte) - Type identifier

**What to do:**
1. Use packet fields to find matching item in `CashShopStorage.Items`
2. Match item by comparing codes
3. Get actual slot from found item
4. Pass correct slot to delete method

**Tell me:** "Do task A4" or "Fix delete item mapping"

---

### A5. No Purchase Audit Log / History 🔴
**Status:** ❌ TODO
**Files:** NEW files needed
**Issue:** No tracking of who bought what, when, for how much
**Impact:** Cannot debug issues, track spending, detect fraud
**Difficulty:** ⭐⭐⭐ Hard
**Estimated Time:** 2-3 hours

**What to do:**
1. Create new entity: `src/DataModel/Entities/CashShopTransaction.cs`
   ```csharp
   public class CashShopTransaction
   {
       public Guid Id { get; set; }
       public Guid AccountId { get; set; }
       public int ProductId { get; set; }
       public int Amount { get; set; }  // Price paid
       public int CoinType { get; set; } // 0=WCoinC, 1=WCoinP, 2=Goblin
       public DateTime Timestamp { get; set; }
       public TransactionType Type { get; set; } // Buy, Gift, Refund
       public string? ReceiverName { get; set; } // For gifts
   }
   ```
2. Add logging in `TryBuyCashShopItemAsync` and `TrySendCashShopGiftAsync`
3. Persist transaction after successful purchase
4. (Optional) Create AdminPanel view to browse history

**Tell me:** "Do task A5" or "Add purchase history"

---

## 🟡 MEDIUM PRIORITY TASKS (Important but not blocking)

### B1. No Account Balance Validation 🟡
**Status:** ❌ TODO
**File:** `src/DataModel/Entities/Account.cs:121-133`
**Lines:** 121-133
**Issue:** No validation to prevent negative balances
**Impact:** Could allow negative cash points through bugs
**Difficulty:** ⭐ Easy
**Estimated Time:** 5 minutes

**What to do:**
1. Add `[Range(0, int.MaxValue)]` attribute to:
   - `WCoinC` property
   - `WCoinP` property
   - `GoblinPoints` property
2. Or add validation in `TryRemoveCashPoints` method

**Tell me:** "Do task B1" or "Add balance validation"

---

### B2. No Product Price Validation 🟡
**Status:** ❌ TODO
**File:** `src/DataModel/Configuration/CashShopProduct.cs:37-47`
**Lines:** 37-47
**Issue:** Prices can be negative or unreasonably high
**Impact:** Could break economy
**Difficulty:** ⭐ Easy
**Estimated Time:** 5 minutes

**What to do:**
1. Add `[Range(0, 1000000)]` attributes to price properties:
   - `PriceWCoinC`
   - `PriceWCoinP`
   - `PriceGoblinPoints`
2. Consider adding MaxPrice constant

**Tell me:** "Do task B2" or "Add price validation"

---

### B3. No Product Availability Date Range 🟡
**Status:** ❌ TODO
**File:** `src/DataModel/Configuration/CashShopProduct.cs:32`
**Line:** 32
**Issue:** `IsAvailable` is just boolean - cannot schedule limited-time offers
**Impact:** Cannot do timed sales/events
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 20-30 minutes

**What to do:**
1. Add properties:
   ```csharp
   public DateTime? AvailableFrom { get; set; }
   public DateTime? AvailableUntil { get; set; }
   ```
2. Create computed property:
   ```csharp
   public bool IsCurrentlyAvailable =>
       IsAvailable &&
       (!AvailableFrom.HasValue || DateTime.UtcNow >= AvailableFrom) &&
       (!AvailableUntil.HasValue || DateTime.UtcNow <= AvailableUntil);
   ```
3. Update business logic to use `IsCurrentlyAvailable` instead of `IsAvailable`
4. Update database initializers

**Tell me:** "Do task B3" or "Add date range for products"

---

### B4. No Rate Limiting / Spam Prevention 🟡
**Status:** ❌ TODO
**Files:** `src/GameServer/MessageHandler/CashShop/*.cs` (all handlers)
**Issue:** No cooldown on purchase requests
**Impact:** Could spam server with requests
**Difficulty:** ⭐⭐⭐ Hard
**Estimated Time:** 1-2 hours

**What to do:**
1. Add rate limiter service or use existing throttling mechanism
2. Track requests per account per timeframe (e.g., max 10 purchases per minute)
3. Return error result if limit exceeded
4. Consider per-request-type limits

**Tell me:** "Do task B4" or "Add rate limiting"

---

### B5. No Refund System 🟡
**Status:** ❌ TODO
**File:** `src/GameLogic/Player.cs` (new method needed)
**Issue:** No way to refund accidental purchases
**Impact:** Poor customer service experience
**Difficulty:** ⭐⭐⭐ Hard
**Estimated Time:** 1-2 hours

**What to do:**
1. Add method to `Player.cs`:
   ```csharp
   public async ValueTask<RefundResult> TryRefundCashShopPurchaseAsync(byte slot)
   {
       // 1. Validate item exists in storage
       // 2. Validate item hasn't been consumed
       // 3. Remove item from storage
       // 4. Return cash points to account
       // 5. Log refund transaction
   }
   ```
2. Create message handler
3. Create view plugin for refund result
4. Define packets in XML
5. (Optional) Add time limit on refunds (e.g., 24 hours)

**Tell me:** "Do task B5" or "Implement refund system"

---

### B6. Consume Item Handler Uses Wrong Field 🟡
**Status:** ❌ TODO
**File:** `src/GameServer/MessageHandler/CashShop/CashShopStorageItemConsumeRequestHandlerPlugIn.cs:31`
**Line:** 31
**Issue:** Uses `ItemIndex` but packet may have other identifiers
**Impact:** May not find correct item
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 15-20 minutes

**Packet Fields:**
- `BaseItemCode` (int)
- `MainItemCode` (int)
- `ItemIndex` (ushort)
- `ProductType` (byte)

**What to do:**
1. Review packet structure documentation
2. Use correct field(s) to identify item in storage
3. Find item by matching codes
4. Get slot from found item

**Tell me:** "Do task B6" or "Fix consume item mapping"

---

## 🟢 LOW PRIORITY TASKS (Nice to Have)

### C1. No Category Entity / Support 🟢
**Status:** ❌ TODO
**File:** `src/DataModel/Configuration/CashShopProduct.cs:32`
**Line:** 32
**Issue:** `CategoryIndex` exists but no `CashShopCategory` entity
**Impact:** Cannot group products nicely in UI
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 30-45 minutes

**What to do:**
1. Create `src/DataModel/Configuration/CashShopCategory.cs`:
   ```csharp
   public class CashShopCategory
   {
       public int CategoryId { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public int SortOrder { get; set; }
   }
   ```
2. Add to `GameConfiguration.cs`:
   ```csharp
   public virtual ICollection<CashShopCategory> CashShopCategories { get; protected set; }
   ```
3. Add navigation property to `CashShopProduct`:
   ```csharp
   public virtual CashShopCategory? Category { get; set; }
   ```
4. Update initializers to create categories

**Tell me:** "Do task C1" or "Add category support"

---

### C2. AdminPanel Menu Item Added ✅
**Status:** ✅ DONE
**File:** `src/Web/AdminPanel/Shared/ConfigNavMenu.razor:22`
**Completed:** 2025-11-01
**What was done:** Added "Cash Shop Products" menu item after Items

---

### C3. No Maximum Purchase Quantity 🟢
**Status:** ❌ TODO
**File:** `src/DataModel/Configuration/CashShopProduct.cs` (new property)
**Issue:** Can buy unlimited quantity of same item
**Impact:** May want to limit rare items
**Difficulty:** ⭐⭐ Medium
**Estimated Time:** 30 minutes

**What to do:**
1. Add properties to `CashShopProduct`:
   ```csharp
   public int? MaxQuantityPerPurchase { get; set; }
   public int? MaxQuantityPerAccount { get; set; }
   ```
2. Validate in purchase methods
3. Track purchases per account (requires A5 first)

**Tell me:** "Do task C3" or "Add purchase limits"

---

### C4. No Gift Message Character Limit 🟢
**Status:** ❌ TODO (depends on A1)
**File:** `src/GameServer/MessageHandler/CashShop/CashShopItemGiftRequestHandlerPlugIn.cs`
**Issue:** No validation on gift message length
**Impact:** Could send very long messages
**Difficulty:** ⭐ Easy
**Estimated Time:** 5 minutes

**What to do:**
1. Validate `GiftText` length (max 200 chars per packet spec)
2. Truncate or reject if too long
3. Sanitize for XSS/injection

**Tell me:** "Do task C4" or "Add message validation"

---

## 📊 COMPLETION TRACKING

| Priority | Total | Done | Remaining | % Complete |
|----------|-------|------|-----------|------------|
| 🔴 Critical | 5 | 0 | 5 | 0% |
| 🟡 Medium | 6 | 0 | 6 | 0% |
| 🟢 Low | 4 | 1 | 3 | 25% |
| **TOTAL** | **15** | **1** | **14** | **6.7%** |

---

## 🎯 RECOMMENDED ORDER

### Phase 1: Core Functionality (Do First)
1. **A1** - Gift message (easy, user-facing)
2. **A4** - Delete item mapping (easy, functional bug)
3. **B1** - Balance validation (easy, prevents bugs)
4. **B2** - Price validation (easy, prevents bugs)

### Phase 2: Critical Features
5. **A2** - Storage list view (hard but critical)
6. **A3** - Event item list (hard but critical)
7. **A5** - Purchase history (enables other features)

### Phase 3: Polish & Safety
8. **B3** - Date ranges (business feature)
9. **B4** - Rate limiting (security)
10. **B6** - Fix consume mapping (bug)
11. **B5** - Refund system (customer service)

### Phase 4: Nice to Have
12. **C1** - Categories (organization)
13. **C3** - Purchase limits (business feature)
14. **C4** - Message validation (polish)

---

## 📝 QUICK COMMAND REFERENCE

Just tell me:
- `"Do task A1"` - I'll implement gift message storage
- `"Do task A2"` - I'll implement storage list view
- `"Fix gift message"` - Same as A1
- `"Show progress"` - I'll update the completion tracking
- `"Do all easy tasks"` - I'll do A1, A4, B1, B2, C4
- `"Do critical tasks"` - I'll do all 🔴 tasks

---

## 🐛 KNOWN BUGS (Not in TODO)

None currently - all issues are tracked above as TODOs.

---

## ✅ RECENTLY COMPLETED

1. **C2** - AdminPanel menu item added (2025-11-01)
2. Fixed `PacketType.CashShop` → `PacketType.CashShopGroup` compilation error
3. Fixed message handler packet field names (GiftReceiverName, GiftText)
4. Implemented all 8 RemoteView plugins (basic implementation)
5. Defined 8 ServerToClient packets in XML
6. Generated C# packet extension methods

---

## 💡 TIPS

- **Start with easy tasks** (⭐) to build momentum
- **Dependencies:** Some tasks require others (noted in task)
- **Testing:** After completing tasks, test with actual game client
- **Rollback:** Git commit before each major change
- **Ask questions:** If unclear, ask me to explain the task better

---

**END OF TODO LIST**
