
## 1. Service Design (Entities & Responsibility)

### 1.1. User Service

* **Responsibilities:** User registration, authentication, profile management, roles.
* **Entities:** User (IdUser, Identity, FirstName, LastName, Email, PhoneNumber, City, Address, PasswordHash, Role), Card.

### 1.2. Donor Service

* **Responsibilities:** Manage donor profiles and donor-related data.
* **Entities:** Donor (IdDonor, FirstName, LastName, Email, PhoneNumber).

### 1.3. Catalog (Gift) Service

* **Responsibilities:** Manage gifts, gift categories, and inventory.
* **Entities:** Gift (IdGift, Name, Description, CategoryId, Amount, Image, IdDonor, IsDrawn, IdUser), GiftCategory.

### 1.4. Order Service

* **Responsibilities:** Manage orders, order status, and order items.
* **Entities:** Order (IdOrder, IdUser, OrderDate, Status, Price), OrdersGift, OrdersPackage.

### 1.5. Package Service

* **Responsibilities:** Manage packages (bundles of gifts), pricing, and package orders.
* **Entities:** Package (IdPackage, Name, Description, AmountRegular, AmountPremium, Price), OrdersPackage.

### 1.6. Winner Service

* **Responsibilities:** Manage winners and winning gifts.
* **Entities:** Winner (IdWinner, IdUser, IdGift).

### 1.7. Notification/Email Service

* **Responsibilities:** Send notifications and emails for order updates, winners, etc.
* **Entities:** Notification logs (implementation detail).

---

## 2. Database per Service

Each service has its own database. For example:

* User Service DB: Only user and card tables.
* Catalog Service DB: Only gift and category tables.
* Order Service DB: Only order, OrdersGift, OrdersPackage tables.
* Donor Service DB: Only donor table.
* No direct DB access between services.

---

## 3. Inter-service Communication

* **Pattern:** REST APIs or asynchronous messaging (e.g., RabbitMQ).
* **No direct DB access:** Services interact only via APIs.

Example: Order Service verifies a gift in Catalog Service

1. **Order Service** receives a request to create an order for a gift.
2. It calls the **Catalog Service API** (e.g., GET /gifts/{giftId}) to check if the gift exists and is available.
3. **Catalog Service** checks its own DB and returns the gift details or a not-found response.
4. **Order Service** proceeds only if the gift is valid.

---

## 4. API Gateway Roles

* **Routing:** Directs all client requests to the correct microservice.
* **Security:** Handles authentication (JWT), authorization, and rate limiting.
* **Aggregation:** Combines data from multiple services for client responses (e.g., order details with user and gift info).
* **Single Entry Point:** All external access is through the API Gateway.

[Client]
   |
   v
[API Gateway]
   |         |         |         |         |
   v         v         v         v         v
[User]   [Catalog]  [Order]  [Donor]  [Package] ...
 Service   Service   Service   Service   Service
   |         |         |         |         |
[DB]      [DB]      [DB]      [DB]      [DB]
