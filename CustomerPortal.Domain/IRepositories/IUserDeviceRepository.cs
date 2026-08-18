using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IUserDeviceRepository
    {
        Task<UserDevice?> GetByUserAndDeviceAsync(Guid fkUser, string deviceId);

        Task<List<UserDevice>> GetByUserAsync(Guid fkUser);

        Task<Guid> AddAsync(UserDevice userDevice);

        Task<bool> UpdateAsync(UserDevice userDevice);
    }
}
