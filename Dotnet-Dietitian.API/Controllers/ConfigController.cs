using Dotnet_Dietitian.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IAppConfigService _configService;

        public ConfigController(IAppConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public IActionResult GetConfig()
        {
            var config = new
            {
                ApplicationName = _configService.ApplicationName,
                Version = _configService.Version,
                DemoMode = _configService.DemoMode,
                MaxDailyAppointments = _configService.MaxDailyAppointments,
                DefaultSessionDuration = _configService.DefaultSessionDurationMinutes,
                DefaultConsultationFee = _configService.DefaultConsultationFee,
                LastConfigUpdate = _configService.LastConfigUpdate
            };

            return Ok(config);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshConfig()
        {
            await _configService.RefreshConfigAsync();
            return Ok(new { Message = "Ayarlar yenilendi", LastUpdate = _configService.LastConfigUpdate });
        }
    }
}