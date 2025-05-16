using Dotnet_Dietitian.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Infrastructure.Services;

public class AppConfigService : IAppConfigService
{
    private readonly IConfiguration _configuration;
    
    public AppConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Başlangıç değerlerini yükle
        ApplicationName = "Dotnet-Dietitian";
        Version = "1.0.0";
        
        // GetValue yerine alternatif
        if (bool.TryParse(_configuration["AppSettings:DemoMode"], out bool demoMode))
            DemoMode = demoMode;
        else
            DemoMode = false;
        
        if (int.TryParse(_configuration["AppSettings:MaxDailyAppointments"], out int maxAppts))
            MaxDailyAppointments = maxAppts;
        else
            MaxDailyAppointments = 10;
        
        if (int.TryParse(_configuration["AppSettings:DefaultSessionDurationMinutes"], out int duration))
            DefaultSessionDurationMinutes = duration;
        else
            DefaultSessionDurationMinutes = 60;
        
        if (decimal.TryParse(_configuration["AppSettings:DefaultConsultationFee"], out decimal fee))
            DefaultConsultationFee = fee;
        else
            DefaultConsultationFee = 500;
        
        LastConfigUpdate = DateTime.Now;
    }
    
    public string ApplicationName { get; private set; }
    public string Version { get; private set; }
    public bool DemoMode { get; set; }
    public int MaxDailyAppointments { get; set; }
    public int DefaultSessionDurationMinutes { get; set; }
    public decimal DefaultConsultationFee { get; set; }
    public DateTime LastConfigUpdate { get; set; }
    
    public async Task RefreshConfigAsync()
    {
        // Burada DB veya dış bir API'den dinamik ayarlar yüklenebilir
        await Task.Delay(100); // Simüle edilmiş asenkron operasyon
        
        // TryParse ile değerleri güncelleme
        if (bool.TryParse(_configuration["AppSettings:DemoMode"], out bool demoMode))
            DemoMode = demoMode;
        
        if (int.TryParse(_configuration["AppSettings:MaxDailyAppointments"], out int maxAppts))
            MaxDailyAppointments = maxAppts;
        
        if (int.TryParse(_configuration["AppSettings:DefaultSessionDurationMinutes"], out int duration))
            DefaultSessionDurationMinutes = duration;
        
        if (decimal.TryParse(_configuration["AppSettings:DefaultConsultationFee"], out decimal fee))
            DefaultConsultationFee = fee;
        
        LastConfigUpdate = DateTime.Now;
    }
}