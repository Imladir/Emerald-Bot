using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class CharactersOnMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ServerID",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "Messages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CharacterID",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CharacterID",
                table: "Messages",
                column: "CharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_NameAliases_CharacterID",
                table: "Messages",
                column: "CharacterID",
                principalTable: "NameAliases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_NameAliases_CharacterID",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_CharacterID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CharacterID",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "ServerID",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "Messages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
