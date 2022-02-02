using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMS.Data;

namespace TMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(config)
               .Enrich.FromLogContext()
               //.WriteTo.Http("http://localhost:8080")
               .CreateLogger();

            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                Log.Information("Application Started.");
                var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Log.Information($"Application running on environment {environmentVariable}");

                Log.Information($"Application version {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}");
                var environment = services.GetRequiredService<IWebHostEnvironment>();
                if (environment.IsDevelopment() || environment.IsEnvironment("Release"))
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                    Log.Information($"Database run migration");
                }
            }
            catch (Exception ex )
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
