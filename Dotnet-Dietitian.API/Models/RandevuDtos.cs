using System;
using System.Collections.Generic;

namespace Dotnet_Dietitian.API.Models
{
    public class RandevuDto
    {
        public Guid Id { get; set; }
        public DateTime RandevuBaslangicTarihi { get; set; }
        public DateTime RandevuBitisTarihi { get; set; }
        public string? RandevuTuru { get; set; }
        public string? Notlar { get; set; }
        public string? Durum { get; set; }
        public bool? DiyetisyenOnayi { get; set; }
        public bool? HastaOnayi { get; set; }
    }

    public class OdemeBilgisiDto
    {
        public Guid Id { get; set; }
        public DateTime Tarih { get; set; }
        public decimal Tutar { get; set; }
        public string? OdemeTuru { get; set; }
        public string? IslemReferansNo { get; set; }
        public string? Aciklama { get; set; }
    }
}