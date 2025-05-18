using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MesajEntityAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_DiyetisyenId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Hastalar_HastaId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_DiyetisyenId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_HastaId",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "DiyetisyenId",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "HastaId",
                table: "Mesajlar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DiyetisyenId",
                table: "Mesajlar",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HastaId",
                table: "Mesajlar",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_DiyetisyenId",
                table: "Mesajlar",
                column: "DiyetisyenId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_HastaId",
                table: "Mesajlar",
                column: "HastaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_DiyetisyenId",
                table: "Mesajlar",
                column: "DiyetisyenId",
                principalTable: "Diyetisyenler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Hastalar_HastaId",
                table: "Mesajlar",
                column: "HastaId",
                principalTable: "Hastalar",
                principalColumn: "Id");
        }
    }
}
