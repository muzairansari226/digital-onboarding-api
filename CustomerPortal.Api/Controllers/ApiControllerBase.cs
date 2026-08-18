using CustomerPortal.Application.Helper;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Returns the envelope under the status code the service already chose, so a 404 stays a
        /// 404 rather than being flattened into a 200 with an error inside.
        /// </summary>
        protected IActionResult Result<T>(ApiResponse<T> response)
            => StatusCode(response.StatusCode, response);
    }
}
