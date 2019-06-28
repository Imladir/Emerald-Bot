using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class SmallFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySum_MoneyID",
                table: "NameAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySum_CostID",
                table: "NameAliases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoneySum",
                table: "MoneySum");

            migrationBuilder.RenameTable(
                name: "MoneySum",
                newName: "MoneySums");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "DieFaces",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmoteID",
                table: "DieFaces",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoneySums",
                table: "MoneySums",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NameAliases_MoneySums_MoneyID",
                table: "NameAliases",
                column: "MoneyID",
                principalTable: "MoneySums",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NameAliases_MoneySums_CostID",
                table: "NameAliases",
                column: "CostID",
                principalTable: "MoneySums",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySums_MoneyID",
                table: "NameAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySums_CostID",
                table: "NameAliases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoneySums",
                table: "MoneySums");

            migrationBuilder.RenameTable(
                name: "MoneySums",
                newName: "MoneySum");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "DieFaces",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<int>(
                name: "EmoteID",
                table: "DieFaces",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoneySum",
                table: "MoneySum",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NameAliases_MoneySum_MoneyID",
                table: "NameAliases",
                column: "MoneyID",
                principalTable: "MoneySum",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NameAliases_MoneySum_CostID",
                table: "NameAliases",
                column: "CostID",
                principalTable: "MoneySum",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
