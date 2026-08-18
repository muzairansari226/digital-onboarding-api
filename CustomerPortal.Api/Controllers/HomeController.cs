using Asp.Versioning;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Services.Home;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Home)]
    public class HomeController : ApiControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            return Result(await _homeService.GetAsync(userId));
        }
    }
}
