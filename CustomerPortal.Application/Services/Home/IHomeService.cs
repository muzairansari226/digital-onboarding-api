using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;

namespace CustomerPortal.Application.Services.Home
{
    public interface IHomeService
    {
        Task<ApiResponse<HomeModel.HomeResponse>> GetAsync(Guid userId);
    }
}
