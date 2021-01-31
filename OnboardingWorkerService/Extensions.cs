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
                   .Logging(l   => l.Serilog())
                   .Routing(r   => r.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue"))
                   .Transport(t => t.UseFileSystem("c:/rebus-advent", "MainQueue"))
                   .Options(t   => t.SimpleRetryStrategy(errorQueueAddress: "ErrorQueue"))
                   .Options(t   => t.EnableMessageAuditing(auditQueue: "AuditQueue"))
                   .Sagas(s     => s.UseFilesystem("c:/rebus-advent/sagas"))
            );

            services.AutoRegisterHandlersFromAssemblyOf<RebusHostedService>();
        }
    }
}