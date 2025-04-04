﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TextShare.UI.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldTextFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueFileNameWithoutExtension",
                table: "TextFiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueFileNameWithoutExtension",
                table: "TextFiles");
        }
    }
}
