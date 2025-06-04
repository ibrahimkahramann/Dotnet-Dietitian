using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIlerlemeOlcumEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IlerlemeOlcumleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HastaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OlcumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kilo = table.Column<float>(type: "real", nullable: false),
                    BelCevresi = table.Column<float>(type: "real", nullable: true),
                    KalcaCevresi = table.Column<float>(type: "real", nullable: true),
                    GogusCevresi = table.Column<float>(type: "real", nullable: true),
                    KolCevresi = table.Column<float>(type: "real", nullable: true),
                    VucutYagOrani = table.Column<float>(type: "real", nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlerlemeOlcumleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlerlemeOlcumleri_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IlerlemeOlcumleri_HastaId",
                table: "IlerlemeOlcumleri",
                column: "HastaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IlerlemeOlcumleri");
        }
    }
}
