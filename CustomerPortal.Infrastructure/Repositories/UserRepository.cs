using CustomerPortal.Application.Contracts;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomerPortalDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserRepository(CustomerPortalDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<User?> GetByIdAsync(Guid recId)
            => _context.Users.FirstOrDefaultAsync(user => user.RecId == recId && user.IsActive);

        public Task<User?> GetByEmailAsync(string email)
            => _context.Users.FirstOrDefaultAsync(user => user.Email == email && user.IsActive);

        public Task<User?> GetByMobileAsync(string mobile)
            => _context.Users.FirstOrDefaultAsync(user => user.Mobile == mobile && user.IsActive);

        public Task<bool> IsEmailTakenAsync(string email)
            => _context.Users.AnyAsync(user => user.Email == email && user.IsActive);

        public Task<bool> IsMobileTakenAsync(string mobile)
            => _context.Users.AnyAsync(user => user.Mobile == mobile && user.IsActive);

        public async Task<Guid> AddAsync(User user)
        {
            user.RecId = Guid.NewGuid();
            user.CreatedAt = user.UpdatedAt = _dateTimeProvider.UtcNow;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.RecId;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            user.UpdatedAt = _dateTimeProvider.UtcNow;
            _context.Users.Update(user);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
