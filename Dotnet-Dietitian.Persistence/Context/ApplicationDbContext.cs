using Dotnet_Dietitian.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Dietitian.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<Diyetisyen> Diyetisyenler { get; set; }
    public DbSet<Hasta> Hastalar { get; set; }
    public DbSet<DiyetProgrami> DiyetProgramlari { get; set; }
    public DbSet<OdemeBilgisi> OdemeBilgileri { get; set; }
    public DbSet<Randevu> Randevular { get; set; }
    public DbSet<DiyetisyenUygunluk> DiyetisyenUygunluklar { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Diyetisyen yapılandırması
        modelBuilder.Entity<Diyetisyen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TcKimlikNumarasi).HasMaxLength(11).IsRequired();
            entity.Property(e => e.Ad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Soyad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Puan).HasDefaultValue(0);
            entity.Property(e => e.ToplamYorumSayisi).HasDefaultValue(0);
            
            entity.HasIndex(e => e.TcKimlikNumarasi).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Telefon).IsUnique();
        });
        
        // Hasta yapılandırması
        modelBuilder.Entity<Hasta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TcKimlikNumarasi).HasMaxLength(11).IsRequired();
            entity.Property(e => e.Ad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Soyad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telefon).HasMaxLength(20);
            
            entity.HasIndex(e => e.TcKimlikNumarasi).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Telefon).IsUnique();
            
            entity.HasOne(h => h.Diyetisyen)
                    .WithMany(d => d.Hastalar)
                    .HasForeignKey(h => h.DiyetisyenId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
            entity.HasOne(h => h.DiyetProgrami)
                    .WithMany(dp => dp.Hastalar)
                    .HasForeignKey(h => h.DiyetProgramiId)
                    .OnDelete(DeleteBehavior.SetNull);
        });
        
        // DiyetProgrami yapılandırması
        modelBuilder.Entity<DiyetProgrami>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).HasMaxLength(100).IsRequired();
            
            entity.HasOne(dp => dp.OlusturanDiyetisyen)
                    .WithMany(d => d.OlusturulanProgramlar)
                    .HasForeignKey(dp => dp.OlusturanDiyetisyenId)
                    .OnDelete(DeleteBehavior.SetNull);
        });
        
        // OdemeBilgisi yapılandırması
        modelBuilder.Entity<OdemeBilgisi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tutar).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Tarih).IsRequired();
            entity.Property(e => e.OdemeTuru).HasMaxLength(50);
            
            entity.HasOne(o => o.Hasta)
                    .WithMany(h => h.Odemeler)
                    .HasForeignKey(o => o.HastaId)
                    .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Randevu yapılandırması
        modelBuilder.Entity<Randevu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RandevuBaslangicTarihi).IsRequired();
            entity.Property(e => e.RandevuBitisTarihi).IsRequired();
            entity.Property(e => e.RandevuTuru).HasMaxLength(50);
            entity.Property(e => e.Durum).HasMaxLength(50).HasDefaultValue("Bekliyor");
            entity.Property(e => e.DiyetisyenOnayi).HasDefaultValue(false);
            entity.Property(e => e.HastaOnayi).HasDefaultValue(false);
            
            entity.HasOne(r => r.Hasta)
                    .WithMany(h => h.Randevular)
                    .HasForeignKey(r => r.HastaId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
            entity.HasOne(r => r.Diyetisyen)
                    .WithMany(d => d.Randevular)
                    .HasForeignKey(r => r.DiyetisyenId)
                    .OnDelete(DeleteBehavior.Cascade);
        });
        
        // DiyetisyenUygunluk yapılandırması
        modelBuilder.Entity<DiyetisyenUygunluk>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Gun).IsRequired();
            entity.Property(e => e.BaslangicSaati).IsRequired();
            entity.Property(e => e.BitisSaati).IsRequired();
            entity.Property(e => e.TekrarTipi).HasMaxLength(20);
            
            entity.HasOne(du => du.Diyetisyen)
                    .WithMany(d => d.UygunlukZamanlari)
                    .HasForeignKey(du => du.DiyetisyenId)
                    .OnDelete(DeleteBehavior.Cascade);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}