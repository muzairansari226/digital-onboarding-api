using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid recId);

        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByMobileAsync(string mobile);

        Task<bool> IsEmailTakenAsync(string email);

        Task<bool> IsMobileTakenAsync(string mobile);

        Task<Guid> AddAsync(User user);

        Task<bool> UpdateAsync(User user);
    }
}
