using System;

namespace Dotnet_Dietitian.Domain.Entities
{
    public class Mesaj : BaseEntity
    {
        public Guid GonderenId { get; set; }
        public string GonderenTipi { get; set; } // "Diyetisyen" veya "Hasta"
        public Guid AliciId { get; set; }
        public string AliciTipi { get; set; } // "Diyetisyen" veya "Hasta"
        public string Icerik { get; set; }
        public DateTime GonderimZamani { get; set; } = DateTime.Now;
        public bool Okundu { get; set; } = false;
        public DateTime? OkunmaZamani { get; set; }
        
        // Gönderen navigation properties
        public virtual Diyetisyen? GonderenDiyetisyen { get; set; }
        public virtual Hasta? GonderenHasta { get; set; }
        
        // Alıcı navigation properties
        public virtual Diyetisyen? AliciDiyetisyen { get; set; }
        public virtual Hasta? AliciHasta { get; set; }
    }
}