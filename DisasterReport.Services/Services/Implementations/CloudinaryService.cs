using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DisasterReport.Services.Config;
using DisasterReport.Services.Models;
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

        // For disaster report 
        public async Task<List<ImpactUrlDto>> UploadFilesAsync(List<IFormFile> files)
        {
            var uploadResults = new List<ImpactUrlDto>();

            foreach (var file in files)
            {
                if (file.Length <= 0)
                    throw new Exception("File is empty");

                var fileType = file.ContentType.Split('/')[0];

                await using var stream = file.OpenReadStream();

                CloudUploadResult uploadResult;

                if (fileType == "image")
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "disaster_reports"
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                else if (fileType == "video")
                {
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "disaster_reports"
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                else
                {
                    throw new Exception("Unsupported file type.");
                }

                if (uploadResult.Error != null)
                    throw new Exception(uploadResult.Error.Message);

                uploadResults.Add(new ImpactUrlDto
                {
                    ImageUrl = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    FileType = file.ContentType,
                    FileSizeKb = (int)(file.Length / 1024),
                    UploadedAt = DateTime.UtcNow
                });
            }

            return uploadResults;
        }

        public async Task DeleteFilesAsync(List<string> publicIds)
        {
            if (publicIds == null || !publicIds.Any())
            {
                return;
            }

            var deletionParams = new DelResParams()
            {
                PublicIds = publicIds.ToList()
            };

            var deletionResult = await _cloudinary.DeleteResourcesAsync(deletionParams);

            if (deletionResult.Error != null)
            {
                throw new Exception(deletionResult.Error.Message);
            }
        }
    }
}
