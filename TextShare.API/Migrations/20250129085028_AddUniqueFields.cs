using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TextFiles_UniqueName",
                table: "TextFiles",
                column: "UniqueFileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintReasons_Name",
                table: "ComplaintReasons",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TextFiles_UniqueName",
                table: "TextFiles");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintReasons_Name",
                table: "ComplaintReasons");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");
        }
    }
}
