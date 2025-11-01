# Cash Shop Implementation - TODO List & Issues

## AdminPanel Availability

### ❌ NOT Currently Available in AdminPanel Menu
**CashShopProduct** is **NOT** exposed in the AdminPanel navigation menu.

**To Add CashShopProduct to AdminPanel:**

1. **Edit:** `src/Web/AdminPanel/Shared/ConfigNavMenu.razor`
   - Add after line 22 (after Items):
   ```razor
   <NavLink href="@($"edit-config-grid/{typeof(CashShopProduct).FullName}/")">Cash Shop Products</NavLink>
   ```

2. **Add using statement** at the top of the file:
   ```csharp
   @using MUnique.OpenMU.DataModel.Configuration
   ```

3. **Access via "Full Configuration":**
   - CashShopProduct IS accessible via the "Full Configuration" page
   - Navigate to: Game Configuration → Full Configuration → CashShopProducts
   - But this shows the raw configuration editor (not user-friendly)

---

## Critical TODOs & Missing Features

### 1. **Server-to-Client Packet Implementations**

#### 🔴 CRITICAL: ShowCashShopStorageListPlugIn
**File:** `src/GameServer/RemoteView/CashShop/ShowCashShopStorageListPlugIn.cs:35`
```
TODO: Implement proper cash shop storage list serialization
```
**Issue:** Storage list packet requires variable-length structure with item serialization
**Impact:** Players cannot see their cash shop storage items in the UI
**Complexity:** HIGH - Requires:
  - Variable-length C2 packet handling
  - Item data serialization using ItemSerializer
  - Proper byte array construction for StoredItem structure

**Steps to Fix:**
1. Study how `InventoryView` serializes items
2. Create similar serialization for CashShopStorage
3. Use `ItemSerializer` to encode item data
4. Build variable-length packet with proper header

---

#### 🟡 MEDIUM: ShowCashShopEventItemListPlugIn
**File:** `src/GameServer/RemoteView/CashShop/ShowCashShopEventItemListPlugIn.cs:35`
```
TODO: Implement event item list population from game configuration
```
**Issue:** Event item list is never populated or sent to client
**Impact:** Players cannot see event items available for purchase
**Complexity:** MEDIUM - Requires:
  - Filter products where `IsEventItem == true`
  - Serialize CashShopProduct structure
  - Send variable-length packet

**Steps to Fix:**
1. Query `GameConfiguration.CashShopProducts.Where(p => p.IsEventItem)`
2. Map products to `CashShopProduct` structure (16 bytes each)
3. Build C2 packet with product count + array
4. Send via connection extension method (already generated)

---

### 2. **Message Handler Issues**

#### 🟡 MEDIUM: CashShopDeleteStorageItemRequestHandlerPlugIn
**File:** `src/GameServer/MessageHandler/CashShop/CashShopDeleteStorageItemRequestHandlerPlugIn.cs:31`
```
TODO: Map packet fields to actual storage slot
```
**Issue:** Always passes slot `0` - doesn't use actual packet data
**Impact:** Can only delete item in slot 0
**Complexity:** MEDIUM

**Packet Fields Available:**
- `BaseItemCode` (int)
- `MainItemCode` (int)
- `ProductType` (byte)

**Steps to Fix:**
1. Create mapping logic to find item by codes
2. Look up item in `CashShopStorage.Items`
3. Get actual slot number from found item
4. Pass correct slot to delete method

---

### 3. **Business Logic Issues**

#### 🔴 CRITICAL: Gift Message Never Persisted
**File:** `src/GameLogic/Player.cs:944`
```csharp
public async ValueTask<CashShopGiftResult> TrySendCashShopGiftAsync(
    int productId, string receiverName, string message, int coinType)
```
**Issue:** `message` parameter is accepted but NEVER used or stored
**Impact:** Gift messages from sender are lost
**Complexity:** LOW

**Steps to Fix:**
1. Add `GiftMessage` property to Item or create GiftMetadata
2. Store message when creating gift item
3. Display message to receiver when they receive gift

