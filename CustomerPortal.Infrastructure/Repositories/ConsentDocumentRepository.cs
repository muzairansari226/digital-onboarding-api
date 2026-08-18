using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;
using CustomerPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerPortal.Infrastructure.Repositories
{
    public class ConsentDocumentRepository : IConsentDocumentRepository
    {
        private readonly CustomerPortalDbContext _context;

        public ConsentDocumentRepository(CustomerPortalDbContext context)
        {
            _context = context;
        }

        public Task<ConsentDocument?> GetActiveByTypeAsync(int documentType)
            => _context.ConsentDocuments
                .Where(document => document.DocumentType == documentType && document.IsActive)
                .OrderByDescending(document => document.EffectiveFrom)
                .FirstOrDefaultAsync();

        public Task<ConsentDocument?> GetByIdAsync(Guid recId)
            => _context.ConsentDocuments.FirstOrDefaultAsync(
                document => document.RecId == recId && document.IsActive);
    }
}
