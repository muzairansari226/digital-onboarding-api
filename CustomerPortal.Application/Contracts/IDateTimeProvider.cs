namespace CustomerPortal.Application.Contracts
{
    //Supplies the current time. Injected rather than called statically so that expiry, cooldown
    //and lockout rules can be tested without waiting for the clock.

    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
