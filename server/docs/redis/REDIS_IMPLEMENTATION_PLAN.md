# Redis Caching Implementation Plan

## שלב 1: הוספת Redis ב-Docker Compose ✅ COMPLETED

### קבצים שעודכנו:
1. **docker-compose.yml** - יצור חדש עם:
   - Redis service (redis:7-alpine)
   - סיסמא מאובטחת: `YourSecurePassword123!`
   - Redis Commander GUI (port 8081) לניהול נתונים
   - Health check אוטומטי
   - Named volume `redis_data` לשמירה של נתונים

### איך להריץ:
```bash
cd server
docker-compose up -d
```

### דברים שצריך לבדוק:
- **Docker Desktop**: אתה צריך לראות שני containers חדשים:
  - `chinese_auction_redis` - Redis server
  - `redis_commander` - UI ל-Redis (http://localhost:8081)
- **Verify with CLI**:
  ```bash
  docker ps | grep chinese_auction_redis
  docker logs chinese_auction_redis
  ```

---

## שלב 2: עדכון הקוד ✅ COMPLETED

### קבצים שעודכנו:

#### 1. **appsettings.json**
```json
"Redis": {
    "Host": "localhost",
    "Port": 6379,
    "Password": "YourSecurePassword123!",
    "DefaultTtlSeconds": 3600
}
```

#### 2. **Services/Caching/ICacheService.cs** 
- ICacheService interface
- RedisCacheService implementation

#### 3. **Program.cs**
- Redis connection setup
- Service registration

#### 4. **ChineseAuctionAPI.csproj**
- StackExchange.Redis 2.7.10

---

## שלב 3: Service Layer Caching ✅ COMPLETED

עדכון שלוש services עם caching:

1. **GiftService**
   - `GetGiftByIdAsync()` - Cache hit/miss logic
   - `GetAllGiftsAsync()` - List caching
   - `UpdateGiftAsync()` - Cache invalidation
   - `DeleteGiftAsync()` - Cache invalidation

2. **DonorService**
   - Same caching patterns

3. **GiftCategoryService**
   - Same caching patterns

---

## שלב 4: Testing & Verification

### Redis CLI:
```bash
docker exec -it chinese_auction_redis redis-cli -a "YourSecurePassword123!"

# Commands
KEYS *
GET gift:1
TTL gift:1
FLUSHDB
```

### API Testing:
1. GET /api/gifts - First call (cache miss) → caches
2. GET /api/gifts - Second call (cache hit) → returns from cache
3. PUT /api/gifts/{id} - Update → invalidates cache
4. GET /api/gifts/{id} - Next call → fetches from DB

---

## Cache Behavior Summary

| Operation | Action | Cache Effect |
|-----------|--------|--------------|
| GET (first) | Read from DB | Miss → Store in Redis |
| GET (within TTL) | Read from Redis | Hit → Return cached |
| GET (after TTL) | Read from DB | Miss → Store in Redis |
| POST | Create in DB | Invalidate `entity:all` |
| PUT | Update in DB | Invalidate `entity:id` + `entity:all` |
| DELETE | Delete from DB | Invalidate `entity:id` + `entity:all` |

---

## Useful Resources

- Redis CLI: https://redis.io/commands/
- StackExchange.Redis: https://github.com/StackExchange/StackExchange.Redis
- Docker Compose: https://docs.docker.com/compose/
