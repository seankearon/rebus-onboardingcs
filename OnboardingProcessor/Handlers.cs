using System;
using System.Threading.Tasks;
using OnboardingMessages;
using Rebus.Bus;
using Rebus.Handlers;
using Serilog;

namespace OnboardingProcessor
{
    public class CreateCustomerAccountHandler : IHandleMessages<CreateCustomerAccount>
    {
        private readonly IBus _bus;

        public CreateCustomerAccountHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(CreateCustomerAccount m)
        {
            Log.Information($"Creating customer account for {m.Name}, {m.Email}.");
            await Task.Delay(500); // Pretend we're doing something!
            await _bus.Reply(new CustomerAccountCreated {Email = m.Email, AccountId = new Random().Next()});
        }
    }

    public class SendWelcomeEmailHandler : IHandleMessages<SendWelcomeEmail>
    {
        private readonly IBus _bus;

        public SendWelcomeEmailHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(SendWelcomeEmail m)
        {
            Log.Information($"Sending welcome email for account {m.AccountId}.");
            await Task.Delay(500); // Pretend we're doing something!
            await _bus.Reply(new WelcomeEmailSent { AccountId = m.AccountId });
        }
    }

    public class ScheduleSalesCallHandler : IHandleMessages<ScheduleSalesCall>
    {
        private readonly IBus _bus;

        public ScheduleSalesCallHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(ScheduleSalesCall m)
        {
            Log.Information($"Scheduling sales call for account {m.AccountId}.");
            await Task.Delay(500); // Pretend we're doing something!
            await _bus.Reply(new SalesCallScheduled { AccountId = m.AccountId });
        }
    }
}