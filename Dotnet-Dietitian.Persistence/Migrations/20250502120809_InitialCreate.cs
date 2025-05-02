using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diyetisyenler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TcKimlikNumarasi = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Uzmanlik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MezuniyetOkulu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeneyimYili = table.Column<int>(type: "int", nullable: true),
                    Puan = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    ToplamYorumSayisi = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Hakkinda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilResmiUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sehir = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diyetisyenler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_AppRoles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalTable: "AppRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiyetisyenUygunluklar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gun = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaslangicSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaati = table.Column<TimeSpan>(type: "time", nullable: false),
                    TekrarTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiyetisyenUygunluklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiyetisyenUygunluklar_Diyetisyenler_DiyetisyenId",
                        column: x => x.DiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiyetProgramlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YagGram = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProteinGram = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KarbonhidratGram = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GunlukAdimHedefi = table.Column<int>(type: "int", nullable: true),
                    OlusturanDiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiyetProgramlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiyetProgramlari_Diyetisyenler_OlusturanDiyetisyenId",
                        column: x => x.OlusturanDiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Hastalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TcKimlikNumarasi = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Yas = table.Column<int>(type: "int", nullable: true),
                    Boy = table.Column<float>(type: "real", nullable: true),
                    Kilo = table.Column<float>(type: "real", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DiyetProgramiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GunlukKaloriIhtiyaci = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hastalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hastalar_DiyetProgramlari_DiyetProgramiId",
                        column: x => x.DiyetProgramiId,
                        principalTable: "DiyetProgramlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Hastalar_Diyetisyenler_DiyetisyenId",
                        column: x => x.DiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OdemeBilgileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HastaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdemeTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IslemReferansNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdemeBilgileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdemeBilgileri_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Randevular",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HastaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RandevuBaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RandevuBitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RandevuTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Bekliyor"),
                    DiyetisyenOnayi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HastaOnayi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Notlar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YaratilmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Randevular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Randevular_Diyetisyenler_DiyetisyenId",
                        column: x => x.DiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Randevular_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_AppRoleId",
                table: "AppUsers",
                column: "AppRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Diyetisyenler_Email",
                table: "Diyetisyenler",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diyetisyenler_TcKimlikNumarasi",
                table: "Diyetisyenler",
                column: "TcKimlikNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diyetisyenler_Telefon",
                table: "Diyetisyenler",
                column: "Telefon",
                unique: true,
                filter: "[Telefon] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DiyetisyenUygunluklar_DiyetisyenId",
                table: "DiyetisyenUygunluklar",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_DiyetProgramlari_OlusturanDiyetisyenId",
                table: "DiyetProgramlari",
                column: "OlusturanDiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_DiyetisyenId",
                table: "Hastalar",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_DiyetProgramiId",
                table: "Hastalar",
                column: "DiyetProgramiId");

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_Email",
                table: "Hastalar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_TcKimlikNumarasi",
                table: "Hastalar",
                column: "TcKimlikNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hastalar_Telefon",
                table: "Hastalar",
                column: "Telefon",
                unique: true,
                filter: "[Telefon] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OdemeBilgileri_HastaId",
                table: "OdemeBilgileri",
                column: "HastaId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_DiyetisyenId",
                table: "Randevular",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_HastaId",
                table: "Randevular",
                column: "HastaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "DiyetisyenUygunluklar");

            migrationBuilder.DropTable(
                name: "OdemeBilgileri");

            migrationBuilder.DropTable(
                name: "Randevular");

            migrationBuilder.DropTable(
                name: "AppRoles");

            migrationBuilder.DropTable(
                name: "Hastalar");

            migrationBuilder.DropTable(
                name: "DiyetProgramlari");

            migrationBuilder.DropTable(
                name: "Diyetisyenler");
        }
    }
}
