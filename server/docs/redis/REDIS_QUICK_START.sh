#!/bin/bash

# Redis Implementation - Step by Step Guide
# ==========================================

echo "🚀 Redis Caching Implementation Guide"
echo "======================================"
echo ""

# Step 1: Start Docker
echo "📦 STEP 1: Start Docker & Run Redis"
echo "------------------------------------"
echo ""
echo "Make sure Docker Desktop is running, then run:"
echo ""
echo "  cd server"
echo "  docker-compose up -d"
echo ""
echo "Verify Redis is running:"
echo "  docker ps | grep chinese_auction_redis"
echo "  docker logs chinese_auction_redis"
echo ""
echo "Open Redis Commander GUI at: http://localhost:8081"
echo ""

# Step 2: Restore and Build
echo "🔨 STEP 2: Restore & Build .NET Project"
echo "--------------------------------------"
echo ""
echo "  cd ChineseAuctionAPI"
echo "  dotnet restore"
echo "  dotnet build"
echo ""

# Step 3: Run the API
echo "🏃 STEP 3: Run the API"
echo "---------------------"
echo ""
echo "  dotnet run"
echo ""
echo "API should start at: http://localhost:5000"
echo "Swagger UI at: http://localhost:5000/swagger"
echo ""

# Step 4: Test with Swagger
echo "🧪 STEP 4: Test Caching with Swagger"
echo "-----------------------------------"
echo ""
echo "1. Open Swagger UI"
echo "2. Try GET /api/gifts - First call hits DB, caches result"
echo "3. Try GET /api/gifts again - Should hit cache (fast)"
echo "4. Open Redis Commander and verify data"
echo ""

# Step 5: Redis CLI commands
echo "📊 STEP 5: Redis CLI - Useful Commands"
echo "-------------------------------------"
echo ""
echo "Connect to Redis:"
echo '  docker exec -it chinese_auction_redis redis-cli -a "YourSecurePassword123!"'
echo ""
echo "Common commands:"
echo "  KEYS *                          # See all keys"
echo "  GET gift:1                      # Get specific gift"
echo "  TTL gift:1                      # See TTL in seconds"
echo "  FLUSHDB                         # Clear all cache"
echo "  INFO stats                      # See Redis stats"
echo "  MONITOR                         # Watch all commands"
echo ""

# Step 6: Test Cache Invalidation
echo "✨ STEP 6: Test Cache Invalidation"
echo "---------------------------------"
echo ""
echo "1. GET /api/gifts/{id} - Should cache it"
echo "2. Check Redis CLI: GET gift:{id}"
echo "3. PUT /api/gifts/{id} to update"
echo "4. Check Redis CLI: GET gift:{id} - Should be gone"
echo "5. GET again - Should fetch from DB and re-cache"
echo ""

# Step 7: Monitor Logs
echo "📝 STEP 7: Check Logs"
echo "-------------------"
echo ""
echo "The API logs will show:"
echo "  - 'Cache hit for key: gift:1'"
echo "  - 'Cache miss for key: gift:1'"
echo "  - 'Cached value for key: gift:1 with TTL: 3600 seconds'"
echo "  - 'Removed cache entry for key: gift:1'"
echo ""

echo ""
echo "✅ All steps completed!"
echo ""
