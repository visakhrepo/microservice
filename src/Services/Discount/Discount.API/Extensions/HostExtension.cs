using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

namespace Discount.API.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDatabase<T>(this IHost host, int? retry = 0)
        {
            int retrForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<T>>();

                try
                {
                    logger.LogInformation("Migrating postgres database.");
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(
		                                    ID SERIAL PRIMARY KEY         NOT NULL,
		                                    ProductName     VARCHAR(24) NOT NULL,
		                                    Description     TEXT,
		                                    Amount          INT
	                                        );";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "Exception on migration");

                    if(retrForAvailability < 50)
                    {
                        retrForAvailability++;
                        System.Threading.Thread.Sleep(1000);
                        MigrateDatabase<Program>(host, retrForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
