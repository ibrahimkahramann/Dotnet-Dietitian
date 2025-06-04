using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKullaniciAyarlariTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KullaniciAyarlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dil = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZamanDilimi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarihFormati = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlcuBirimi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalismaBaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: true),
                    CalismaBitisSaati = table.Column<TimeSpan>(type: "time", nullable: true),
                    HaftaSonuCalisma = table.Column<bool>(type: "bit", nullable: false),
                    EmailRandevuBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    EmailMesajBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    EmailDiyetGuncellemeBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    EmailPazarlamaBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    UygulamaRandevuBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    UygulamaMesajBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    UygulamaDiyetGuncellemeBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    UygulamaGunlukHatirlatmalar = table.Column<bool>(type: "bit", nullable: false),
                    EmailYeniHastaBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    UygulamaYeniHastaBildirimleri = table.Column<bool>(type: "bit", nullable: false),
                    YeniGirisUyarilari = table.Column<bool>(type: "bit", nullable: false),
                    OturumZamanAsimi = table.Column<bool>(type: "bit", nullable: false),
                    SaglikVerisiPaylasimiIzni = table.Column<bool>(type: "bit", nullable: false),
                    AktiviteVerisiPaylasimiIzni = table.Column<bool>(type: "bit", nullable: false),
                    AnonimKullanimVerisiPaylasimiIzni = table.Column<bool>(type: "bit", nullable: false),
                    ProfilGorunurlugu = table.Column<bool>(type: "bit", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PanelDuzeni = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RenkSemasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IlerlemeGrafigiGoster = table.Column<bool>(type: "bit", nullable: false),
                    SuTakibiGoster = table.Column<bool>(type: "bit", nullable: false),
                    KaloriTakibiGoster = table.Column<bool>(type: "bit", nullable: false),
                    SonGuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciAyarlari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciAyarlari_KullaniciId_KullaniciTipi",
                table: "KullaniciAyarlari",
                columns: new[] { "KullaniciId", "KullaniciTipi" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciAyarlari");
        }
    }
}
