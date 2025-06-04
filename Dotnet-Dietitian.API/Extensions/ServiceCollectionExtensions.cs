using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Interfaces.AppUserInterfaces;
using Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands;
using Dotnet_Dietitian.Application.Services;
using Dotnet_Dietitian.Infrastructure.Services;
using Dotnet_Dietitian.Persistence.Context;
using Dotnet_Dietitian.Persistence.Repositories;
using Dotnet_Dietitian.Persistence.Repositories.AppUserRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Dotnet_Dietitian.Domain.Entities;
using Dotnet_Dietitian.Application.Decorators;
using Dotnet_Dietitian.Application.Strategies;
using Dotnet_Dietitian.Application.TemplatePattern;

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
            
           // MediatR kaydı
            services.AddMediatR(typeof(LoginCommand).Assembly);        

            services.AddSingleton<IAppConfigService, AppConfigService>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IDiyetisyenRepository, DiyetisyenRepository>();
            services.AddScoped<IHastaRepository, HastaRepository>();
            services.AddScoped<IDiyetProgramiRepository, DiyetProgramiRepository>();            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IRandevuRepository, RandevuRepository>();
            services.AddScoped<IDiyetisyenUygunlukRepository, DiyetisyenUygunlukRepository>();
            services.AddScoped<IIlerlemeOlcumRepository, IlerlemeOlcumRepository>();
            services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
            
            // Services
            services.AddScoped<IDiyetisyenService, DiyetisyenService>();
            services.AddScoped<IHastaService, HastaService>();
            services.AddScoped<IDiyetProgramiService, DiyetProgramiService>();
            
            // Infrastructure services
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            
            // MemoryCache servisini ekleyin
            services.AddMemoryCache();
            
            // Repository kaydı
            services.AddScoped<IMesajRepository, MesajRepository>();
            
            // Decorator ile repository sınıflarını kaydedin
            // Hasta Repository için Örnek
            services.AddScoped<BaseRepository<Hasta>, HastaRepository>();
            services.AddScoped<IRepository<Hasta>>(provider => {
                var baseRepo = provider.GetRequiredService<BaseRepository<Hasta>>();
                var logger = provider.GetRequiredService<ILogger<LoggingRepositoryDecorator<Hasta>>>();
                var cache = provider.GetRequiredService<IMemoryCache>();
                
                // Önce loglama, sonra cache ile sarmala
                return new CachingRepositoryDecorator<Hasta>(
                    new LoggingRepositoryDecorator<Hasta>(baseRepo, logger), 
                    cache
                );
            });
            
            // Diyetisyen Repository için de benzer eklemeler yapılabilir
            
            // Facade servisini ekleyin
            services.AddScoped<IDiyetYonetimFacade, DiyetYonetimFacade>();
            
            // Strategy pattern kayıtları
            services.AddScoped<IKaloriHesaplamaStrategy, HarrisBenedictKaloriStrategy>();
            services.AddScoped<KaloriHesaplamaService>();

            // İhtiyaç duyulduğunda alternatif strateji seçimi için named instance kayıtları
            services.AddScoped<HarrisBenedictKaloriStrategy>();
            services.AddScoped<MifflinStJeorKaloriStrategy>();
            
            // Template Pattern kayıtları
            services.AddScoped<DiyetProgramOlusturucuFactory>();
            
            return services;
        }
    }
}