# Configuration Guide
# ===================

## Redis Configuration

### Environment Variables
The Redis password is stored in `server/.env` file:
```
REDIS_PASSWORD=YourSecurePassword123!
```

### Docker Compose
The password is passed to both Redis and Redis Commander via the .env file.

### appsettings.json
Default configuration for local development:
```json
"Redis": {
    "Host": "localhost",
    "Port": 6379,
    "Password": "YourSecurePassword123!",
    "DefaultTtlSeconds": 3600
}
```

## Changing the Password

1. Update `server/.env`:
   ```
   REDIS_PASSWORD=NewSecurePassword123!
   ```

2. Update `appsettings.json`:
   ```json
   "Redis": {
       "Password": "NewSecurePassword123!"
   }
   ```

3. Restart containers:
   ```bash
   docker-compose down
   docker-compose up -d
   ```

## Security Notes

- Never commit `.env` file with sensitive passwords to Git
- Add `.env` to `.gitignore`
- In production, use Azure Key Vault or similar service
- Use strong, randomly generated passwords
- Example strong password generation:
  ```bash
  openssl rand -base64 32
  ```

## Production Deployment

For production (e.g., Azure Container Instances):
1. Store password in Azure Key Vault
2. Reference it in docker-compose or app configuration
3. Never expose passwords in logs or configuration files
4. Use SSL/TLS for Redis connections
5. Enable Redis persistence in production
