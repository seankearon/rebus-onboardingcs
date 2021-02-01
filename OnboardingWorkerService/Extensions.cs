using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Auditing.Messages;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.FileSystem;

namespace OnboardingWorkerService
{
    public static class Extensions
    {
        public static void AddRebusAsSendAndReceive(this IServiceCollection services, IConfiguration config)
        {
            services.AddRebus(
                rebus => rebus
                   .Logging(x   => x.Serilog())
                   .Routing(x   => x.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue"))
                   .Transport(x => x.UseFileSystem("c:/rebus-advent", "MainQueue"))
                   .Options(x   => x.SimpleRetryStrategy(errorQueueAddress: "ErrorQueue"))
                   .Options(x   => x.EnableMessageAuditing(auditQueue: "AuditQueue"))
                   .Sagas(x     => x.UseFilesystem("c:/rebus-advent/sagas"))
                   .Timeouts(x  => x.UseFileSystem("c:/rebus-advent/timeouts"))
            );

            services.AutoRegisterHandlersFromAssemblyOf<RebusHostedService>();
        }
    }
}