---

#### 🟡 MEDIUM: Product.Item Null Check Missing
**File:** `src/GameLogic/Player.cs:1086`
```csharp
if (this.SelectedCharacter?.CashShopStorage is null || product.Item is null)
```
**Issue:** Only checked in BuyProductToInventory, not in BuyCashShopItem
**Impact:** Potential NullReferenceException if product has no item defined
**Complexity:** LOW

**Steps to Fix:**
1. Add null check in `TryBuyCashShopItemAsync` method (line ~904)
2. Return `CashShopBuyResult.Failed` if product.Item is null
3. Log warning about misconfigured product

---

### 4. **Data Model & Validation Issues**

#### 🟡 MEDIUM: No Price Validation
**File:** `src/DataModel/Configuration/CashShopProduct.cs`
**Issue:** No maximum price validation
**Impact:** Could allow prices > int.MaxValue or negative prices
**Complexity:** LOW

**Steps to Fix:**
1. Add `[Range(0, int.MaxValue)]` attribute to Price property
2. Add validation in business logic before deducting cash
3. Consider adding MaxPrice constant

---

#### 🟡 MEDIUM: No Product Availability Date Range
**File:** `src/DataModel/Configuration/CashShopProduct.cs`
**Issue:** `IsAvailable` is boolean only - no start/end dates
**Impact:** Cannot schedule limited-time offers
**Complexity:** MEDIUM

**Steps to Fix:**
1. Add `AvailableFrom` (DateTime?) property
2. Add `AvailableUntil` (DateTime?) property
3. Update `IsAvailable` to check date range
4. Create `IsCurrentlyAvailable` computed property

---

### 5. **Missing Features**

#### 🔴 CRITICAL: No Purchase History / Audit Log
**Issue:** No tracking of who bought what and when
**Impact:**
  - Cannot track spending
  - Cannot debug purchase issues
  - No fraud detection
**Complexity:** MEDIUM

**Steps to Fix:**
1. Create `CashShopTransaction` entity:
   - AccountId
   - ProductId
   - Amount (price paid)
   - CoinType
   - Timestamp
   - TransactionType (Buy, Gift, Refund)
2. Log all purchases
3. Create AdminPanel view for history

---

#### 🟡 MEDIUM: No Rate Limiting / Spam Prevention
**Issue:** No cooldown on purchases
**Impact:** Potential for spam/abuse
**Complexity:** MEDIUM

**Steps to Fix:**
1. Add rate limiting middleware
2. Track requests per account per timeframe
3. Return error if limit exceeded

---

#### 🟡 MEDIUM: No Refund System
**Issue:** No way to refund purchases
**Impact:** Customer service issues
**Complexity:** MEDIUM

**Steps to Fix:**
1. Add `TryRefundPurchaseAsync` method
2. Validate item is still in storage (not consumed)
3. Remove item from storage
4. Return cash points to account
5. Log refund transaction

---

#### 🟢 LOW: No Category Support in UI
**File:** `src/DataModel/Configuration/CashShopProduct.cs:32`
**Issue:** CategoryIndex exists but no Category entity
**Impact:** Cannot group products in UI
**Complexity:** LOW

**Steps to Fix:**
1. Create `CashShopCategory` entity
2. Add navigation property to CashShopProduct
3. Update initializers to create categories
4. Filter products by category in event list

---

### 6. **Network Protocol Issues**

#### 🔴 CRITICAL: Variable-Length Packet Structure Not Defined
**Files:**
- `ServerToClientPackets.xml:10860` (CashShopEventItemListResponse)
- `ServerToClientPackets.xml:10882` (CashShopStorageListResponse)

**Issue:** Structure arrays need proper packet building
**Impact:** Cannot send lists to client
**Complexity:** HIGH

