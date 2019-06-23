using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class UpdateNPCs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MartialRank",
                table: "NameAliases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SocialRank",
                table: "NameAliases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MartialRank",
                table: "NameAliases");

            migrationBuilder.DropColumn(
                name: "SocialRank",
                table: "NameAliases");
        }
    }
}
