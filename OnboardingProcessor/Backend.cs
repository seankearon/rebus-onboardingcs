using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OnboardingMessages;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.ServiceProvider;

namespace OnboardingProcessor
{
    public class Backend : IDisposable
    {
        private readonly ServiceProvider _provider;
        private          IBus            _bus;

        public Backend()
        {
            var services = new ServiceCollection();
            services.AddRebusAsSendAndReceive();
            _provider = services.BuildServiceProvider();
            _provider.UseRebus(x => _bus = x);
        }

        public void Dispose()
        {
            _bus?.Dispose();
            _provider?.Dispose();
        }
    }

    public class DummyHandler : IHandleMessages<OnboardNewCustomer>
    {
        public Task Handle(OnboardNewCustomer message) => throw new NotImplementedException();
    }
}