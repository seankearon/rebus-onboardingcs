using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.ServiceProvider;

namespace OnboardingProcessor
{
    public class Backend : IDisposable
    {
        private readonly ServiceProvider _provider;
        private          IBus            _bus;

        public Backend(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddRebusAsSendAndReceive(configuration, bus => Task.FromResult(_bus = bus));
            _provider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            _bus?.Dispose();
            _provider?.Dispose();
        }
    }
}