using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TodoApi.Data;

namespace ApiTestsWithWebApplicationFactory;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            /* TodoApi's database context is registered in Program.cs. The test app's builder.ConfigureServices callback
               is executed after the app's Program.cs code is executed. To use a different database for the tests than 
               the app's database, the app's database context must be replaced in builder.ConfigureServices. */
            
            // Find the app's database context registration.
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == 
                     typeof(IDbContextOptionsConfiguration<TodoContext>));
            
            // Remove the app's database context registration.
            services.Remove(dbContextDescriptor);
            
            // Find the app's database connection registration.
            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));
            // Remove the app's database connection registration.
            services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            // Register a test database context using the open SqliteConnection.
            services.AddDbContext<TodoContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        // Set the environment to Development.
        builder.UseEnvironment("Development");
    }
}