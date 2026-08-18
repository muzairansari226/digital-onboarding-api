using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class OnboardingRequirementRepository : IOnboardingRequirementRepository
    {
        private readonly CustomerPortalDbContext _context;

        public OnboardingRequirementRepository(CustomerPortalDbContext context)
        {
            _context = context;
        }

        public Task<List<OnboardingRequirement>> GetAllActiveAsync()
            => _context.OnboardingRequirements
                .Where(requirement => requirement.IsActive)
                .OrderBy(requirement => requirement.DisplayOrder)
                .ThenBy(requirement => requirement.RecId)
                .ToListAsync();
    }
}
