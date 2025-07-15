using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
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
           
            return services;
        }
    }
}
