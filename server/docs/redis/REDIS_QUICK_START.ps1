# Redis Implementation - Step by Step for Windows
# ==================================================

Write-Host "🚀 Redis Caching Implementation Guide for Windows" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Start Docker
Write-Host "📦 STEP 1: Start Docker & Run Redis" -ForegroundColor Yellow
Write-Host "-----------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "Make sure Docker Desktop is RUNNING, then run:" -ForegroundColor White
Write-Host ""
Write-Host "  cd server" -ForegroundColor Green
Write-Host "  docker-compose up -d" -ForegroundColor Green
Write-Host ""
Write-Host "Verify Redis is running:" -ForegroundColor White
Write-Host ""
Write-Host "  docker ps | findstr chinese_auction_redis" -ForegroundColor Green
Write-Host "  docker logs chinese_auction_redis" -ForegroundColor Green
Write-Host ""
Write-Host "Open Redis Commander GUI at: " -NoNewline
Write-Host "http://localhost:8081" -ForegroundColor Magenta
Write-Host ""
Write-Host ""

# Step 2: Restore and Build
Write-Host "🔨 STEP 2: Restore & Build .NET Project" -ForegroundColor Yellow
Write-Host "--------------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "  cd ChineseAuctionAPI" -ForegroundColor Green
Write-Host "  dotnet restore" -ForegroundColor Green
Write-Host "  dotnet build" -ForegroundColor Green
Write-Host ""

# Step 3: Run the API
Write-Host "🏃 STEP 3: Run the API" -ForegroundColor Yellow
Write-Host "---------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "  dotnet run" -ForegroundColor Green
Write-Host ""
Write-Host "API should start at: " -NoNewline
Write-Host "http://localhost:5000" -ForegroundColor Magenta
Write-Host "Swagger UI at: " -NoNewline
Write-Host "http://localhost:5000/swagger" -ForegroundColor Magenta
Write-Host ""
Write-Host ""

# Step 4: Test with Swagger
Write-Host "🧪 STEP 4: Test Caching with Swagger" -ForegroundColor Yellow
Write-Host "-----------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Open Swagger UI" -ForegroundColor White
Write-Host "2. Try GET /api/gifts - First call hits DB, caches result" -ForegroundColor White
Write-Host "3. Try GET /api/gifts again - Should hit cache (fast)" -ForegroundColor White
Write-Host "4. Open Redis Commander and verify data" -ForegroundColor White
Write-Host ""
Write-Host ""

# Step 5: Redis CLI commands
Write-Host "📊 STEP 5: Redis CLI - Useful Commands" -ForegroundColor Yellow
Write-Host "-------------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "Connect to Redis:" -ForegroundColor White
Write-Host '  docker exec -it chinese_auction_redis redis-cli -a "YourSecurePassword123!"' -ForegroundColor Green
Write-Host ""
Write-Host "Common commands:" -ForegroundColor White
Write-Host ""
Write-Host "  KEYS *                          # See all keys" -ForegroundColor Cyan
Write-Host "  GET gift:1                      # Get specific gift" -ForegroundColor Cyan
Write-Host "  TTL gift:1                      # See TTL in seconds" -ForegroundColor Cyan
Write-Host "  FLUSHDB                         # Clear all cache" -ForegroundColor Cyan
Write-Host "  INFO stats                      # See Redis stats" -ForegroundColor Cyan
Write-Host "  MONITOR                         # Watch all commands" -ForegroundColor Cyan
Write-Host ""
Write-Host ""

# Step 6: Test Cache Invalidation
Write-Host "✨ STEP 6: Test Cache Invalidation" -ForegroundColor Yellow
Write-Host "---------------------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. GET /api/gifts/{id} - Should cache it" -ForegroundColor White
Write-Host "2. Check Redis CLI: GET gift:{id}" -ForegroundColor White
Write-Host "3. PUT /api/gifts/{id} to update" -ForegroundColor White
Write-Host "4. Check Redis CLI: GET gift:{id} - Should be gone" -ForegroundColor White
Write-Host "5. GET again - Should fetch from DB and re-cache" -ForegroundColor White
Write-Host ""
Write-Host ""

# Step 7: Monitor Logs
Write-Host "📝 STEP 7: Check Logs" -ForegroundColor Yellow
Write-Host "-------------------" -ForegroundColor Yellow
Write-Host ""
Write-Host "The API logs will show:" -ForegroundColor White
Write-Host ""
Write-Host "  - 'Cache hit for key: gift:1'" -ForegroundColor Cyan
Write-Host "  - 'Cache miss for key: gift:1'" -ForegroundColor Cyan
Write-Host "  - 'Cached value for key: gift:1 with TTL: 3600 seconds'" -ForegroundColor Cyan
Write-Host "  - 'Removed cache entry for key: gift:1'" -ForegroundColor Cyan
Write-Host ""
Write-Host ""

Write-Host "✅ Guide completed! Follow the steps above." -ForegroundColor Green
Write-Host ""
