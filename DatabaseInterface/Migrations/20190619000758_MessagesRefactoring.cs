using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class MessagesRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Messages",
                newName: "Icon");

            migrationBuilder.AddColumn<int>(
                name: "Colour",
                table: "Messages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colour",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "Messages",
                newName: "Data");

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
    }
}
