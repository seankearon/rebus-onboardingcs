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
        public string Email      { get; init; }
        public int    CustomerId { get; init; }
    }

    public record SendWelcomeEmail
    {
        public int CustomerId { get; init; }
    }

    public record ScheduleSalesCall
    {
        public int CustomerId { get; init; }
    }

    public record CustomerOnboarded
    {
        public int CustomerId { get; init; }
    }
}