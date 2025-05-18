using System;
using System.Collections.Generic;

namespace Dotnet_Dietitian.API.Models
{
    public class GetHastaByIdQueryResult
    {
        public Guid Id { get; set; }
        public string? TcKimlikNumarasi { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefon { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string? Cinsiyet { get; set; }
        public decimal? Boy { get; set; }  // cm cinsinden
        public decimal? Kilo { get; set; } // kg cinsinden
        public int? Yas { get; set; }
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string? KanGrubu { get; set; }
        public string? KronikHastaliklar { get; set; }
        public string? Alerjiler { get; set; }
        public string? IlacKullanimi { get; set; }
        
        public Guid? DiyetisyenId { get; set; }
        public string? DiyetisyenAdi { get; set; }
        
        public Guid? DiyetProgramiId { get; set; }
        public string? DiyetProgramiAdi { get; set; }
        
        public List<RandevuDto>? Randevular { get; set; }
        public List<OdemeBilgisiDto>? Odemeler { get; set; }
    }

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