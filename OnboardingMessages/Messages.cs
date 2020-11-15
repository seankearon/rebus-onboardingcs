using System;

namespace OnboardingMessages
{
    public record OnboardNewCustomer
    {
        public string Name  { get; init; }
        public string Email { get; init; }
    }

    public record CreateCustomerAccount
    {
        public string Name  { get; init; }
        public string Email { get; init; }
    }

    public record CustomerAccountCreated
    {
        public string Email     { get; init; }
        public int    AccountId { get; init; }
    }

    public record SendWelcomeEmail
    {
        public int AccountId { get; init; }
    }

    public record WelcomeEmailSent
    {
        public int AccountId { get; init; }
    }

    public record ScheduleSalesCall
    {
        public int AccountId { get; init; }
    }

    public record SalesCallScheduled
    {
        public int AccountId { get; init; }
    }

    public record OnboardingOlaBreached
    {
        public Guid SagaId { get; init; }
    }

    public record CancelSalesCall
    {
        public int AccountId { get; init; }
    }

    public record NotifyServiceDesk
    {
        public string Message { get; init; }
    }
}