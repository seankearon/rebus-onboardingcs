using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.FileSystem;

namespace EntryPointAPI
{
    public static class Extensions
    {
        public static void AddRebusAsOneWayClient(this IServiceCollection services)
        {
            services.AddRebus(
                cfg => cfg
                   .Logging(l => l.Console())
                   .Routing(r => r.TypeBased().Map<OnboardNewCustomer>("MainQueue"))
                   .Transport(t => t.UseFileSystemAsOneWayClient("c:/rebus-advent"))
                   .Options(t => t.SimpleRetryStrategy(errorQueueAddress: "ErrorQueue")));
        }
    }
}