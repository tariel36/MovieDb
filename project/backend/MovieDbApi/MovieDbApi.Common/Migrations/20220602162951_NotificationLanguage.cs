using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class NotificationLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Subscribers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Subscribers");
        }
    }
}
