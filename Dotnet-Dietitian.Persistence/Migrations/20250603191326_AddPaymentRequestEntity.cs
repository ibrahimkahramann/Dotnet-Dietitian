using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRequestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HastaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiyetProgramiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    VadeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeBilgisiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RedNotu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_DiyetProgramlari_DiyetProgramiId",
                        column: x => x.DiyetProgramiId,
                        principalTable: "DiyetProgramlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Diyetisyenler_DiyetisyenId",
                        column: x => x.DiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentRequests_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);                    table.ForeignKey(
                        name: "FK_PaymentRequests_OdemeBilgileri_OdemeBilgisiId",
                        column: x => x.OdemeBilgisiId,
                        principalTable: "OdemeBilgileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_DiyetisyenId",
                table: "PaymentRequests",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_DiyetProgramiId",
                table: "PaymentRequests",
                column: "DiyetProgramiId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_HastaId",
                table: "PaymentRequests",
                column: "HastaId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRequests_OdemeBilgisiId",
                table: "PaymentRequests",
                column: "OdemeBilgisiId",
                unique: true,
                filter: "[OdemeBilgisiId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentRequests");
        }
    }
}
