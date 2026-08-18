using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IUserConsentRepository
    {
        Task<Guid> AddAsync(UserConsent userConsent);

        Task<bool> HasAcceptedAsync(Guid fkUser, int documentType);
    }
}