**Current Definition:**
```xml
<Packet>
  <HeaderType>C2HeaderWithSubCode</HeaderType>
  <Code>D2</Code>
  <SubCode>05</SubCode>
  <Name>CashShopEventItemListResponse</Name>
  <Fields>
    <Field>
      <Index>4</Index>
      <Type>Byte</Type>
      <Name>ItemCount</Name>
    </Field>
    <Field>
      <Index>5</Index>
      <Type>Structure[]</Type>
      <TypeName>CashShopProduct</TypeName>
      <Name>Products</Name>
    </Field>
  </Fields>
</Packet>
```

**Issue:** C# generator doesn't create proper extension methods for variable-length structure arrays

**Workaround Required:**
- Manually build packets using `IConnection.Output.GetSpan()`
- Calculate total length: 5 + (itemCount * 16)
- Write header, subcode, count, then each product manually

---

## Summary Statistics

| Priority | Count | Description |
|----------|-------|-------------|
| 🔴 CRITICAL | 4 | Breaks core functionality |
| 🟡 MEDIUM | 7 | Limits features or has workarounds |
| 🟢 LOW | 1 | Nice to have |
| **TOTAL** | **12** | **Items to address** |

---

## Implementation Roadmap

### Phase 1: Critical Fixes (1-2 days)
1. ✅ Fix PacketType enum reference (DONE)
2. ✅ Fix message handler packet field names (DONE)
3. ⬜ Implement ShowCashShopStorageListPlugIn
4. ⬜ Implement ShowCashShopEventItemListPlugIn
5. ⬜ Fix gift message persistence
6. ⬜ Add purchase history/audit log

### Phase 2: Essential Features (2-3 days)
7. ⬜ Add product.Item null validation
8. ⬜ Fix delete item slot mapping
9. ⬜ Add rate limiting
10. ⬜ Add refund system
11. ⬜ Add CashShopProduct to AdminPanel menu

### Phase 3: Polish & Enhancement (1-2 days)
12. ⬜ Add price validation
13. ⬜ Add availability date ranges
14. ⬜ Add category support
15. ⬜ Add admin interface for managing products

---

## Files Requiring Modifications

### Must Fix:
1. `src/GameServer/RemoteView/CashShop/ShowCashShopStorageListPlugIn.cs`
2. `src/GameServer/RemoteView/CashShop/ShowCashShopEventItemListPlugIn.cs`
3. `src/GameServer/MessageHandler/CashShop/CashShopDeleteStorageItemRequestHandlerPlugIn.cs`
4. `src/GameLogic/Player.cs` (gift message)
5. `src/Web/AdminPanel/Shared/ConfigNavMenu.razor` (add menu item)

### Should Fix:
6. `src/DataModel/Configuration/CashShopProduct.cs` (validation)
7. `src/GameLogic/Player.cs` (null checks)
8. Create new: `src/DataModel/Entities/CashShopTransaction.cs`
9. Create new: `src/DataModel/Configuration/CashShopCategory.cs`

### Optional Enhancements:
10. `src/Persistence/Initialization/*/CashShopProductsInitializer.cs` (categories)
11. Create new: AdminPanel views for transactions and refunds

---

## Testing Checklist

### Manual Testing Required:
- [ ] Buy item with WCoinC
- [ ] Buy item with WCoinP
- [ ] Buy item with Goblin Points
- [ ] Send gift to online player
- [ ] Send gift to offline player
- [ ] View cash shop storage
- [ ] Consume item from storage
- [ ] Delete item from storage
- [ ] View event item list
- [ ] Test with insufficient funds
- [ ] Test with full storage (240 items)
- [ ] Test null product scenario
- [ ] Test null product.Item scenario
- [ ] Verify gift message is stored and displayed

### Integration Testing:
- [ ] AdminPanel can CRUD CashShopProduct
- [ ] Purchase history is logged
- [ ] Refund works correctly
- [ ] Rate limiting prevents spam

---

## Current Build Status

✅ **Build: SUCCESSFUL** (0 errors, 65 warnings - all StyleCop)

All compilation errors have been resolved. The implementation is functionally complete for basic operations but requires the above TODOs for full production readiness.
