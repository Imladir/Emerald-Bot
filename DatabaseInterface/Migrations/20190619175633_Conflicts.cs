using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class Conflicts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerID = table.Column<int>(nullable: true),
                    ConflictTypeID = table.Column<int>(nullable: true),
                    Round = table.Column<int>(nullable: false),
                    CurrentParticipant = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflicts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Conflicts_ConflictTypes_ConflictTypeID",
                        column: x => x.ConflictTypeID,
                        principalTable: "ConflictTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conflicts_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConflictParticipant",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CharacterID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Init = table.Column<int>(nullable: false),
                    Fatigue = table.Column<int>(nullable: false),
                    Strife = table.Column<int>(nullable: false),
                    ConflictID = table.Column<int>(nullable: true),
                    ConflictID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictParticipant", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ConflictParticipant_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictParticipant_Conflicts_ConflictID",
                        column: x => x.ConflictID,
                        principalTable: "Conflicts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictParticipant_Conflicts_ConflictID1",
                        column: x => x.ConflictID1,
                        principalTable: "Conflicts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConflictParticipant_CharacterID",
                table: "ConflictParticipant",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictParticipant_ConflictID",
                table: "ConflictParticipant",
                column: "ConflictID");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictParticipant_ConflictID1",
                table: "ConflictParticipant",
                column: "ConflictID1");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_ConflictTypeID",
                table: "Conflicts",
                column: "ConflictTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_ServerID",
                table: "Conflicts",
                column: "ServerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConflictParticipant");

            migrationBuilder.DropTable(
                name: "Conflicts");
        }
    }
}
