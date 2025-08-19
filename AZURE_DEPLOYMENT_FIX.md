# Azure Deployment Fix Guide for TaskFlow API

## Problem Summary
Your TaskFlow API is getting a 500 Internal Server Error when deployed to Azure, specifically when trying to register users. The error suggests transient database connection failures and Azure Storage configuration issues.

## Root Causes Identified

1. **Missing Azure Storage Connection String**: The deployed app is trying to use `UseDevelopmentStorage=true` which only works locally
2. **Database Connection Issues**: While retry logic exists, the connection string might not be properly configured
3. **Queue Service Failures**: The queue service fails to initialize, causing the registration flow to break

## Solutions Implemented

### 1. Enhanced Error Handling and Logging
- Added comprehensive logging for database and Azure Storage connection status
- Improved error handling in QueueService to gracefully handle connection failures
- Added service availability checks before attempting operations

### 2. Resilient Queue Service
- QueueService now handles connection failures gracefully
- Added `IsAvailable()` method to check service status
- Service continues to work even if Azure Storage is temporarily unavailable

### 3. Better Configuration Management
- Created `appsettings.Production.json` for Azure-specific settings
- Enhanced environment variable handling
- Added validation for required connection strings

## Required Azure Configuration

### 1. Environment Variables (Set in Azure App Service Configuration)
```
DATABASE_CONNECTION_STRING=Server=tcp:your-server.database.windows.net,1433;Initial Catalog=your-database;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

AZURE_STORAGE_CONNECTION_STRING=DefaultEndpointsProtocol=https;AccountName=your-storage-account;AccountKey=your-storage-key;EndpointSuffix=core.windows.net

JWT_KEY=your-secure-jwt-secret-key
JWT_ISSUER=taskflow
JWT_AUDIENCE=taskflow
```

### 2. Azure Resources Required
- **Azure SQL Database**: For user data storage
- **Azure Storage Account**: For queue message storage
- **Azure Function App**: For processing user registration messages

## Steps to Fix Your Azure Deployment

### Step 1: Configure Azure App Service Environment Variables
1. Go to your Azure App Service in the Azure Portal
2. Navigate to **Configuration** → **Application settings**
3. Add the following environment variables:
   - `DATABASE_CONNECTION_STRING` (your actual Azure SQL connection string)
   - `AZURE_STORAGE_CONNECTION_STRING` (your actual Azure Storage connection string)
   - `JWT_KEY` (a secure random string)
   - `JWT_ISSUER` and `JWT_AUDIENCE` (can be "taskflow")

### Step 2: Verify Azure SQL Database
1. Ensure your Azure SQL Database is running and accessible
2. Check firewall rules allow connections from Azure App Service
3. Verify the connection string format matches Azure SQL requirements

### Step 3: Configure Azure Storage Account
1. Create or verify your Azure Storage Account
2. Ensure the storage account has queues enabled
3. Get the connection string from **Access keys** section

### Step 4: Deploy Updated Code
1. The updated code now handles connection failures gracefully
2. Deploy the updated `Program.cs`, `QueueService.cs`, and `AuthController.cs`
3. The app will now provide better error messages and handle service unavailability

## Testing the Fix

### 1. Check Health Endpoint
After deployment, visit your root endpoint:
```
GET https://your-app.azurewebsites.net/
```

This should now show:
```json
{
  "status": "OK",
  "message": "TaskFlow API is running 🚀",
  "databaseConfigured": true,
  "azureStorageConfigured": true
}
```

### 2. Test Database Connection
Use the database test endpoint:
```
GET https://your-app.azurewebsites.net/api/auth/db-test
```

### 3. Test Registration
Try the registration endpoint again:
```
POST https://your-app.azurewebsites.net/api/auth/register
```

## Monitoring and Troubleshooting

### 1. Check Azure App Service Logs
- Go to **Log stream** in your App Service
- Look for connection string validation messages
- Monitor for any remaining errors

### 2. Common Issues and Solutions

**Issue**: "Queue service is not available"
**Solution**: Check `AZURE_STORAGE_CONNECTION_STRING` environment variable

**Issue**: "Database connection failed"
**Solution**: Verify `DATABASE_CONNECTION_STRING` and Azure SQL firewall rules

**Issue**: JWT token generation fails
**Solution**: Ensure `JWT_KEY` environment variable is set

## Next Steps

1. **Deploy the updated code** to your Azure App Service
2. **Configure the environment variables** as specified above
3. **Test the endpoints** to verify the fix
4. **Monitor the logs** for any remaining issues

## Additional Recommendations

1. **Use Azure Key Vault** for storing sensitive connection strings
2. **Implement Application Insights** for better monitoring
3. **Set up proper logging** to Azure Storage or Application Insights
4. **Consider using Managed Identity** for Azure SQL and Storage access

## Support

If you continue to experience issues after implementing these fixes:
1. Check the Azure App Service logs for specific error messages
2. Verify all environment variables are correctly set
3. Test database connectivity from the App Service
4. Ensure Azure resources are in the same region for better performance
