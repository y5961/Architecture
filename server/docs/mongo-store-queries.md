# MongoDB Queries for the Chinese Auction Store

## 1. מציאת כל המתנות בקטגוריה מסוימת עם מלאי חיובי
```js
const db = db.getSiblingDB('ChineseAuctionDB');
db.gifts.find({ categoryId: 3, amount: { $gt: 0 } });
```

## 2. מציאת כל ההזמנות של משתמש
```js
const db = db.getSiblingDB('ChineseAuctionDB');
db.orders.find({ userId: 2 });
```

## 3. מציאת כל החבילות במחיר עד 100
```js
const db = db.getSiblingDB('ChineseAuctionDB');
db.packages.find({ price: { $lte: 100 } });
```

## 4. שימוש באופרטור Regex לחיפוש תורמים עם כתובת Gmail
```js
const db = db.getSiblingDB('ChineseAuctionDB');
db.donors.find({ email: { $regex: /@gmail\.com$/i } });
```

## 5. Aggregation: חישוב סך ההכנסות לכל סטטוס הזמנה
```js
const db = db.getSiblingDB('ChineseAuctionDB');
db.orders.aggregate([
  {
    $group: {
      _id: '$status',
      totalRevenue: { $sum: '$price' },
      count: { $sum: 1 }
    }
  },
  {
    $project: {
      status: '$_id',
      totalRevenue: 1,
      count: 1,
      _id: 0
    }
  }
]);
```

## הפעלת השאילתות ב-MongoDB Compass
1. פתח את MongoDB Compass.
2. התחבר ל-`mongodb://localhost:27017`.
3. בחר את מסד הנתונים `ChineseAuctionDB`.
4. בחר טאב **Playground**.
5. הדבק את כל הקוד של Seed (מהקובץ mongo-store-seed.js) והריץ את זה קודם.
6. ואז הדבק כל שאילתה בנפרד בחלון Playground וריץ.
7. עבור Aggregation בחלונית החדשה, בחר **Aggregation** במקום **Playground**.
