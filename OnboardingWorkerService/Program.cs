using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace OnboardingWorkerService
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            ReadConfiguration();

            CreateHostBuilder(args).Build().Run();
        }

        private static void ReadConfiguration()
        {
            var profile       = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDevelopment = profile == "Development";

            // https://andrewlock.net/sharing-appsettings-json-configuration-files-between-projects-in-asp-net-core/
            var builder =
                new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json", optional: false)
                   .AddEnvironmentVariables();

            if (isDevelopment)
            {
                builder
                   .AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly(), optional: true);
            }

            Configuration = builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(Configuration);
                    services.AddHostedService<RebusHostedService>();
                });
    }
}
