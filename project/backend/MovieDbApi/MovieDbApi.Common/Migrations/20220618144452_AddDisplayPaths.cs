using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class AddDisplayPaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayPath",
                table: "ScannedPaths",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayPath",
                table: "ScannedPaths");
        }
    }
}
