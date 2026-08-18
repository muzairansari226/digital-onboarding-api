using CustomerPortal.Application.Contracts;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class UserDeviceRepository : IUserDeviceRepository
    {
        private readonly CustomerPortalDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserDeviceRepository(
            CustomerPortalDbContext context, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<UserDevice?> GetByUserAndDeviceAsync(Guid fkUser, string deviceId)
            => _context.UserDevices.FirstOrDefaultAsync(device =>
                device.FkUser == fkUser && device.DeviceId == deviceId && device.IsActive);

        public Task<List<UserDevice>> GetByUserAsync(Guid fkUser)
            => _context.UserDevices
                .Where(device => device.FkUser == fkUser && device.IsActive)
                .OrderByDescending(device => device.UpdatedAt)
                .ToListAsync();

        public async Task<Guid> AddAsync(UserDevice userDevice)
        {
            userDevice.RecId = Guid.NewGuid();
            userDevice.CreatedAt = userDevice.UpdatedAt = _dateTimeProvider.UtcNow;
            userDevice.IsActive = true;

            _context.UserDevices.Add(userDevice);
            await _context.SaveChangesAsync();

            return userDevice.RecId;
        }

        public async Task<bool> UpdateAsync(UserDevice userDevice)
        {
            userDevice.UpdatedAt = _dateTimeProvider.UtcNow;
            _context.UserDevices.Update(userDevice);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
