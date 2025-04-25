using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Persistence.Context;
using Dotnet_Dietitian.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet_Dietitian.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IDiyetisyenRepository, DiyetisyenRepository>();
            services.AddScoped<IHastaRepository, HastaRepository>();
            
            // Services
            services.AddScoped<IDiyetisyenService, DiyetisyenService>();
            
            return services;
        }
    }
}