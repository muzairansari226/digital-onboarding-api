using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Biometric;
using CustomerPortal.Application.Services.UserProfile;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Users)]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IBiometricService _biometricService;

        public UsersController(
            IUserProfileService userProfileService, IBiometricService biometricService)
        {
            _userProfileService = userProfileService;
            _biometricService = biometricService;
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            return Result(await _userProfileService.GetByIdAsync(userId));
        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> Update(
            Guid userId, [FromBody] UserModel.UserUpdateRequest request)
        {
            return Result(await _userProfileService.UpdateAsync(userId, request));
        }

        [HttpGet("{userId:guid}/devices")]
        public async Task<IActionResult> GetDevices(Guid userId)
        {
            return Result(await _biometricService.GetDevicesAsync(userId));
        }
    }
}
