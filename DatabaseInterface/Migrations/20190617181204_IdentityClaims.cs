using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class IdentityClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordSalt",
                table: "Users",
                newName: "NormalizedUserName");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedRoleName",
                table: "Roles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedRoleName",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "Users",
                newName: "PasswordSalt");
        }
    }
}
