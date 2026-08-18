using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IOnboardingRequirementRepository
    {
        Task<List<OnboardingRequirement>> GetAllActiveAsync();
    }
}
