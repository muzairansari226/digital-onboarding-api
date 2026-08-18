using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IHomeContentCardRepository
    {
        Task<List<HomeContentCard>> GetAllActiveAsync();
    }
}
