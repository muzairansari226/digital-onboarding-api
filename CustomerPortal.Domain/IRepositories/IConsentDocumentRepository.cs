using CustomerPortal.Domain.Entities;

namespace CustomerPortal.Domain.IRepositories
{
    public interface IConsentDocumentRepository
    {
        Task<ConsentDocument?> GetActiveByTypeAsync(int documentType);

        Task<ConsentDocument?> GetByIdAsync(Guid recId);
    }
}
