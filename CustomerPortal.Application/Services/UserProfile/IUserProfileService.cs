using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.UserProfile
{
    public interface IUserProfileService
    {
        Task<ApiResponse<UserModel.UserProfileResponse>> GetByIdAsync(Guid userId);

        Task<ApiResponse<UserModel.UserProfileResponse>> UpdateAsync(
            Guid userId, UserModel.UserUpdateRequest request);
    }
}
