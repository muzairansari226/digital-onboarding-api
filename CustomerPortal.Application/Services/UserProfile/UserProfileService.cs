using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Domain.Entities;
using CustomerPortal.Domain.IRepositories;

namespace CustomerPortal.Application.Services.UserProfile
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;

        public UserProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserModel.UserProfileResponse>> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return ApiResponse<UserModel.UserProfileResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            return ApiResponse<UserModel.UserProfileResponse>.SuccessResponse(
                MapToProfileResponse(user),
                ApplicationConstant.ResponseMessages.ProfileRetrieved);
        }

        public async Task<ApiResponse<UserModel.UserProfileResponse>> UpdateAsync(
            Guid userId, UserModel.UserUpdateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return ApiResponse<UserModel.UserProfileResponse>.ErrorResponse(
                    ApplicationConstant.ResponseMessages.UserNotFound,
                    ApplicationConstant.StatusCodes.NotFound);
            }

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            await _userRepository.UpdateAsync(user);

            return ApiResponse<UserModel.UserProfileResponse>.SuccessResponse(
                MapToProfileResponse(user),
                ApplicationConstant.ResponseMessages.ProfileUpdated);
        }

        private static UserModel.UserProfileResponse MapToProfileResponse(User user) => new()
        {
            RecId = user.RecId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MaskedEmail = MaskingHelper.MaskEmail(user.Email),
            MaskedMobile = MaskingHelper.MaskMobile(user.Mobile),
            IsEmailVerified = user.IsEmailVerified,
            IsMobileVerified = user.IsMobileVerified,
            IsMigrated = user.IsMigrated,
            Status = ((EnUserStatus)user.Status).ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
