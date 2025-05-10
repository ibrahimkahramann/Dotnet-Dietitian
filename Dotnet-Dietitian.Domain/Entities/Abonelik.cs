using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Domain.Entities
{
    public class Abonelik
    {
        public Guid HastaId { get; set; }
        public Guid AbonelikPaketId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Durum { get; set; } = "Aktif"; // "Aktif", "Sona Erdi", "İptal Edildi"
        public int KullanilabilirRandevuSayisi { get; set; }
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? SonGuncellemeTarihi { get; set; }

        // Navigation properties
        public virtual Hasta Hasta { get; set; }
        public virtual AbonelikPaket AbonelikPaket { get; set; }
        public virtual ICollection<OdemeBilgisi> Odemeler { get; set; }

    }
}
