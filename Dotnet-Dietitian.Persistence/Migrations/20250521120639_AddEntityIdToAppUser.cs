using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet_Dietitian.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityIdToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "AppUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AppUsers");
        }
    }
}
