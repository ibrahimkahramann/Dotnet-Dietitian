using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dotnet_Dietitian.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.API.Controllers
{
    [Authorize(Roles = "Hasta, Admin")]
    public class PatientController : Controller
    {
        public IActionResult Dashboard()
        {
            // Gerçek uygulamada burada veri tabanından hasta verisi çekilir
            // Şimdilik test verileri oluşturalım
            var model = new GetHastaByIdQueryResult
            {
                Id = Guid.NewGuid(),
                Ad = "Ahmet",
                Soyad = "Yılmaz",
                Email = "ahmet.yilmaz@example.com",
                Telefon = "05551234567",
                Kilo = 78.5M,
                Boy = 178.0M,
                Yas = 35,
                DiyetProgramiAdi = "Akdeniz Diyeti",
                DiyetisyenAdi = "Dr. Ayşe Kaya",
                
                Randevular = new List<RandevuDto>
                {
                    new RandevuDto 
                    { 
                        Id = Guid.NewGuid(), 
                        RandevuBaslangicTarihi = DateTime.Now.AddDays(3),
                        RandevuBitisTarihi = DateTime.Now.AddDays(3).AddHours(1),
                        RandevuTuru = "Kontrol",
                        Durum = "Onaylandı"
                    },
                    new RandevuDto 
                    { 
                        Id = Guid.NewGuid(), 
                        RandevuBaslangicTarihi = DateTime.Now.AddDays(10),
                        RandevuBitisTarihi = DateTime.Now.AddDays(10).AddHours(1),
                        RandevuTuru = "Diyet Planı Güncelleme",
                        Durum = "Bekliyor"
                    }
                }
            };
            
            return View(model);
        }
        
        public IActionResult DietProgram()
        {
            return View();
        }
        
        public IActionResult Appointments()
        {
            return View();
        }
        
        public IActionResult Messages()
        {
            return View();
        }
        
        public IActionResult Profile()
        {
            return View();
        }
    }
}