using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class Gear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GearQualitiesGear_GearQuality_GearQualityID",
                table: "GearQualitiesGear");

            migrationBuilder.DropForeignKey(
                name: "FK_GearQuality_Source_SourceID",
                table: "GearQuality");

            migrationBuilder.DropForeignKey(
                name: "FK_RollDie_DieFaces_FaceID",
                table: "RollDie");

            migrationBuilder.DropForeignKey(
                name: "FK_RollDie_Rolls_RollID",
                table: "RollDie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RollDie",
                table: "RollDie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GearQuality",
                table: "GearQuality");

            migrationBuilder.RenameTable(
                name: "RollDie",
                newName: "RollDice");

            migrationBuilder.RenameTable(
                name: "GearQuality",
                newName: "GearQualities");

            migrationBuilder.RenameIndex(
                name: "IX_RollDie_RollID",
                table: "RollDice",
                newName: "IX_RollDice_RollID");

            migrationBuilder.RenameIndex(
                name: "IX_RollDie_FaceID",
                table: "RollDice",
                newName: "IX_RollDice_FaceID");

            migrationBuilder.RenameIndex(
                name: "IX_GearQuality_SourceID",
                table: "GearQualities",
                newName: "IX_GearQualities_SourceID");

            migrationBuilder.AlterColumn<string>(
                name: "Gear_Description",
                table: "NameAliases",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoneyID",
                table: "NameAliases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostID",
                table: "NameAliases",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RarityID",
                table: "NameAliases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GearQualities",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RollDice",
                table: "RollDice",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GearQualities",
                table: "GearQualities",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "CharacterGear",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    GearID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterGear", x => new { x.GearID, x.CharacterID });
                    table.ForeignKey(
                        name: "FK_CharacterGear_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterGear_NameAliases_GearID",
                        column: x => x.GearID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GearRarity",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MinRarity = table.Column<int>(nullable: false),
                    MaxRarity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearRarity", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MoneySum",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Koku = table.Column<int>(nullable: false),
                    Bu = table.Column<int>(nullable: false),
                    Zeni = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneySum", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_MoneyID",
                table: "NameAliases",
                column: "MoneyID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_CostID",
                table: "NameAliases",
                column: "CostID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_RarityID",
                table: "NameAliases",
                column: "RarityID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterGear_CharacterID",
                table: "CharacterGear",
                column: "CharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_GearQualities_Source_SourceID",
                table: "GearQualities",
                column: "SourceID",
                principalTable: "Source",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GearQualitiesGear_GearQualities_GearQualityID",
                table: "GearQualitiesGear",
                column: "GearQualityID",
                principalTable: "GearQualities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_NameAliases_GearRarity_RarityID",
                table: "NameAliases",
                column: "RarityID",
                principalTable: "GearRarity",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RollDice_DieFaces_FaceID",
                table: "RollDice",
                column: "FaceID",
                principalTable: "DieFaces",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RollDice_Rolls_RollID",
                table: "RollDice",
                column: "RollID",
                principalTable: "Rolls",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GearQualities_Source_SourceID",
                table: "GearQualities");

            migrationBuilder.DropForeignKey(
                name: "FK_GearQualitiesGear_GearQualities_GearQualityID",
                table: "GearQualitiesGear");

            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySum_MoneyID",
                table: "NameAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_MoneySum_CostID",
                table: "NameAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_NameAliases_GearRarity_RarityID",
                table: "NameAliases");

            migrationBuilder.DropForeignKey(
                name: "FK_RollDice_DieFaces_FaceID",
                table: "RollDice");

            migrationBuilder.DropForeignKey(
                name: "FK_RollDice_Rolls_RollID",
                table: "RollDice");

            migrationBuilder.DropTable(
                name: "CharacterGear");

            migrationBuilder.DropTable(
                name: "GearRarity");

            migrationBuilder.DropTable(
                name: "MoneySum");

            migrationBuilder.DropIndex(
                name: "IX_NameAliases_MoneyID",
                table: "NameAliases");

            migrationBuilder.DropIndex(
                name: "IX_NameAliases_CostID",
                table: "NameAliases");

            migrationBuilder.DropIndex(
                name: "IX_NameAliases_RarityID",
                table: "NameAliases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RollDice",
                table: "RollDice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GearQualities",
                table: "GearQualities");

            migrationBuilder.DropColumn(
                name: "MoneyID",
                table: "NameAliases");

            migrationBuilder.DropColumn(
                name: "CostID",
                table: "NameAliases");

            migrationBuilder.DropColumn(
                name: "RarityID",
                table: "NameAliases");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "GearQualities");

            migrationBuilder.RenameTable(
                name: "RollDice",
                newName: "RollDie");

            migrationBuilder.RenameTable(
                name: "GearQualities",
                newName: "GearQuality");

            migrationBuilder.RenameIndex(
                name: "IX_RollDice_RollID",
                table: "RollDie",
                newName: "IX_RollDie_RollID");

            migrationBuilder.RenameIndex(
                name: "IX_RollDice_FaceID",
                table: "RollDie",
                newName: "IX_RollDie_FaceID");

            migrationBuilder.RenameIndex(
                name: "IX_GearQualities_SourceID",
                table: "GearQuality",
                newName: "IX_GearQuality_SourceID");

            migrationBuilder.AlterColumn<string>(
                name: "Gear_Description",
                table: "NameAliases",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RollDie",
                table: "RollDie",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GearQuality",
                table: "GearQuality",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_GearQualitiesGear_GearQuality_GearQualityID",
                table: "GearQualitiesGear",
                column: "GearQualityID",
                principalTable: "GearQuality",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GearQuality_Source_SourceID",
                table: "GearQuality",
                column: "SourceID",
                principalTable: "Source",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RollDie_DieFaces_FaceID",
                table: "RollDie",
                column: "FaceID",
                principalTable: "DieFaces",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RollDie_Rolls_RollID",
                table: "RollDie",
                column: "RollID",
                principalTable: "Rolls",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
