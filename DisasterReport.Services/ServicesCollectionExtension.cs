using DisasterReport.Services.Config;
using DisasterReport.Services.Services;
using Microsoft.Extensions.Configuration;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DisasterReport.Services
{
    public static class ServicesCollectionExtension
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOAuthService, OAuthService>();
            services.AddScoped<IAuthAccountService, AuthAccountService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.AddScoped<IOrganizationService, OrganizationService>();

            services.AddScoped<IDisasterReportService, DisasterReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBlacklistEntryService, BlacklistEntryService>();
            services.AddScoped<IDisasterTopicService, DisasterTopicService>();
            services.AddScoped<ISupportTypeService, SupportTypeService>();
            services.AddScoped<IImpactTypeService, ImpactTypeService>();

            //Add Cloudinary
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            return services;
        }
    }
}
