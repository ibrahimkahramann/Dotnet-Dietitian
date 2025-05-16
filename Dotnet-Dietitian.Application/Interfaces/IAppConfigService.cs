namespace Dotnet_Dietitian.Application.Interfaces;

public interface IAppConfigService
{
    string ApplicationName { get; }
    string Version { get; }
    bool DemoMode { get; set; }
    int MaxDailyAppointments { get; set; }
    int DefaultSessionDurationMinutes { get; set; }
    decimal DefaultConsultationFee { get; set; }
    DateTime LastConfigUpdate { get; set; }
    
    // Ayarları yenileme metodu 
    Task RefreshConfigAsync();
}