# MongoDB Compass Query Execution & Screenshot Guide

## Prerequisites
1. MongoDB installed and running (Docker container via docker-compose)
2. MongoDB Compass installed
3. .NET development environment for running the seed script (if not using Docker Compass directly)

## Step 1: Ensure MongoDB is Running

### Option A: Using Docker Compose (Recommended)
```powershell
cd server
docker-compose up -d
```

Verify MongoDB is running:
```powershell
docker ps
# Look for MongoDB service running on port 27017
```

### Option B: Local MongoDB
Ensure MongoDB server is running on `localhost:27017`

---

## Step 2: Connect MongoDB Compass

1. Open **MongoDB Compass**
2. In the connection string field, enter:
   ```
   mongodb://localhost:27017
   ```
3. Click **Connect**

---

## Step 3: Seed the Database

### Using MongoDB Compass Playground

1. In MongoDB Compass, right-click on your connection
2. Select **Create Database**
   - Database name: `ChineseAuctionDB`
   - Collection name: `dummy` (will be replaced)
3. Click **Create Database**

4. Click on the **ChineseAuctionDB** database
5. Click the **>_** icon to open **Playground**
6. Paste the entire seed script from `server/docs/mongo-store-seed.js`
7. Click the **▶ Run** button or press `Ctrl+Enter`

**Expected Result**: You should see output showing collections created with documents inserted

---

## Step 4: Execute Each Query & Capture Screenshots

### Query 1: Find All Gifts in Category with Positive Stock

**Description**: מציאת כל המתנות בקטגוריה מסוימת עם מלאי חיובי

**Steps**:
1. Open **Playground** for the `ChineseAuctionDB` database
2. Clear previous code and paste:
```javascript
const db = db.getSiblingDB('ChineseAuctionDB');
db.gifts.find({ categoryId: 3, amount: { $gt: 0 } });
```
3. Click **▶ Run** or press `Ctrl+Enter`
4. Take a screenshot showing:
   - The query code
   - The results (should show Luxury Spa Set)
   - The result count

**File**: `screenshots/query_1_gifts_by_category.png`

---

### Query 2: Find All Orders by User

**Description**: מציאת כל ההזמנות של משתמש

**Steps**:
1. Open **Playground** for the `ChineseAuctionDB` database
2. Clear and paste:
```javascript
const db = db.getSiblingDB('ChineseAuctionDB');
db.orders.find({ userId: 2 });
```
3. Click **▶ Run**
4. Take a screenshot showing:
   - The query code
   - The results (should show 2 orders for user 2)
   - Order details

**File**: `screenshots/query_2_user_orders.png`

---

### Query 3: Find All Packages Under Price Limit

**Description**: מציאת כל החבילות במחיר עד 100

**Steps**:
1. Open **Playground**
2. Clear and paste:
```javascript
const db = db.getSiblingDB('ChineseAuctionDB');
db.packages.find({ price: { $lte: 100 } });
```
3. Click **▶ Run**
4. Take a screenshot showing:
   - The query code
   - The results (should show Bronze Package with price 50)
   - Result count

**File**: `screenshots/query_3_packages_price.png`

---

### Query 4: Search Donors by Email Pattern (Regex Operator)

**Description**: שימוש באופרטור Regex לחיפוש תורמים עם כתובת Gmail

**Steps**:
1. Open **Playground**
2. Clear and paste:
```javascript
const db = db.getSiblingDB('ChineseAuctionDB');
db.donors.find({ email: { $regex: /@gmail\.com$/i } });
```
3. Click **▶ Run**
4. Take a screenshot showing:
   - The query code
   - The results (should show donors with @gmail.com emails)
   - Demonstrating the regex operator usage

**File**: `screenshots/query_4_donors_gmail_regex.png`

---

### Query 5: Find Recent Orders with Date Comparison

**Description**: חיפוש הזמנות אחרונות (Bonus Query using $gte operator)

**Steps**:
1. Open **Playground**
2. Clear and paste:
```javascript
const db = db.getSiblingDB('ChineseAuctionDB');
db.orders.find({ 
  orderDate: { $gte: new Date('2026-05-11T00:00:00Z') }
});
```
3. Click **▶ Run**
4. Take a screenshot showing:
   - The query code
   - The results showing orders from the specified date
   - Demonstrating the $gte operator

