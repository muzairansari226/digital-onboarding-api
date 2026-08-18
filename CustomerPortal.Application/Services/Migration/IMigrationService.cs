using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Migration
{
    public interface IMigrationService
    {
        Task<ApiResponse<MigrationModel.MigrationStartResponse>> StartAsync(
            MigrationModel.MigrationStartRequest request);
    }
}
