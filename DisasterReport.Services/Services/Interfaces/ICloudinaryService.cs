using DisasterReport.Services.Models; //Use this for custom UploadResult
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services
{
    public interface ICloudinaryService
    {
        Task<UploadResult> UploadFileAsync(IFormFile file); //Custom Model's UploadResult
        //For disaster report
        Task<List<ImpactUrlDto>> UploadFilesAsync(List<IFormFile> files);
        Task DeleteFilesAsync(List<string> publicIds);
    }
}
