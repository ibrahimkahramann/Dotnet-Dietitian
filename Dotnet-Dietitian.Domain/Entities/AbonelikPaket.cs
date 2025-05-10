using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Domain.Entities
{
    public class AbonelikPaket:BaseEntity
    {
        public string Ad { get; set; }
        public string Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int SureDegeri { get; set; }
        public string SureTipi { get; set; } // "Ay", "Yıl" vs.
        public bool Aktif { get; set; } = true;
        public int MaxRandevuSayisi { get; set; }
        public decimal Indirim { get; set; } = 0;
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Abonelik> Abonelikler { get; set; }

    }
}
