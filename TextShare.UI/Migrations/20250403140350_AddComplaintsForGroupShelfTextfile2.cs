using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.UI.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintsForGroupShelfTextfile2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UniqueFileName",
                table: "TextFiles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "TextFiles",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TextFileId",
                table: "Complaints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_GroupId",
                table: "Complaints",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ShelfId",
                table: "Complaints",
                column: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Groups_GroupId",
                table: "Complaints",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Shelves_ShelfId",
                table: "Complaints",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "ShelfId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Groups_GroupId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Shelves_ShelfId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_GroupId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_ShelfId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "Complaints");

            migrationBuilder.AlterColumn<string>(
                name: "UniqueFileName",
                table: "TextFiles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                table: "TextFiles",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TextFileId",
                table: "Complaints",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
