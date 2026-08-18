using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class HomeContentCardRepository : IHomeContentCardRepository
    {
        private readonly CustomerPortalDbContext _context;

        public HomeContentCardRepository(CustomerPortalDbContext context)
        {
            _context = context;
        }

        public Task<List<HomeContentCard>> GetAllActiveAsync()
            => _context.HomeContentCards
                .Where(card => card.IsActive)
                .OrderBy(card => card.DisplayOrder)
                .ThenBy(card => card.RecId)
                .ToListAsync();
    }
}
