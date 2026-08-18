using CustomerPortal.Application.Contracts;

namespace CustomerPortal.Infrastructure.Providers.Security
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
