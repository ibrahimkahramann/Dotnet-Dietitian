using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOlusturulmaTarihi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Randevular",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "OdemeBilgileri",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Mesajlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Hastalar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "DiyetProgramlari",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CalistigiKurum",
                table: "Diyetisyenler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "Diyetisyenler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Unvan",
                table: "Diyetisyenler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "AppUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturulmaTarihi",
                table: "AppRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "OdemeBilgileri");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Mesajlar");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Hastalar");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "DiyetProgramlari");

            migrationBuilder.DropColumn(
                name: "CalistigiKurum",
                table: "Diyetisyenler");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "Diyetisyenler");

            migrationBuilder.DropColumn(
                name: "Unvan",
                table: "Diyetisyenler");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "OlusturulmaTarihi",
                table: "AppRoles");
        }
    }
}
