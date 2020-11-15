using System;
using System.Threading.Tasks;
using OnboardingMessages;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;

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
                MarkAsComplete();
            }
        }

        protected override void CorrelateMessages(ICorrelationConfig<OnboardingSagaData> config)
        {
            config.Correlate<OnboardNewCustomer>    (x => x.Email, nameof(OnboardingSagaData.CustomerEmail));
            config.Correlate<CustomerAccountCreated>(x => x.Email, nameof(OnboardingSagaData.CustomerEmail));
            config.Correlate<WelcomeEmailSent>      (x => x.AccountId, nameof(OnboardingSagaData.AccountId));
            config.Correlate<SalesCallScheduled>    (x => x.AccountId, nameof(OnboardingSagaData.AccountId));
        }

        public async Task Handle(OnboardNewCustomer m)
        {
            if (!IsNew) return;

            Data.CustomerName  = m.Name;
            Data.CustomerEmail = m.Email;

            await _bus.Send(new CreateCustomerAccount {Name = m.Name, Email = m.Email});

            TryComplete();
        }

        public async Task Handle(CustomerAccountCreated m)
        {
            Data.AccountId = m.AccountId;
            Data.AccountCreated = true;

            await _bus.Send(new SendWelcomeEmail  {AccountId = Data.AccountId});
            await _bus.Send(new ScheduleSalesCall {AccountId = Data.AccountId});

            TryComplete();
        }

        public Task Handle(WelcomeEmailSent _)
        {
            Data.WelcomeEmailSent = true;
            TryComplete();
            return Task.CompletedTask;
        }

        public Task Handle(SalesCallScheduled _)
        {
            Data.SalesCallScheduled = true;
            TryComplete();
            return Task.CompletedTask;
        }
    }
}