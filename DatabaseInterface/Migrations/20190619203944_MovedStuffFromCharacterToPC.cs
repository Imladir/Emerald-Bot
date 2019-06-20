using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class MovedStuffFromCharacterToPC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterConditions_NameAliases_CharacterID",
                table: "CharacterConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_ConflictParticipant_Conflicts_ConflictID1",
                table: "ConflictParticipant");

            migrationBuilder.DropIndex(
                name: "IX_ConflictParticipant_ConflictID1",
                table: "ConflictParticipant");

            migrationBuilder.DropColumn(
                name: "ConflictID1",
                table: "ConflictParticipant");

            migrationBuilder.RenameColumn(
                name: "CharacterID",
                table: "CharacterConditions",
                newName: "PCID");

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "ConflictParticipant",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterConditions_NameAliases_PCID",
                table: "CharacterConditions",
                column: "PCID",
                principalTable: "NameAliases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterConditions_NameAliases_PCID",
                table: "CharacterConditions");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "ConflictParticipant");

            migrationBuilder.RenameColumn(
                name: "PCID",
                table: "CharacterConditions",
                newName: "CharacterID");

            migrationBuilder.AddColumn<int>(
                name: "ConflictID1",
                table: "ConflictParticipant",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConflictParticipant_ConflictID1",
                table: "ConflictParticipant",
                column: "ConflictID1");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterConditions_NameAliases_CharacterID",
                table: "CharacterConditions",
                column: "CharacterID",
                principalTable: "NameAliases",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConflictParticipant_Conflicts_ConflictID1",
                table: "ConflictParticipant",
                column: "ConflictID1",
                principalTable: "Conflicts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
