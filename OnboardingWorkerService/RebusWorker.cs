using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Bus;
using Rebus.ServiceProvider;

namespace OnboardingWorkerService
{
    /// <summary>
    ///  Reference:
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio
    /// </summary>
    public class RebusHostedService : IHostedService
    {
        private readonly IConfigurationRoot _configuration;
        private          IBus               _bus;
        private          ServiceProvider    _provider;

        public RebusHostedService(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var services = new ServiceCollection();
            services.AddRebusAsSendAndReceive(_configuration);

            _provider = services.BuildServiceProvider();
            _provider.UseRebus(x => _bus = x);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _provider.DisposeAsync();
        }
    }
}