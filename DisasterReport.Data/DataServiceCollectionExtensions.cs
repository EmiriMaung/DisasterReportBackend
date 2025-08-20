using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Implementations;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data
{
    public static class DataServiceCollectionExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(connectionString));

            //register repositories if needed 
            services.AddScoped<IOrganizationRepo, OrganizationRepo>();
            services.AddScoped<IOrganizationDocRepo, OrganizationDocRepo>();
            services.AddScoped<IOrganizationMemberRepo, OrganizationMemberRepo>();
            services.AddScoped<IDonationRepo, DonationRepo>();
            services.AddScoped<IDonateRequestRepo, DonateRequestRepo>();
            services.AddScoped<IPostRepo, PostRepo>();
            services.AddScoped<ILocationRepo, LocationRepo>();
            services.AddScoped<IImpactUrlRepo, ImpactUrlRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IBlacklistEntryRepo, BlacklistEntryRepo>();
            services.AddScoped<ISupportTypeRepo, SupportTypeRepo>();
            services.AddScoped<IImpactTypeRepo, ImpactTypeRepo>();
            services.AddScoped<IReportRepo, ReportRepo>();
            services.AddScoped<IDisasterEventNasaRepo, DisasterEventNasaRepo>();
            return services;
        }
    }
}
