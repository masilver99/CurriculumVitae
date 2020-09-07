using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace CV
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var logDir = Environment.GetEnvironmentVariable("LOG_DIR") ?? "logs";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDir, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

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
