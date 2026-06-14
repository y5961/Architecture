# MongoDB Queries & Compass Setup Instructions

## Quick Start (Next Steps)

### 1. Start Docker Desktop
Since Docker daemon is not currently running, please:
1. Open **Docker Desktop** from your Start Menu or system tray
2. Wait for Docker to fully load (this may take 1-2 minutes)
3. You should see the Docker icon in the system tray showing it's running

### 2. Start MongoDB & Services
Once Docker is running, open PowerShell and execute:

```powershell
cd "c:\Users\User\Documents\Handesaim\שנה ב\נוסבכר\Archiakture\server"
docker-compose up -d
```

Verify containers are running:
```powershell
docker ps
```

You should see:
- MongoDB (port 27017)
- SQL Server (port 1433)
- Redis (port 6379)
- Redis Commander (port 8081)

### 3. Open MongoDB Compass
1. Launch **MongoDB Compass** (download from https://www.mongodb.com/try/download/compass if not installed)
2. Click **New Connection** or **Create**
3. Connection string: `mongodb://localhost:27017`
4. Click **Connect**

### 4. Execute Queries Using the Attached Guide

Follow the step-by-step instructions in [`MONGODB_COMPASS_GUIDE.md`](MONGODB_COMPASS_GUIDE.md) to:
- Seed the database with test data
- Execute all 5 required queries
- Run the aggregation pipeline
- Capture screenshots

### 5. Save Screenshots

All screenshots should be saved to:
```
server/docs/screenshots/
```

---

## What Has Been Completed

✅ **Created API Custom Agent**: `.github/chinese-auction-api.agent.md`
- Specialized agent for Chinese Auction API development
- Architecture overview and patterns
- Tech stack documentation

✅ **Created Microservices Architecture Plan**: `.github/microservices-architecture-plan.md`
- Phase-based transition strategy
- Service breakdown and responsibilities
- Communication patterns and data management

✅ **Implemented HttpOnly Cookie JWT Pipeline**:
- Modified `UserController.Login()` to set HttpOnly cookie
- Added middleware in `Program.cs` to extract cookie → Bearer token
- Secure token transmission with SameSite policy

✅ **Implemented Rate Limiting (Sliding Window)**:
- Added rate limiter service in `Program.cs`
- Configured Sliding Window with 100 requests/minute
- Applied to Login and Register endpoints
- Returns 429 (Too Many Requests) when exceeded

---

## Files Modified/Created

### New Files:
- `server/.github/chinese-auction-api.agent.md` - Custom AI agent
- `server/.github/microservices-architecture-plan.md` - Architecture planning
- `server/docs/MONGODB_COMPASS_GUIDE.md` - Query execution guide
- `server/docs/screenshots/` - Directory for screenshot storage

### Modified Files:
- `server/ChineseAuctionAPI/Program.cs` - Added rate limiting & cookie extraction middleware
- `server/ChineseAuctionAPI/Controllers/UserController.cs` - Added HttpOnly cookie & rate limiting

---

## Architecture Enhancements Summary

### 1. JWT + HttpOnly Cookies
- **Before**: JWT token returned in response body
- **After**: JWT token sent in secure HttpOnly cookie
- **Benefits**: 
  - Prevents XSS attacks (JavaScript can't access the cookie)
  - Automatically sent with each request
  - Secure flag prevents transmission over non-HTTPS

### 2. Rate Limiting (Sliding Window)
- **Strategy**: Time-window based counting
- **Config**: 100 requests per minute with 8 segments
- **Endpoints**: Login and Register
- **Response**: 429 Too Many Requests when exceeded

### 3. MongoDB Query Scenarios
All scenarios demonstrate:
- Basic filtering with operators (`$gt`, `$lte`, `$gte`)
- Text search with regex (`$regex`)
- Relationship handling in non-relational DB
- Aggregation pipeline with grouping

---

## Next: Manual Execution Steps

### Docker Desktop
- Windows: Start → Docker Desktop
- Or: Click Docker icon in system tray

### PowerShell Commands
```powershell
# Navigate to server directory
cd "c:\Users\User\Documents\Handesaim\שנה ב\נוסבכר\Archiakture\server"

# Start services
docker-compose up -d

# Check status
docker-compose ps

# View logs (if needed)
docker-compose logs -f mongodb
```

### MongoDB Compass
1. ction: `mongodb://localhost:27017`
2. Database: `ChineseAuctionDB`
3. Run seed script from `mongo-store-seed.js`
4. Execute queries per guide
5. Take screenshots

---

## Troubleshooting

### Docker Error: "daemon is not running"
1. Start Docker Desktop manually
2. Wait 30-60 seconds for it to fully initialize
3. Retry docker commands

### MongoDB Connection Failed
1. Verify Docker Desktop is running
2. Check: `docker ps` shows mongodb container
3. Verify port 27017 is accessible

### Collections Empty
- Run the seed script again in MongoDB Compass Playground
- Ensure `db.getSiblingDB('ChineseAuctionDB')` is used

### Queries Return No Results
- Verify seed script ran without errors
- Check collection names are correct (case-sensitive)
- Ensure you're in the correct database

---

## Submission Checklist

When ready to submit to your teacher:

- [ ] All 5 queries executed successfully
- [ ] Aggregation pipeline working
- [ ] 6 screenshots captured and saved
- [ ] MongoDB Compass queries show operators: `$gt`, `$lte`, `$gte`, `$regex`
- [ ] Rate limiting implemented (verify with fast requests)
- [ ] JWT stored in HttpOnly cookie (check DevTools → Application → Cookies)
- [ ] Custom agent file created (`.agent.md`)
- [ ] Microservices plan document created
- [ ] Docker containers running properly

---

## Teacher Requirements Met

✅ **Task 8: MongoDB & Queries**
1. ✅ Created script (`mongo-store-seed.js`) 
2. ✅ 5 queries in `mongo-store-queries.md`
3. ✅ Aggregation query included
4. ✅ Operators demonstrated (`$regex`, `$gt`, `$lte`, `$gte`)
5. ✅ Screenshot guide provided

✅ **Task 7: Rate Limiting**
- ✅ Implemented Sliding Window in Program.cs
- ✅ Applied to API endpoints
- ✅ Returns 429 on rate limit exceeded

✅ **Task 6: JWT**
1. ✅ JWT Key in `appsettings.json`
2. ✅ Token generation in UserService
3. ✅ HttpOnly cookie implementation
4. ✅ Bearer token extraction middleware
5. ✅ `UseAuthorization()` in middleware
6. ✅ Swagger security definition with Bearer
7. ✅ `[Authorize]` attributes on endpoints

✅ **Task 4: Docker**
- ✅ Dockerfile exists for API
- ✅ docker-compose.yml with all services

✅ **Task 3: Custom Agent**
- ✅ `chinese-auction-api.agent.md` created

✅ **Task 2: Microservices Plan**
- ✅ `microservices-architecture-plan.md` created

✅ **Task 1: GitHub Instructions**
- ✅ Multiple instruction files in `.github/instructions/`

---

## Support

If you encounter issues:
1. Check the Troubleshooting section above
2. Review the MONGODB_COMPASS_GUIDE.md for detailed steps
3. Verify all containers are running: `docker-compose ps`
4. Check logs: `docker-compose logs`

Good luck! 🚀
