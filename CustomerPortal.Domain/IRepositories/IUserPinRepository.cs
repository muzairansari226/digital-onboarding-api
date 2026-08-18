using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IUserPinRepository
    {
        Task<UserPin?> GetByUserAsync(Guid fkUser);

        Task<Guid> AddAsync(UserPin userPin);

        Task<bool> UpdateAsync(UserPin userPin);
    }
}
