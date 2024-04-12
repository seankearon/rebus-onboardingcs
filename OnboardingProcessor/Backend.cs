using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;

namespace OnboardingProcessor
{
    public class Backend : IDisposable
    {
        private readonly ServiceProvider _provider;
        private readonly IBus            _bus;

        public Backend(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddRebusAsSendAndReceive(configuration);
            _provider = services.BuildServiceProvider();
            _bus      = _provider.GetRequiredService<IBus>();
        }

        public void Dispose()
        {
            _bus?.Dispose();
            _provider?.Dispose();
        }
    }
}