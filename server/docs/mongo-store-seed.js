// MongoDB seed script for the Chinese Auction store
// Run in MongoDB Compass `Aggregations` / `Playground` window.

// Switch to the database
const db = db.getSiblingDB('ChineseAuctionDB');

// Cleanup existing collections
const collections = [
  'users',
  'donors',
  'giftCategories',
  'gifts',
  'packages',
  'orders'
];
collections.forEach(name => {
  if (db.getCollectionNames().includes(name)) {
    db.getCollection(name).drop();
  }
});

// Users
db.users.insertMany([
  {
    _id: 1,
    identity: '123456789',
    firstName: 'Dana',
    lastName: 'Cohen',
    email: 'dana.cohen@example.com',
    phoneNumber: '+972501234567',
    city: 'Tel Aviv',
    address: 'רבי עקיבא 10',
    role: 'Manager'
  },
  {
    _id: 2,
    identity: '987654321',
    firstName: 'Eli',
    lastName: 'Levi',
    email: 'eli.levi@example.com',
    phoneNumber: '+972522345678',
    city: 'Jerusalem',
    address: 'ביאליק 15',
    role: 'User'
  }
]);

// Donors
db.donors.insertMany([
  {
    _id: 1,
    firstName: 'Yael',
    lastName: 'Levy',
    email: 'yael.levy@example.com',
    phoneNumber: '+972506789012'
  },
  {
    _id: 2,
    firstName: 'Roni',
    lastName: 'Ben-David',
    email: 'roni.bendavid@example.com',
    phoneNumber: '+972527890123'
  }
]);

// Gift categories
db.giftCategories.insertMany([
  { _id: 1, name: 'Electronics' },
  { _id: 2, name: 'Home & Kitchen' },
  { _id: 3, name: 'Beauty' }
]);

// Gifts
db.gifts.insertMany([
  {
    _id: 1,
    name: 'Smart Speaker',
    description: 'Wi-Fi enabled smart speaker with voice assistant.',
    categoryId: 1,
    amount: 2,
    image: 'smart-speaker.png',
    donorId: 1,
    isDrawn: false,
    userId: null
  },
  {
    _id: 2,
    name: 'Luxury Spa Set',
    description: 'A deluxe spa gift set for relaxation.',
    categoryId: 3,
    amount: 1,
    image: 'spa-set.png',
    donorId: 2,
    isDrawn: false,
    userId: null
  }
]);

// Packages
db.packages.insertMany([
  {
    _id: 1,
    name: 'Bronze Package',
    description: '3 coupons for basic prizes.',
    amountRegular: 3,
    amountPremium: 0,
    price: 50
  },
  {
    _id: 2,
    name: 'Silver Package',
    description: '5 coupons plus one premium entry.',
    amountRegular: 5,
    amountPremium: 1,
    price: 120
  }
]);

// Orders
db.orders.insertMany([
  {
    _id: 1,
    userId: 2,
    orderDate: new Date('2026-05-10T10:30:00Z'),
    status: 1,
    price: 170,
    packages: [
      {
        packageId: 2,
        name: 'Silver Package',
        quantity: 1,
        priceAtPurchase: 120
      }
    ],
    gifts: [
      {
        giftId: 1,
        name: 'Smart Speaker',
        amount: 1,
        categoryId: 1
      }
    ]
  },
  {
    _id: 2,
    userId: 2,
    orderDate: new Date('2026-05-12T15:45:00Z'),
    status: 0,
    price: 50,
    packages: [
      {
        packageId: 1,
        name: 'Bronze Package',
        quantity: 1,
        priceAtPurchase: 50
      }
    ],
    gifts: []
  }
]);

print('MongoDB seed completed.');
