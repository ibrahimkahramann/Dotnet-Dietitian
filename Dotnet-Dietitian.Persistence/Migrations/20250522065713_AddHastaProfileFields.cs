using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHastaProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Alerjiler",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cinsiyet",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DogumTarihi",
                table: "Hastalar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KanGrubu",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KronikHastaliklar",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KullanilanIlaclar",
                table: "Hastalar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SaglikBilgisiPaylasimiIzni",
                table: "Hastalar",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adres",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Alerjiler",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "DogumTarihi",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "KanGrubu",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "KronikHastaliklar",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "KullanilanIlaclar",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "SaglikBilgisiPaylasimiIzni",
                table: "Hastalar");
        }
    }
}
