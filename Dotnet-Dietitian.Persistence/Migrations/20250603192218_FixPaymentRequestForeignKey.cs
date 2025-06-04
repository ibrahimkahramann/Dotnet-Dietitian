using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPaymentRequestForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_OdemeBilgileri_OdemeBilgisiId",
                table: "PaymentRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_OdemeBilgileri_OdemeBilgisiId",
                table: "PaymentRequests",
                column: "OdemeBilgisiId",
                principalTable: "OdemeBilgileri",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRequests_OdemeBilgileri_OdemeBilgisiId",
                table: "PaymentRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRequests_OdemeBilgileri_OdemeBilgisiId",
                table: "PaymentRequests",
                column: "OdemeBilgisiId",
                principalTable: "OdemeBilgileri",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
