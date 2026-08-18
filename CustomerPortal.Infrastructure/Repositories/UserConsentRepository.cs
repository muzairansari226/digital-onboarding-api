using CustomerPortal.Application.Contracts;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class UserConsentRepository : IUserConsentRepository
    {
        private readonly CustomerPortalDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserConsentRepository(
            CustomerPortalDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Guid> AddAsync(UserConsent userConsent)
        {
            userConsent.RecId = Guid.NewGuid();
            userConsent.CreatedAt = _dateTimeProvider.UtcNow;

            _context.UserConsents.Add(userConsent);
            await _context.SaveChangesAsync();

            return userConsent.RecId;
        }

        public Task<bool> HasAcceptedAsync(Guid fkUser, int documentType)
            => _context.UserConsents.AnyAsync(consent =>
                consent.FkUser == fkUser && consent.DocumentType == documentType);
    }
}
