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
            services.AddScoped<IOrganizationMemberService, OrganizationMemberService>();
            services.AddScoped<InvitationNotificationService>();
            services.AddScoped<IDonateRequestService, DonateRequestService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<ICertificateService, CertificateService>();

            services.AddScoped<IDisasterReportService, DisasterReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBlacklistEntryService, BlacklistEntryService>();
            services.AddScoped<IDisasterTopicService, DisasterTopicService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ISupportTypeService, SupportTypeService>();
            services.AddScoped<IImpactTypeService, ImpactTypeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<INasaService, NasaService>();
            services.AddScoped<IDisasterEventNasaService, DisasterEventNasaService>();
            services.AddHttpClient<INasaService, NasaService>();
            services.AddScoped<IEmailServices, EmailService>(); 
            services.AddScoped<IDisasterNotificationService, DisasterNotificationService>();
            services.AddScoped<IPeopleVoiceService, PeopleVoiceService>();

            //Add Cloudinary    
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            return services;
        }
    }
}
