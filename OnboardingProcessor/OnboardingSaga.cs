using System;
using System.Threading.Tasks;
using OnboardingMessages;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using Serilog;

namespace OnboardingProcessor
{
    public class OnboardingSagaData: ISagaData
    {
        public Guid Id       { get; set; }
        public int  Revision { get; set; }

        public string CustomerName  { get; set; }
        public string CustomerEmail { get; set; }
        public int    AccountId     { get; set; }

        public bool AccountCreated     { get; set; }
        public bool WelcomeEmailSent   { get; set; }
        public bool SalesCallScheduled { get; set; }

        public bool IsComplete => AccountCreated && WelcomeEmailSent && SalesCallScheduled;
    }

    public class OnboardingSaga : Saga<OnboardingSagaData>
                                , IAmInitiatedBy<OnboardNewCustomer>
                                , IHandleMessages<CustomerAccountCreated>
                                , IHandleMessages<WelcomeEmailSent>
                                , IHandleMessages<SalesCallScheduled>
                                , IHandleMessages<OnboardingOlaBreached>
    {
        private readonly IBus _bus;

        public OnboardingSaga(IBus bus)
        {
            _bus = bus;
        }

        private void TryComplete()
        {
            if (Data.IsComplete)
            {
                Log.Information($"Onboarding completed for {Data.CustomerName}, {Data.CustomerEmail}, {Data.AccountId}.");
                MarkAsComplete();
            }
        }

        protected override void CorrelateMessages(ICorrelationConfig<OnboardingSagaData> config)
        {
            config.Correlate<OnboardNewCustomer>    (x => x.Email, nameof(OnboardingSagaData.CustomerEmail));
            config.Correlate<CustomerAccountCreated>(x => x.Email, nameof(OnboardingSagaData.CustomerEmail));
            config.Correlate<WelcomeEmailSent>      (x => x.AccountId, nameof(OnboardingSagaData.AccountId));
            config.Correlate<SalesCallScheduled>    (x => x.AccountId, nameof(OnboardingSagaData.AccountId));
            config.Correlate<OnboardingOlaBreached> (x => x.SagaId, nameof(OnboardingSagaData.Id));
        }

        public async Task Handle(OnboardNewCustomer m)
        {
            if (!IsNew) return;
            Log.Information($"Beginning onboarding process for {m.Name}, {m.Email}.");

            Data.CustomerName  = m.Name;
            Data.CustomerEmail = m.Email;

            await _bus.Send(new CreateCustomerAccount {Name = m.Name, Email = m.Email});
            await _bus.Defer(TimeSpan.FromSeconds(5), new OnboardingOlaBreached {SagaId = Data.Id});

            TryComplete();
        }

        public async Task Handle(CustomerAccountCreated m)
        {
            Log.Information($"Customer account created for {m.Email} with ID {m.AccountId}.");

            Data.AccountId = m.AccountId;
            Data.AccountCreated = true;

            await _bus.Send(new SendWelcomeEmail  {AccountId = Data.AccountId});
            await _bus.Send(new ScheduleSalesCall {AccountId = Data.AccountId});

            TryComplete();
        }

        public Task Handle(WelcomeEmailSent m)
        {
            Log.Information($"Welcome email sent for {m.AccountId}.");
            Data.WelcomeEmailSent = true;
            TryComplete();
            return Task.CompletedTask;
        }

        public Task Handle(SalesCallScheduled m)
        {
            Log.Information($"Sales call scheduled for {m.AccountId}.");
            Data.SalesCallScheduled = true;
            TryComplete();
            return Task.CompletedTask;
        }

        public async Task Handle(OnboardingOlaBreached m)
        {
            Log.Information($"ONBOARDING OLA BREACH PENDING FOR for saga {m.SagaId}.");

            if (Data.SalesCallScheduled)
            {
                await _bus.Send(new CancelSalesCall {AccountId = Data.AccountId});
            }

            await _bus.Send(new NotifyServiceDesk {Message = $"Customer onboarding OLA breach pending for new customer {Data.CustomerName} with email {Data.CustomerEmail}."});

            Log.Information($"Abandoning saga {Data.Id}.");
            MarkAsComplete();
        }
    }
}