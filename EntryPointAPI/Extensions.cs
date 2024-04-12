using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace EntryPointAPI
{
    public static class Extensions
    {
        public static void AddRebusAsOneWayClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddRebus(
                rebus => rebus
                   .Logging(l => l.Console())
                   .Routing(r => r.TypeBased().Map<OnboardNewCustomer>("MainQueue"))
                   .Transport(t => t.UseAzureServiceBusAsOneWayClient(config.GetConnectionString("AzureServiceBusConnectionString")))
                   .Options(t => t.RetryStrategy(errorQueueName: "ErrorQueue")));
        }
    }
}