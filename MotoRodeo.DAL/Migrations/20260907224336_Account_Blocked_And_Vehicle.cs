using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoRodeo.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Account_Blocked_And_Vehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Vehicle",
                table: "Accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Vehicle",
                table: "Accounts");
        }
    }
}