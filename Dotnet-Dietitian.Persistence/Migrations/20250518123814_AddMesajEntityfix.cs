using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMesajEntityfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciId",
                table: "Mesajlar",
                column: "AliciId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar",
                column: "GonderenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_AliciId",
                table: "Mesajlar",
                column: "AliciId",
                principalTable: "Diyetisyenler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_GonderenId",
                table: "Mesajlar",
                column: "GonderenId",
                principalTable: "Diyetisyenler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Hastalar_AliciId",
                table: "Mesajlar",
                column: "AliciId",
                principalTable: "Hastalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mesajlar_Hastalar_GonderenId",
                table: "Mesajlar",
                column: "GonderenId",
                principalTable: "Hastalar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_AliciId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Diyetisyenler_GonderenId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Hastalar_AliciId",
                table: "Mesajlar");

            migrationBuilder.DropForeignKey(
                name: "FK_Mesajlar_Hastalar_GonderenId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_AliciId",
                table: "Mesajlar");

            migrationBuilder.DropIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar");
        }
    }
}
