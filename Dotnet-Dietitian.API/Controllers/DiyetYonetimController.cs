using Dotnet_Dietitian.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiyetYonetimController : ControllerBase
    {
        private readonly IDiyetYonetimFacade _diyetYonetim;

        public DiyetYonetimController(IDiyetYonetimFacade diyetYonetim)
        {
            _diyetYonetim = diyetYonetim;
        }

        [HttpGet("hasta/{hastaId}/detay")]
        public async Task<IActionResult> GetHastaDetay(Guid hastaId)
        {
            var hasta = await _diyetYonetim.GetHastaWithDetailsAsync(hastaId);
            return Ok(hasta);
        }

        [HttpPost("hasta/ata")]
        public async Task<IActionResult> AtamaYap([FromQuery] Guid hastaId, [FromQuery] Guid diyetisyenId, [FromQuery] Guid diyetProgramiId)
        {
            var result = await _diyetYonetim.AtamaYapAsync(hastaId, diyetisyenId, diyetProgramiId);
            if (result)
                return Ok("Atama başarıyla yapıldı");
            else
                return BadRequest("Atama yapılamadı");
        }

        [HttpPost("randevu/olustur")]
        public async Task<IActionResult> RandevuOlustur([FromBody] RandevuOlusturModel model)
        {
            var result = await _diyetYonetim.RandevuOlusturAsync(
                model.HastaId, 
                model.DiyetisyenId, 
                model.BaslangicZamani, 
                TimeSpan.FromMinutes(model.SureDakika),
                model.Notlar
            );
            
            if (result)
                return Ok("Randevu başarıyla oluşturuldu");
            else
                return BadRequest("Randevu oluşturulamadı");
        }

        [HttpGet("hasta/{hastaId}/randevular")]
        public async Task<IActionResult> GetHastaRandevular(Guid hastaId)
        {
            var randevular = await _diyetYonetim.GetHastaGelecekRandevulariAsync(hastaId);
            return Ok(randevular);
        }

        [HttpPost("odeme/yap")]
        public async Task<IActionResult> OdemeYap([FromBody] OdemeYapModel model)
        {
            var result = await _diyetYonetim.OdemeYapAsync(
                model.HastaId,
                model.Tutar,
                model.OdemeTuru,
                model.Aciklama
            );
            
            if (result)
                return Ok("Ödeme başarıyla kaydedildi");
            else
                return BadRequest("Ödeme kaydedilemedi");
        }

        [HttpGet("hasta/{hastaId}/odemeler")]
        public async Task<IActionResult> GetHastaOdemeler(Guid hastaId)
        {
            var odemeler = await _diyetYonetim.GetHastaOdemeleriAsync(hastaId);
            return Ok(odemeler);
        }
    }

    public class RandevuOlusturModel
    {
        public Guid HastaId { get; set; }
        public Guid DiyetisyenId { get; set; }
        public DateTime BaslangicZamani { get; set; }
        public int SureDakika { get; set; } = 60;
        public string Notlar { get; set; }
    }

    public class OdemeYapModel
    {
        public Guid HastaId { get; set; }
        public decimal Tutar { get; set; }
        public string OdemeTuru { get; set; }
        public string Aciklama { get; set; }
    }
}