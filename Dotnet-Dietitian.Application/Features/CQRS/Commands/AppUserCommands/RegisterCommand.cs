using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands
{
    public class RegisterCommand : IRequest<Guid>
    {
        [Required(ErrorMessage = "T.C. Kimlik No gereklidir")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır")]
        [Display(Name = "T.C. Kimlik No")]
        public string IdentityNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ad gereklidir")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Soyad gereklidir")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;
        
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime? BirthDate { get; set; }
        
        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }
        
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;
        
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Hesap tipi seçiniz")]
        [Display(Name = "Hesap Tipi")]
        public string UserType { get; set; } = "Hasta";
        
        [Required(ErrorMessage = "Kullanım koşullarını kabul etmeniz gerekmektedir")]
        [Display(Name = "Kullanım Koşulları")]
        public bool AgreeTerms { get; set; }

        // Diyetisyen-specific fields
        [Display(Name = "Lisans Numarası")]
        public string? LicenseNumber { get; set; }
        
        [Display(Name = "Uzmanlık Alanı")]
        public string? Specialty { get; set; }
        
        [Display(Name = "Mezuniyet Okulu")]
        public string? GraduationSchool { get; set; }
        
        [Display(Name = "Deneyim Yılı")]
        [Range(0, 100, ErrorMessage = "Geçerli bir deneyim yılı giriniz")]
        public int? ExperienceYears { get; set; }
        
        [Display(Name = "Şehir")]
        public string? City { get; set; }
        
        [Display(Name = "Hakkında")]
        public string? About { get; set; }
    }
} 