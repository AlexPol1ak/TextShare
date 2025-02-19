using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.UI.Migrations
{
    /// <inheritdoc />
    public partial class UserAddAvatarMimeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarMimeType",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarMimeType",
                table: "AspNetUsers");
        }
    }
}
