using DisasterReport.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certService;

        public CertificateController(ICertificateService certService)
        {
            _certService = certService;
        }

        [Authorize]
        [HttpGet("download/{donationId}")]
        public async Task<IActionResult> DownloadCertificate(int donationId)
        {
            var pdfBytes = await _certService.GenerateCertificatePdfAsync(donationId);

            return File(pdfBytes, "application/pdf", $"Certificate_{donationId}.pdf");
        }
    }
}
