using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.Consent
{
    public class ConsentService : IConsentService
    {
        private readonly IConsentDocumentRepository _consentDocumentRepository;
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ConsentService(
            IConsentDocumentRepository consentDocumentRepository,
            IUserConsentRepository userConsentRepository,
            IUserRepository userRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _consentDocumentRepository = consentDocumentRepository;
            _userConsentRepository = userConsentRepository;
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ApiResponse<ConsentModel.ConsentDocumentResponse>> GetDocumentAsync(
            EnConsentDocumentType documentType)
        {
            var document = await _consentDocumentRepository.GetActiveByTypeAsync((int)documentType);
            if (document is null)
            {
                return ApiResponse<ConsentModel.ConsentDocumentResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.ConsentDocumentNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            return ApiResponse<ConsentModel.ConsentDocumentResponse>.SuccessResponse(
                MapToDocumentResponse(document),
                ApplicationConstant.ResponseMessages.ConsentDocumentRetrieved);
        }

        public async Task<ApiResponse<ConsentModel.ConsentAcceptResponse>> AcceptAsync(
            ConsentModel.ConsentAcceptRequest request, string? acceptedFromIp)
        {
            var user = await _userRepository.GetByIdAsync(request.RegistrationId);
            if (user is null)
            {
                return ApiResponse<ConsentModel.ConsentAcceptResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            if (!user.IsMobileVerified)
            {
                return ApiResponse<ConsentModel.ConsentAcceptResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.MobileVerificationRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            if (!user.IsEmailVerified)
            {
                return ApiResponse<ConsentModel.ConsentAcceptResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.EmailVerificationRequired,
                    ApplicationConstant.StatusCodes.BadRequest);
            }

            var document = await _consentDocumentRepository.GetByIdAsync(request.DocumentId);
            if (document is null)
            {
                return ApiResponse<ConsentModel.ConsentAcceptResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.ConsentDocumentNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            if (await _userConsentRepository.HasAcceptedAsync(user.RecId, document.DocumentType))
            {
                return ApiResponse<ConsentModel.ConsentAcceptResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.ConsentAlreadyRecorded,
                    ApplicationConstant.StatusCodes.Conflict);
            }

            var acceptedAt = _dateTimeProvider.UtcNow;

            var consentId = await _userConsentRepository.AddAsync(new UserConsent
            {
                FkUser = user.RecId,
                FkConsentDocument = document.RecId,
                DocumentType = document.DocumentType,
                DocumentVersion = document.Version,
                AcceptedAt = acceptedAt,
                AcceptedFromIp = acceptedFromIp
            });

            return ApiResponse<ConsentModel.ConsentAcceptResponse>.SuccessResponse(
                new ConsentModel.ConsentAcceptResponse
                {
                    ConsentId = consentId,
                    DocumentVersion = document.Version,
                    AcceptedAt = acceptedAt,
                    NextStep = OnboardingStepHelper.Resolve(
                        user.IsMobileVerified,
                        user.IsEmailVerified,
                        isConsentAccepted: true,
                        isPinSet: false,
                        isBiometricEnrolled: false).ToString()
                },
                ApplicationConstant.ResponseMessages.ConsentRecorded,
                ApplicationConstant.StatusCodes.Created);
        }

        private static ConsentModel.ConsentDocumentResponse MapToDocumentResponse(
            ConsentDocument document) => new()
            {
                DocumentId = document.RecId,
                DocumentType = ((EnConsentDocumentType)document.DocumentType).ToString(),
                Version = document.Version,
                Title = document.Title,
                Content = document.Content,
                EffectiveFrom = document.EffectiveFrom
            };
    }
}
