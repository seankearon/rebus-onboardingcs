using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Auditing.Messages;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;

namespace OnboardingProcessor
{
    public static class Extensions
    {
        public static void AddRebusAsSendAndReceive(this IServiceCollection services, IConfiguration config, Func<IBus,Task> rebusCreated)
        {
            services.AddRebus(
                rebus => rebus
                   .Logging  (l => l.Serilog())
                   .Routing  (r => r.TypeBased().MapAssemblyOf<OnboardNewCustomer>("MainQueue"))
                   .Transport(t => t.UseAzureServiceBus(config.GetConnectionString("AzureServiceBusConnectionString"), "MainQueue").AutomaticallyRenewPeekLock())
                   .Options  (t => t.RetryStrategy(errorQueueName: "ErrorQueue"))
                   .Options  (t => t.EnableMessageAuditing(auditQueue: "AuditQueue"))
                   .Sagas    (s => s.StoreInSqlServer(config.GetConnectionString("MsSqlConnectionString"), "Sagas", "SagaIndexes")),
                onCreated: rebusCreated
                );

            services.AutoRegisterHandlersFromAssemblyOf<Backend>();
        }
    }
}