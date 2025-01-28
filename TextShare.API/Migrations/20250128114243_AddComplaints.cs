using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplaintReasons",
                columns: table => new
                {
                    ComplaintReasonsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintReasons", x => x.ComplaintReasonsId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TextFileId = table.Column<int>(type: "int", nullable: false),
                    ComplaintReasonsId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_Complaints_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaints_ComplaintReasons_ComplaintReasonsId",
                        column: x => x.ComplaintReasonsId,
                        principalTable: "ComplaintReasons",
                        principalColumn: "ComplaintReasonsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaints_TextFiles_TextFileId",
                        column: x => x.TextFileId,
                        principalTable: "TextFiles",
                        principalColumn: "TextFileId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AuthorId",
                table: "Complaints",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ComplaintReasonsId",
                table: "Complaints",
                column: "ComplaintReasonsId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TextFileId",
                table: "Complaints",
                column: "TextFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "ComplaintReasons");
        }
    }
}
