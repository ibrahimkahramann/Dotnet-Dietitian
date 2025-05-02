using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Infrastructure.Services;
using Dotnet_Dietitian.Persistence.Context;
using Dotnet_Dietitian.Persistence.Repositories;
using Dotnet_Dietitian.Persistence.Repositories.AppUserRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet_Dietitian.API.Extensions
{
    //DI Injections
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
            services.AddScoped<IDiyetProgramiRepository, DiyetProgramiRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            
            // Services
            services.AddScoped<IDiyetisyenService, DiyetisyenService>();
            services.AddScoped<IHastaService, HastaService>();
            services.AddScoped<IDiyetProgramiService, DiyetProgramiService>();
            
            // Infrastructure services
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            
            return services;
        }
    }
}