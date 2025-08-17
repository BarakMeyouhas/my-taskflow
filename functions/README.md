# TaskFlow Azure Functions

This Azure Functions app handles background processing for the TaskFlow application using Azure Storage Queues.

## Architecture

The application uses a queue-based architecture where:

1. **Backend API** receives user registration requests
2. **Queue Service** sends messages to Azure Storage Queue
3. **Azure Functions** process the queue messages asynchronously
4. **Database operations** are performed by the functions

## Setup

### Prerequisites

- .NET 8.0 SDK
- Azure Storage Emulator or Azure Storage Account
- SQL Server (local or Azure)

### Local Development

1. **Install Azure Storage Emulator** (for local development):
   ```bash
   # Using Azure Storage Emulator
   # Or use Azurite (recommended for cross-platform)
   npm install -g azurite
   ```

2. **Start Azure Storage Emulator**:
   ```bash
   azurite --silent
   ```

3. **Update Connection Strings**:
   - In `local.settings.json`: Set `AzureWebJobsStorage` to `UseDevelopmentStorage=true`
   - In `appsettings.Development.json`: Set `AzureStorage` to `UseDevelopmentStorage=true`

4. **Run the Functions App**:
   ```bash
   cd functions
   func start
   ```

### Azure Deployment

1. **Update Connection Strings**:
   - Set `AzureWebJobsStorage` to your Azure Storage connection string
   - Set `DefaultConnection` to your Azure SQL connection string

2. **Deploy to Azure**:
   ```bash
   func azure functionapp publish <your-function-app-name>
   ```

## Functions

### UserRegistrationFunction

- **Trigger**: Queue trigger on `user-registration-queue`
- **Purpose**: Processes user registration requests asynchronously
- **Operations**:
  - Validates user data
  - Checks for existing users
  - Hashes passwords using BCrypt
  - Saves user to database

### QueueTrigger1

- **Trigger**: General queue trigger (can be used for other purposes)
- **Purpose**: General message processing

## Message Format

User registration messages use the following JSON format:

```json
{
  "Username": "string",
  "Email": "string",
  "Password": "string",
  "RequestedAt": "datetime",
  "RequestId": "guid"
}
```

## Configuration

### Environment Variables

- `AzureWebJobsStorage`: Azure Storage connection string
- `DefaultConnection`: SQL Server connection string

### Queue Names

- `user-registration-queue`: For user registration processing

## Benefits

1. **Asynchronous Processing**: User registration doesn't block the API
2. **Scalability**: Functions automatically scale based on queue depth
3. **Reliability**: Built-in retry mechanisms and error handling
4. **Separation of Concerns**: API handles requests, functions handle processing

## Monitoring

- Use Azure Application Insights for production monitoring
- Check Azure Functions logs for processing status
- Monitor queue depth and processing times

## Troubleshooting

1. **Queue Messages Not Processing**:
   - Check connection strings
   - Verify queue exists
   - Check function logs

2. **Database Connection Issues**:
   - Verify connection string
   - Check firewall rules (Azure SQL)
   - Ensure database exists

3. **Local Development Issues**:
   - Start Azure Storage Emulator
   - Check `local.settings.json` configuration
   - Verify .NET version compatibility
