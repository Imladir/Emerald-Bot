using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmeraldBot.Model.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Activation = table.Column<string>(maxLength: 1024, nullable: true),
                    Effect = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AdvantageClasses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvantageClasses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AdvantageTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvantageTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Clans",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    Colour = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clans", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ConflictTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Demeanors",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Effect = table.Column<string>(maxLength: 64, nullable: false),
                    Unmasking = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demeanors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JournalTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NPCTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: true),
                    LoginToken = table.Column<string>(maxLength: 64, nullable: true),
                    Verbose = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SceneTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SceneTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(maxLength: 1, nullable: true),
                    DiceChannelID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SkillGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Book = table.Column<string>(maxLength: 64, nullable: false),
                    Page = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TechniqueTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechniqueTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WeaponGrips",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Hands = table.Column<int>(nullable: false),
                    NewRangeMin = table.Column<int>(nullable: false),
                    NewRangeMax = table.Column<int>(nullable: false),
                    DamageModificator = table.Column<int>(nullable: false),
                    DeadlinessModificator = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponGrips", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ActionTypeAction",
                columns: table => new
                {
                    ActionTypeID = table.Column<int>(nullable: false),
                    ActionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypeAction", x => new { x.ActionTypeID, x.ActionID });
                    table.ForeignKey(
                        name: "FK_ActionTypeAction_Actions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "Actions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionTypeAction_ActionTypes_ActionTypeID",
                        column: x => x.ActionTypeID,
                        principalTable: "ActionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConflictAction",
                columns: table => new
                {
                    ConflictID = table.Column<int>(nullable: false),
                    ActionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictAction", x => new { x.ConflictID, x.ActionID });
                    table.ForeignKey(
                        name: "FK_ConflictAction_Actions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "Actions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictAction_ConflictTypes_ConflictID",
                        column: x => x.ConflictID,
                        principalTable: "ConflictTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionScenes",
                columns: table => new
                {
                    ActionID = table.Column<int>(nullable: false),
                    SceneTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionScenes", x => new { x.ActionID, x.SceneTypeID });
                    table.ForeignKey(
                        name: "FK_ActionScenes_Actions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "Actions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionScenes_SceneTypes_SceneTypeID",
                        column: x => x.SceneTypeID,
                        principalTable: "SceneTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Emotes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Text = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Emotes_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GMs",
                columns: table => new
                {
                    ServerID = table.Column<int>(nullable: false),
                    PlayerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GMs", x => new { x.ServerID, x.PlayerID });
                    table.ForeignKey(
                        name: "FK_GMs_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GMs_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerID = table.Column<int>(nullable: false),
                    PlayerID = table.Column<int>(nullable: false),
                    DiscordChannelID = table.Column<long>(nullable: false),
                    DiscordMessageID = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrivateChannels",
                columns: table => new
                {
                    PlayerID = table.Column<int>(nullable: false),
                    ServerID = table.Column<int>(nullable: false),
                    ChannelDiscordID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateChannels", x => new { x.PlayerID, x.ServerID });
                    table.ForeignKey(
                        name: "FK_PrivateChannels_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateChannels_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GearQuality",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SourceID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearQuality", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GearQuality_Source_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NameAliases",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alias = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    ServerID = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ClanID = table.Column<int>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Fatigue = table.Column<int>(nullable: true),
                    Strife = table.Column<int>(nullable: true),
                    Endurance = table.Column<int>(nullable: true),
                    Composure = table.Column<int>(nullable: true),
                    Focus = table.Column<int>(nullable: true),
                    Vigilance = table.Column<int>(nullable: true),
                    DemeanorID = table.Column<int>(nullable: true),
                    NPCTypeID = table.Column<int>(nullable: true),
                    Honour = table.Column<int>(nullable: true),
                    Glory = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    Ability = table.Column<string>(nullable: true),
                    SourceID = table.Column<int>(nullable: true),
                    PlayerID = table.Column<int>(nullable: true),
                    Family = table.Column<string>(nullable: true),
                    School = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: true),
                    Age = table.Column<int>(nullable: true),
                    Ninjo = table.Column<string>(maxLength: 256, nullable: true),
                    Giri = table.Column<string>(maxLength: 256, nullable: true),
                    CurrentVoid = table.Column<int>(nullable: true),
                    Condition_Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Effect = table.Column<string>(nullable: true),
                    RemovedWhen = table.Column<string>(nullable: true),
                    AdvantageClassID = table.Column<int>(nullable: true),
                    RingID = table.Column<int>(nullable: true),
                    PermanentEffect = table.Column<string>(maxLength: 1024, nullable: true),
                    RollEffect = table.Column<string>(maxLength: 1024, nullable: true),
                    Advantage_SourceID = table.Column<int>(nullable: true),
                    Gear_SourceID = table.Column<int>(nullable: true),
                    Gear_Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Physical = table.Column<int>(nullable: true),
                    Spiritual = table.Column<int>(nullable: true),
                    WeaponTypeID = table.Column<int>(nullable: true),
                    RangeMin = table.Column<int>(nullable: true),
                    RangeMax = table.Column<int>(nullable: true),
                    Damage = table.Column<int>(nullable: true),
                    Deadliness = table.Column<int>(nullable: true),
                    GroupID = table.Column<int>(nullable: true),
                    Skill_SourceID = table.Column<int>(nullable: true),
                    TypeID = table.Column<int>(nullable: true),
                    Technique_Rank = table.Column<int>(nullable: true),
                    Technique_RingID = table.Column<int>(nullable: true),
                    TN = table.Column<int>(nullable: true),
                    Technique_SourceID = table.Column<int>(nullable: true),
                    Activation = table.Column<string>(nullable: true),
                    Technique_Effect = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameAliases", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NameAliases_Clans_ClanID",
                        column: x => x.ClanID,
                        principalTable: "Clans",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Demeanors_DemeanorID",
                        column: x => x.DemeanorID,
                        principalTable: "Demeanors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_NPCTypes_NPCTypeID",
                        column: x => x.NPCTypeID,
                        principalTable: "NPCTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Source_SourceID",
                        column: x => x.SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_AdvantageClasses_AdvantageClassID",
                        column: x => x.AdvantageClassID,
                        principalTable: "AdvantageClasses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Rings_RingID",
                        column: x => x.RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Source_Advantage_SourceID",
                        column: x => x.Advantage_SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Source_Gear_SourceID",
                        column: x => x.Gear_SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_SkillGroups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "SkillGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Source_Skill_SourceID",
                        column: x => x.Skill_SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Rings_Technique_RingID",
                        column: x => x.Technique_RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Source_Technique_SourceID",
                        column: x => x.Technique_SourceID,
                        principalTable: "Source",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_TechniqueTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "TechniqueTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_WeaponTypes_WeaponTypeID",
                        column: x => x.WeaponTypeID,
                        principalTable: "WeaponTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NameAliases_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DieFaces",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DieType = table.Column<string>(nullable: false),
                    Value = table.Column<string>(maxLength: 2, nullable: true),
                    EmoteID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieFaces", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DieFaces_Emotes_EmoteID",
                        column: x => x.EmoteID,
                        principalTable: "Emotes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvantageTypeAdvantages",
                columns: table => new
                {
                    AdvantageTypeID = table.Column<int>(nullable: false),
                    AdvantageID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvantageTypeAdvantages", x => new { x.AdvantageTypeID, x.AdvantageID });
                    table.ForeignKey(
                        name: "FK_AdvantageTypeAdvantages_NameAliases_AdvantageID",
                        column: x => x.AdvantageID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdvantageTypeAdvantages_AdvantageTypes_AdvantageTypeID",
                        column: x => x.AdvantageTypeID,
                        principalTable: "AdvantageTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterConditions",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    ConditionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterConditions", x => new { x.CharacterID, x.ConditionID });
                    table.ForeignKey(
                        name: "FK_CharacterConditions_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterConditions_NameAliases_ConditionID",
                        column: x => x.ConditionID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterRings",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    RingID = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRings", x => new { x.CharacterID, x.RingID });
                    table.ForeignKey(
                        name: "FK_CharacterRings_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterRings_Rings_RingID",
                        column: x => x.RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkillGroups",
                columns: table => new
                {
                    NPCID = table.Column<int>(nullable: false),
                    SkillGroupID = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkillGroups", x => new { x.NPCID, x.SkillGroupID });
                    table.ForeignKey(
                        name: "FK_CharacterSkillGroups_NameAliases_NPCID",
                        column: x => x.NPCID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterSkillGroups_SkillGroups_SkillGroupID",
                        column: x => x.SkillGroupID,
                        principalTable: "SkillGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkills",
                columns: table => new
                {
                    PCID = table.Column<int>(nullable: false),
                    SkillID = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkills", x => new { x.PCID, x.SkillID });
                    table.ForeignKey(
                        name: "FK_CharacterSkills_NameAliases_PCID",
                        column: x => x.PCID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterSkills_NameAliases_SkillID",
                        column: x => x.SkillID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DefaultCharacter",
                columns: table => new
                {
                    ServerID = table.Column<int>(nullable: false),
                    PlayerID = table.Column<int>(nullable: false),
                    CharacterID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultCharacter", x => new { x.ServerID, x.PlayerID });
                    table.ForeignKey(
                        name: "FK_DefaultCharacter_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultCharacter_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DefaultCharacter_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GearQualitiesGear",
                columns: table => new
                {
                    GearID = table.Column<int>(nullable: false),
                    GearQualityID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearQualitiesGear", x => new { x.GearID, x.GearQualityID });
                    table.ForeignKey(
                        name: "FK_GearQualitiesGear_NameAliases_GearID",
                        column: x => x.GearID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GearQualitiesGear_GearQuality_GearQualityID",
                        column: x => x.GearQualityID,
                        principalTable: "GearQuality",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntry",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntryDate = table.Column<DateTime>(nullable: false),
                    JournalID = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(maxLength: 1024, nullable: false),
                    PCID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_JournalEntry_JournalTypes_JournalID",
                        column: x => x.JournalID,
                        principalTable: "JournalTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntry_NameAliases_PCID",
                        column: x => x.PCID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerID = table.Column<int>(nullable: false),
                    SourceID = table.Column<int>(nullable: true),
                    Variable = table.Column<bool>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Effect = table.Column<string>(maxLength: 1024, nullable: false),
                    RingID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Opportunities_Rings_RingID",
                        column: x => x.RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunities_NameAliases_SourceID",
                        column: x => x.SourceID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityTriggers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RingID = table.Column<int>(nullable: false),
                    SkillGroupID = table.Column<int>(nullable: true),
                    TechniqueTypeID = table.Column<int>(nullable: true),
                    TechniqueID = table.Column<int>(nullable: true),
                    SceneTypeID = table.Column<int>(nullable: true),
                    ActionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityTriggers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_Actions_ActionID",
                        column: x => x.ActionID,
                        principalTable: "Actions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_Rings_RingID",
                        column: x => x.RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_SceneTypes_SceneTypeID",
                        column: x => x.SceneTypeID,
                        principalTable: "SceneTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_SkillGroups_SkillGroupID",
                        column: x => x.SkillGroupID,
                        principalTable: "SkillGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_NameAliases_TechniqueID",
                        column: x => x.TechniqueID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggers_TechniqueTypes_TechniqueTypeID",
                        column: x => x.TechniqueTypeID,
                        principalTable: "TechniqueTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCAdvantage",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    AdvantageID = table.Column<int>(nullable: false),
                    PCID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCAdvantage", x => new { x.CharacterID, x.AdvantageID });
                    table.ForeignKey(
                        name: "FK_PCAdvantage_NameAliases_AdvantageID",
                        column: x => x.AdvantageID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCAdvantage_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCAdvantage_NameAliases_PCID",
                        column: x => x.PCID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PCTechnique",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    TechniqueID = table.Column<int>(nullable: false),
                    PCID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCTechnique", x => new { x.CharacterID, x.TechniqueID });
                    table.ForeignKey(
                        name: "FK_PCTechnique_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCTechnique_NameAliases_PCID",
                        column: x => x.PCID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PCTechnique_NameAliases_TechniqueID",
                        column: x => x.TechniqueID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rolls",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServerID = table.Column<int>(nullable: false),
                    PlayerID = table.Column<int>(nullable: false),
                    CharacterID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    TechniqueID = table.Column<int>(nullable: true),
                    SkillID = table.Column<int>(nullable: true),
                    RingID = table.Column<int>(nullable: true),
                    TN = table.Column<int>(nullable: false),
                    Locked = table.Column<bool>(nullable: false),
                    DiscordChannelID = table.Column<long>(nullable: false),
                    DiscordMessageID = table.Column<long>(nullable: false),
                    Log = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rolls", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rolls_NameAliases_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rolls_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rolls_Rings_RingID",
                        column: x => x.RingID,
                        principalTable: "Rings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rolls_Servers_ServerID",
                        column: x => x.ServerID,
                        principalTable: "Servers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rolls_NameAliases_SkillID",
                        column: x => x.SkillID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rolls_NameAliases_TechniqueID",
                        column: x => x.TechniqueID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechniqueSkillGroups",
                columns: table => new
                {
                    TechniqueID = table.Column<int>(nullable: false),
                    SkillGroupID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechniqueSkillGroups", x => new { x.TechniqueID, x.SkillGroupID });
                    table.ForeignKey(
                        name: "FK_TechniqueSkillGroups_SkillGroups_SkillGroupID",
                        column: x => x.SkillGroupID,
                        principalTable: "SkillGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechniqueSkillGroups_NameAliases_TechniqueID",
                        column: x => x.TechniqueID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechniquesSkills",
                columns: table => new
                {
                    TechniqueID = table.Column<int>(nullable: false),
                    SkillID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechniquesSkills", x => new { x.TechniqueID, x.SkillID });
                    table.ForeignKey(
                        name: "FK_TechniquesSkills_NameAliases_SkillID",
                        column: x => x.SkillID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechniquesSkills_NameAliases_TechniqueID",
                        column: x => x.TechniqueID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WeaponGripsWeapon",
                columns: table => new
                {
                    WeaponID = table.Column<int>(nullable: false),
                    WeaponGripID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponGripsWeapon", x => new { x.WeaponID, x.WeaponGripID });
                    table.ForeignKey(
                        name: "FK_WeaponGripsWeapon_WeaponGrips_WeaponGripID",
                        column: x => x.WeaponGripID,
                        principalTable: "WeaponGrips",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WeaponGripsWeapon_NameAliases_WeaponID",
                        column: x => x.WeaponID,
                        principalTable: "NameAliases",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityTriggerOpportunity",
                columns: table => new
                {
                    OpportunityID = table.Column<int>(nullable: false),
                    OpportunityTriggerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityTriggerOpportunity", x => new { x.OpportunityID, x.OpportunityTriggerID });
                    table.ForeignKey(
                        name: "FK_OpportunityTriggerOpportunity_Opportunities_OpportunityID",
                        column: x => x.OpportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityTriggerOpportunity_OpportunityTriggers_OpportunityTriggerID",
                        column: x => x.OpportunityTriggerID,
                        principalTable: "OpportunityTriggers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RollDie",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FaceID = table.Column<int>(nullable: true),
                    Exploded = table.Column<bool>(nullable: false),
                    RollID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollDie", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RollDie_DieFaces_FaceID",
                        column: x => x.FaceID,
                        principalTable: "DieFaces",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RollDie_Rolls_RollID",
                        column: x => x.RollID,
                        principalTable: "Rolls",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionScenes_SceneTypeID",
                table: "ActionScenes",
                column: "SceneTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTypeAction_ActionID",
                table: "ActionTypeAction",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_AdvantageTypeAdvantages_AdvantageID",
                table: "AdvantageTypeAdvantages",
                column: "AdvantageID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterConditions_ConditionID",
                table: "CharacterConditions",
                column: "ConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterRings_RingID",
                table: "CharacterRings",
                column: "RingID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkillGroups_SkillGroupID",
                table: "CharacterSkillGroups",
                column: "SkillGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkills_SkillID",
                table: "CharacterSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictAction_ActionID",
                table: "ConflictAction",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultCharacter_CharacterID",
                table: "DefaultCharacter",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultCharacter_PlayerID",
                table: "DefaultCharacter",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_DieFaces_EmoteID",
                table: "DieFaces",
                column: "EmoteID");

            migrationBuilder.CreateIndex(
                name: "IX_Emotes_ServerID",
                table: "Emotes",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_GearQualitiesGear_GearQualityID",
                table: "GearQualitiesGear",
                column: "GearQualityID");

            migrationBuilder.CreateIndex(
                name: "IX_GearQuality_SourceID",
                table: "GearQuality",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_GMs_PlayerID",
                table: "GMs",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_JournalID",
                table: "JournalEntry",
                column: "JournalID");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_PCID",
                table: "JournalEntry",
                column: "PCID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PlayerID",
                table: "Messages",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ServerID",
                table: "Messages",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_ClanID",
                table: "NameAliases",
                column: "ClanID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_DemeanorID",
                table: "NameAliases",
                column: "DemeanorID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_NPCTypeID",
                table: "NameAliases",
                column: "NPCTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_SourceID",
                table: "NameAliases",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_PlayerID",
                table: "NameAliases",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_AdvantageClassID",
                table: "NameAliases",
                column: "AdvantageClassID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_RingID",
                table: "NameAliases",
                column: "RingID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_Advantage_SourceID",
                table: "NameAliases",
                column: "Advantage_SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_Gear_SourceID",
                table: "NameAliases",
                column: "Gear_SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_GroupID",
                table: "NameAliases",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_Skill_SourceID",
                table: "NameAliases",
                column: "Skill_SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_Technique_RingID",
                table: "NameAliases",
                column: "Technique_RingID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_Technique_SourceID",
                table: "NameAliases",
                column: "Technique_SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_TypeID",
                table: "NameAliases",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_WeaponTypeID",
                table: "NameAliases",
                column: "WeaponTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_NameAliases_ServerID",
                table: "NameAliases",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_RingID",
                table: "Opportunities",
                column: "RingID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ServerID",
                table: "Opportunities",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_SourceID",
                table: "Opportunities",
                column: "SourceID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggerOpportunity_OpportunityTriggerID",
                table: "OpportunityTriggerOpportunity",
                column: "OpportunityTriggerID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_ActionID",
                table: "OpportunityTriggers",
                column: "ActionID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_RingID",
                table: "OpportunityTriggers",
                column: "RingID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_SceneTypeID",
                table: "OpportunityTriggers",
                column: "SceneTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_SkillGroupID",
                table: "OpportunityTriggers",
                column: "SkillGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_TechniqueID",
                table: "OpportunityTriggers",
                column: "TechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityTriggers_TechniqueTypeID",
                table: "OpportunityTriggers",
                column: "TechniqueTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PCAdvantage_AdvantageID",
                table: "PCAdvantage",
                column: "AdvantageID");

            migrationBuilder.CreateIndex(
                name: "IX_PCAdvantage_PCID",
                table: "PCAdvantage",
                column: "PCID");

            migrationBuilder.CreateIndex(
                name: "IX_PCTechnique_PCID",
                table: "PCTechnique",
                column: "PCID");

            migrationBuilder.CreateIndex(
                name: "IX_PCTechnique_TechniqueID",
                table: "PCTechnique",
                column: "TechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateChannels_ServerID",
                table: "PrivateChannels",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_RollDie_FaceID",
                table: "RollDie",
                column: "FaceID");

            migrationBuilder.CreateIndex(
                name: "IX_RollDie_RollID",
                table: "RollDie",
                column: "RollID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_CharacterID",
                table: "Rolls",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_PlayerID",
                table: "Rolls",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_RingID",
                table: "Rolls",
                column: "RingID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_ServerID",
                table: "Rolls",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_SkillID",
                table: "Rolls",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_Rolls_TechniqueID",
                table: "Rolls",
                column: "TechniqueID");

            migrationBuilder.CreateIndex(
                name: "IX_TechniqueSkillGroups_SkillGroupID",
                table: "TechniqueSkillGroups",
                column: "SkillGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TechniquesSkills_SkillID",
                table: "TechniquesSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponGripsWeapon_WeaponGripID",
                table: "WeaponGripsWeapon",
                column: "WeaponGripID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionScenes");

            migrationBuilder.DropTable(
                name: "ActionTypeAction");

            migrationBuilder.DropTable(
                name: "AdvantageTypeAdvantages");

            migrationBuilder.DropTable(
                name: "CharacterConditions");

            migrationBuilder.DropTable(
                name: "CharacterRings");

            migrationBuilder.DropTable(
                name: "CharacterSkillGroups");

            migrationBuilder.DropTable(
                name: "CharacterSkills");

            migrationBuilder.DropTable(
                name: "ConflictAction");

            migrationBuilder.DropTable(
                name: "DefaultCharacter");

            migrationBuilder.DropTable(
                name: "GearQualitiesGear");

            migrationBuilder.DropTable(
                name: "GMs");

            migrationBuilder.DropTable(
                name: "JournalEntry");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "OpportunityTriggerOpportunity");

            migrationBuilder.DropTable(
                name: "PCAdvantage");

            migrationBuilder.DropTable(
                name: "PCTechnique");

            migrationBuilder.DropTable(
                name: "PrivateChannels");

            migrationBuilder.DropTable(
                name: "RollDie");

            migrationBuilder.DropTable(
                name: "TechniqueSkillGroups");

            migrationBuilder.DropTable(
                name: "TechniquesSkills");

            migrationBuilder.DropTable(
                name: "WeaponGripsWeapon");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropTable(
                name: "AdvantageTypes");

            migrationBuilder.DropTable(
                name: "ConflictTypes");

            migrationBuilder.DropTable(
                name: "GearQuality");

            migrationBuilder.DropTable(
                name: "JournalTypes");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "OpportunityTriggers");

            migrationBuilder.DropTable(
                name: "DieFaces");

            migrationBuilder.DropTable(
                name: "Rolls");

            migrationBuilder.DropTable(
                name: "WeaponGrips");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "SceneTypes");

            migrationBuilder.DropTable(
                name: "Emotes");

            migrationBuilder.DropTable(
                name: "NameAliases");

            migrationBuilder.DropTable(
                name: "Clans");

            migrationBuilder.DropTable(
                name: "Demeanors");

            migrationBuilder.DropTable(
                name: "NPCTypes");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "AdvantageClasses");

            migrationBuilder.DropTable(
                name: "Rings");

            migrationBuilder.DropTable(
                name: "SkillGroups");

            migrationBuilder.DropTable(
                name: "TechniqueTypes");

            migrationBuilder.DropTable(
                name: "WeaponTypes");

            migrationBuilder.DropTable(
                name: "Servers");
        }
    }
}
