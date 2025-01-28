using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessRuleId",
                table: "TextFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccessRules",
                columns: table => new
                {
                    AccessRuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AvailableAll = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TextFileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRules", x => x.AccessRuleId);
                    table.ForeignKey(
                        name: "FK_AccessRules_TextFiles_TextFileId",
                        column: x => x.TextFileId,
                        principalTable: "TextFiles",
                        principalColumn: "TextFileId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccessRuleGroups",
                columns: table => new
                {
                    AccessRulesAccessRuleId = table.Column<int>(type: "int", nullable: false),
                    AvailableGroupsGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRuleGroups", x => new { x.AccessRulesAccessRuleId, x.AvailableGroupsGroupId });
                    table.ForeignKey(
                        name: "FK_AccessRuleGroups_AccessRules_AccessRulesAccessRuleId",
                        column: x => x.AccessRulesAccessRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "AccessRuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRuleGroups_Groups_AvailableGroupsGroupId",
                        column: x => x.AvailableGroupsGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccessRuleUsers",
                columns: table => new
                {
                    AccessRulesAccessRuleId = table.Column<int>(type: "int", nullable: false),
                    AvailableUsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRuleUsers", x => new { x.AccessRulesAccessRuleId, x.AvailableUsersId });
                    table.ForeignKey(
                        name: "FK_AccessRuleUsers_AccessRules_AccessRulesAccessRuleId",
                        column: x => x.AccessRulesAccessRuleId,
                        principalTable: "AccessRules",
                        principalColumn: "AccessRuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRuleUsers_AspNetUsers_AvailableUsersId",
                        column: x => x.AvailableUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRuleGroups_AvailableGroupsGroupId",
                table: "AccessRuleGroups",
                column: "AvailableGroupsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRules_TextFileId",
                table: "AccessRules",
                column: "TextFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccessRuleUsers_AvailableUsersId",
                table: "AccessRuleUsers",
                column: "AvailableUsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessRuleGroups");

            migrationBuilder.DropTable(
                name: "AccessRuleUsers");

            migrationBuilder.DropTable(
                name: "AccessRules");

            migrationBuilder.DropColumn(
                name: "AccessRuleId",
                table: "TextFiles");
        }
    }
}
