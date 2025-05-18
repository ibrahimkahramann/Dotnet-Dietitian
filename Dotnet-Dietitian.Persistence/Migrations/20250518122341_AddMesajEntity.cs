using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMesajEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BaslangicZamani",
                table: "DiyetisyenUygunluklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BitisZamani",
                table: "DiyetisyenUygunluklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Musait",
                table: "DiyetisyenUygunluklar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notlar",
                table: "DiyetisyenUygunluklar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "DiyetisyenUygunluklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "TekrarlanirMi",
                table: "DiyetisyenUygunluklar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TekrarlanmaSikligi",
                table: "DiyetisyenUygunluklar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GonderenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GonderenTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AliciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AliciTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GonderimZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Okundu = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OkunmaZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiyetisyenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HastaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Diyetisyenler_DiyetisyenId",
                        column: x => x.DiyetisyenId,
                        principalTable: "Diyetisyenler",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mesajlar_Hastalar_HastaId",
                        column: x => x.HastaId,
                        principalTable: "Hastalar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_DiyetisyenId",
                table: "Mesajlar",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_HastaId",
                table: "Mesajlar",
                column: "HastaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "BaslangicZamani",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "BitisZamani",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "Musait",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "Notlar",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "TekrarlanirMi",
                table: "DiyetisyenUygunluklar");

            migrationBuilder.DropColumn(
                name: "TekrarlanmaSikligi",
                table: "DiyetisyenUygunluklar");
        }
    }
}
