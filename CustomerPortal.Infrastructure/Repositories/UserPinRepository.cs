using CustomerPortal.Application.Contracts;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class UserPinRepository : IUserPinRepository
    {
        private readonly CustomerPortalDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserPinRepository(CustomerPortalDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<UserPin?> GetByUserAsync(Guid fkUser)
            => _context.UserPins.FirstOrDefaultAsync(
                userPin => userPin.FkUser == fkUser && userPin.IsActive);

        public async Task<Guid> AddAsync(UserPin userPin)
        {
            userPin.RecId = Guid.NewGuid();
            userPin.CreatedAt = userPin.UpdatedAt = _dateTimeProvider.UtcNow;
            userPin.IsActive = true;

            _context.UserPins.Add(userPin);
            await _context.SaveChangesAsync();

            return userPin.RecId;
        }

        public async Task<bool> UpdateAsync(UserPin userPin)
        {
            userPin.UpdatedAt = _dateTimeProvider.UtcNow;
            _context.UserPins.Update(userPin);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
