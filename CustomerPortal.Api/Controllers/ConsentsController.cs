using Asp.Versioning;
using CustomerPortal.Application.Enums;
using CustomerPortal.Application.Helper;
using CustomerPortal.Application.Models;
using CustomerPortal.Application.Services.Consent;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPortal.Api.Controllers
{
    [ApiVersion(ApplicationConstant.ApiVersions.V1)]
    [Route(ApplicationConstant.ApiRoutes.Consents)]
    public class ConsentsController : ApiControllerBase
    {
        private readonly IConsentService _consentService;

        public ConsentsController(IConsentService consentService)
        {
            _consentService = consentService;
        }

        [HttpGet("documents/{documentType}")]
        public async Task<IActionResult> GetDocument(EnConsentDocumentType documentType)
        {
            return Result(await _consentService.GetDocumentAsync(documentType));
        }

        [HttpPost]
        public async Task<IActionResult> Accept([FromBody] ConsentModel.ConsentAcceptRequest request)
        {
            return Result(await _consentService.AcceptAsync(
                request, HttpContext.Connection.RemoteIpAddress?.ToString()));
        }
    }
}