**File**: `screenshots/query_5_recent_orders.png`

---

### Aggregation Query: Revenue Summary by Order Status

**Description**: Aggregation - חישוב סך ההכנסות לכל סטטוס הזמנה

**Steps**:
1. Click on **Aggregations** tab (not Playground)
2. The aggregation builder will appear
3. Click **+ Add Stage**
4. Select **$group** stage
5. Paste or input:
```javascript
{
  _id: '$status',
  totalRevenue: { $sum: '$price' },
  count: { $sum: 1 }
}
```
6. Click **▶ Run** or auto-executes
7. Add another stage: **$project**
```javascript
{
  status: '$_id',
  totalRevenue: 1,
  count: 1,
  _id: 0
}
```
8. Take a screenshot showing:
   - Both aggregation stages
   - The results showing revenue breakdown by status
   - The aggregation pipeline visualization

**File**: `screenshots/query_aggregation_revenue_status.png`

---

## Step 5: Verify Query Results

Expected Results Summary:

| Query | Expected Count | Description |
|-------|----------------|-------------|
| Q1: Gifts in category 3 with stock | 1 | Luxury Spa Set |
| Q2: Orders by user 2 | 2 | Two orders |
| Q3: Packages ≤ 100 | 1 | Bronze Package ($50) |
| Q4: Gmail donors | 2 | Yael & Roni (Gmail) |
| Q5: Recent orders | 1-2 | Orders from 2026-05-10+ |
| Aggregation | 2 groups | Status 0 & 1 revenue |

---

## Step 6: Troubleshooting

#ction refused"
- Ensure Docker is running: `docker ps`
- Ensure MongoDB container is up: `docker-compose ps`
- Try: `docker-compose restart`

### "Database not found"
- Run the seed script again
- Ensure you're connected to the correct MongoDB instance

### "No results"
- Verify the collection name is correct (case-sensitive)
- Check that seed script ran successfully
- Review the query syntax

### Seed Script Errors
If the seed script fails:
1. Clear all collections: Open console in each collection and run:
   ```javascript
   db.collection_name.deleteMany({})
   ```
2. Re-run the seed script

---

## Step 7: Save Screenshots

Create a folder in your project:
```
server/docs/screenshots/
```

Place screenshots with these names:
- `query_1_gifts_by_category.png`
- `query_2_user_orders.png`
- `query_3_packages_price.png`
- `query_4_donors_gmail_regex.png`
- `query_5_recent_orders.png`
- `aggregation_revenue_status.png`

---

## Alternative: Running Queries via Code

If you prefer to execute queries programmatically from .NET:

```csharp
// Example: Injected MongoOrderQueryService
var recentOrders = await _mongoOrderQueryService.GetRecentOrdersSinceAsync(
    DateTime.UtcNow.AddDays(-1)
);

var ordersWithMinGifts = await _mongoOrderQueryService.GetOrdersWithMinimumGiftLinesAsync(1);
```

See `MongoOrderQueryService.cs` for more query examples.

---

## Final Checklist

- [ ] MongoDB running (docker-compose or local)
- [ ] MongoDB Compass connected
- [ ] Database `ChineseAuctionDB` created
- [ ] Seed script executed successfully
- [ ] Query 1: Gifts by category - Screenshot captured
- [ ] Query 2: Orders by user - Screenshot captured
- [ ] Query 3: Packages by price - Screenshot captured
- [ ] Query 4: Donors by email regex - Screenshot captured
- [ ] Query 5: Recent orders - Screenshot captured
- [ ] Aggregation query - Screenshot captured
- [ ] All screenshots placed in `server/docs/screenshots/`
- [ ] Verified results match expected outputs

---

## Notes for Teacher Submission

When submitting this assignment, include:
1. This guide (proving understanding of MongoDB queries)
2. All 6 screenshot files showing successful query execution
3. The seed script (already provided: `mongo-store-seed.js`)
4. Evidence of aggregation pipeline understanding

The queries demonstrate:
- ✅ Basic find operations (Q1, Q2, Q3)
- ✅ Operator usage: `$gt`, `$lte`, `$gte`, `$regex` (Q4, Q5)
- ✅ Complex aggregation with grouping and projection
- ✅ Proper relationship handling in non-relational database
- ✅ Data integrity from relational → document conversion
