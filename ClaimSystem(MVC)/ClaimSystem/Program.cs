using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ClaimSystem
{
    public class Program
    {
        // Main of application - entry point
        public static void Main(string[] args)
        {
            // Create, run the web host
            CreateHostBuilder(args).Build().Run();
        }

        // Set web host with basic settings
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();  // Selects Startup class to be used
                });
    }
}
