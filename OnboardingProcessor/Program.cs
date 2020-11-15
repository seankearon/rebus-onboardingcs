using Microsoft.Extensions.Configuration;
using Serilog;
using Topper;

namespace OnboardingProcessor
{
    static class Program
    {
        private static IConfiguration Configuration { get; set; }

        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();

            ReadConfiguration();

            var serviceConfiguration = new ServiceConfiguration()
               .Add("OurBackendBus", () => new Backend(Configuration));

            ServiceHost.Run(serviceConfiguration);
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
    }
}