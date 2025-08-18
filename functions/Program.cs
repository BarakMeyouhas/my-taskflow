using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskFlow.Functions.Data;

//test!
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Add database context
        services.AddDbContext<FunctionsDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback for local development
                connectionString =
                    "Server=(localdb)\\mssqllocaldb;Database=TaskFlowFunctions;Trusted_Connection=true;MultipleActiveResultSets=true";
            }

            options.UseSqlServer(connectionString);
        });

        // Add logging
        services.AddLogging();
    })
    .Build();

host.Run();
