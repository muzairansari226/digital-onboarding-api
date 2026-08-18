using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Consent
{
    public interface IConsentService
    {
        Task<ApiResponse<ConsentModel.ConsentDocumentResponse>> GetDocumentAsync(
            EnConsentDocumentType documentType);

        Task<ApiResponse<ConsentModel.ConsentAcceptResponse>> AcceptAsync(
            ConsentModel.ConsentAcceptRequest request, string? acceptedFromIp);
    }
}
