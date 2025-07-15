using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DisasterReport.Services.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

// Aliases to avoid confusion between Cloudinary's UploadResult and custom own model
using CloudUploadResult = CloudinaryDotNet.Actions.UploadResult;
using DisasterUploadResult = DisasterReport.Services.Models.UploadResult;

namespace DisasterReport.Services.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<DisasterUploadResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or missing.");
            }

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            CloudUploadResult uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Upload failed: " + uploadResult.Error?.Message);
            }

            // Map from Cloudinary's UploadResult to custom own model
            return new DisasterUploadResult
            {
                SecureUrl = uploadResult.SecureUrl.ToString(),
                FileName = file.FileName,
                FileType = file.ContentType
            };
        }
    }
}
