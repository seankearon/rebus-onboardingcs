using System;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
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
}