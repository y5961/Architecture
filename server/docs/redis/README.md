# Redis Caching Implementation

This directory contains all documentation and guides for the Redis caching implementation in the Chinese Auction API.

## 📚 Files Overview

### 1. **REDIS_IMPLEMENTATION_PLAN.md**
Complete technical documentation of the Redis caching implementation:
- Docker setup with Redis and Redis Commander
- Code integration (Program.cs, appsettings.json, services)
- Service layer caching patterns
- Cache behavior and TTL management
- Testing strategies

**Start here for:** Understanding the complete architecture

---

### 2. **REDIS_QUICK_START.ps1** (Windows)
Quick start guide with colored output for Windows PowerShell:
- Step-by-step instructions
- Docker commands for Windows
- Redis CLI usage
- Cache testing scenarios
- Log monitoring

**How to run:**
```powershell
.\REDIS_QUICK_START.ps1
```

---

### 3. **REDIS_QUICK_START.sh** (Linux/Mac)
Quick start guide for bash shell:
- Same steps as PowerShell version
- Linux/Mac compatible commands

**How to run:**
```bash
chmod +x REDIS_QUICK_START.sh
./REDIS_QUICK_START.sh
```

---

## 🚀 Quick Start (5 minutes)

### Prerequisites:
- Docker Desktop running
- .NET 8 SDK installed
- cd to project root

### Steps:

1. **Start Redis**
   ```bash
   cd server
   docker-compose up -d
   ```

2. **Build API**
   ```bash
   cd ChineseAuctionAPI
   dotnet restore
   dotnet build
   ```

3. **Run API**
   ```bash
   dotnet run
   ```

4. **Test**
   - Open: http://localhost:5000/swagger
   - Redis Commander: http://localhost:8081
   - CLI: `docker exec -it chinese_auction_redis redis-cli -a "YourSecurePassword123!"`

---

## 📊 Cache Keys Reference

| Entity | Single | List | Pattern |
|--------|--------|------|---------|
| Gift | `gift:1` | `gift:all` | `gift:*` |
| Donor | `donor:1` | `donor:all` | `donor:*` |
| Category | `category:1` | `category:all` | `category:*` |

---

## 🔍 Common Redis CLI Commands

```bash
# Connect
docker exec -it chinese_auction_redis redis-cli -a "YourSecurePassword123!"

# Inside redis-cli
KEYS *              # See all keys
GET gift:1          # Get specific key
TTL gift:1          # See TTL in seconds
FLUSHDB             # Clear all cache
INFO stats          # Redis statistics
MONITOR             # Watch all commands in real-time
DEL key1 key2       # Delete specific keys
EXPIRE key 60       # Set TTL to 60 seconds
```

---

## ✨ Cache Behavior

### **Read Operations (GET)**
1. Check Redis cache
2. If found → Return (cache hit)
3. If not found → Query database → Store in Redis → Return

### **Write Operations (POST/PUT/DELETE)**
1. Modify in database
2. Invalidate related cache keys
3. On next GET → Fetch from database and re-cache

### **TTL (Time To Live)**
- Default: 3600 seconds (1 hour)
- Configurable in `appsettings.json`
- Auto-expiration removes old entries

---

## 📁 Project Structure

```
server/
├── docker-compose.yml           ← Redis config
├── .env                         ← Environment variables
└── ChineseAuctionAPI/
    ├── Program.cs               ← Redis registration
    ├── appsettings.json         ← Redis settings
    ├── ChineseAuctionAPI.csproj ← NuGet packages
    └── Services/
        ├── Caching/
        │   └── ICacheService.cs ← Cache abstraction
        ├── GiftService.cs       ← With caching
        ├── DonorService.cs      ← With caching
        └── GiftCategoryService.cs ← With caching

docs/
└── redis/
    ├── README.md                ← This file
    ├── REDIS_IMPLEMENTATION_PLAN.md
    ├── REDIS_QUICK_START.ps1
    └── REDIS_QUICK_START.sh
```

---

## 🐛 Troubleshooting

### "Connection refused"
```bash
# Check if Redis is running
docker ps | grep chinese_auction_redis

# Restart if needed
docker-compose restart redis
```

### "Authentication failed"
- Verify password in `appsettings.json` matches `.env`
- Check Redis logs: `docker logs chinese_auction_redis`

### "Cache not working"
1. Check Redis Commander: http://localhost:8081
2. Verify keys exist: `KEYS *` in redis-cli
3. Check API logs for errors
4. Clear cache: `FLUSHDB` and retry

---

## 🔐 Security Notes

- ✅ Password stored in `.env` (not hardcoded)
- ✅ `.env` added to `.gitignore`
- ⏭️ Production: Use Azure Key Vault
- ⏭️ Production: Enable TLS/SSL
- ⏭️ Production: Strong random passwords

---

## 📝 Logging

All cache operations logged at Information level:

```
Information: Cache hit for key: gift:1
Information: Cache miss for key: gift:1
Information: Cached value for key: gift:1 with TTL: 3600 seconds
Information: Removed cache entry for key: gift:1
Information: Removed 2 cache entries matching pattern: gift:*
```

Check logs in:
- Console (when running `dotnet run`)
- `logs/` directory (Serilog)

---

## 📚 Additional Resources

- [Redis Documentation](https://redis.io/)
- [StackExchange.Redis GitHub](https://github.com/StackExchange/StackExchange.Redis)
- [Docker Compose Guide](https://docs.docker.com/compose/)
- [Redis Best Practices](https://redis.io/docs/management/optimization/)

---

## ✅ Implementation Checklist

- [x] Docker Redis setup
- [x] Code integration
- [x] Service layer caching
- [x] Cache invalidation
- [x] Documentation
- [ ] Load testing (optional)
- [ ] Monitoring setup (optional)
- [ ] Production deployment (future)

---

**Last Updated:** April 22, 2026
**Status:** Ready for testing and development
