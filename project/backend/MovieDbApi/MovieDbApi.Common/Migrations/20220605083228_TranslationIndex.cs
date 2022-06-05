using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class TranslationIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceHash",
                table: "TranslationCache",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationCache_Language_SourceHash",
                table: "TranslationCache",
                columns: new[] { "Language", "SourceHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TranslationCache_SourceHash",
                table: "TranslationCache",
                column: "SourceHash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TranslationCache_Language_SourceHash",
                table: "TranslationCache");

            migrationBuilder.DropIndex(
                name: "IX_TranslationCache_SourceHash",
                table: "TranslationCache");

            migrationBuilder.DropColumn(
                name: "SourceHash",
                table: "TranslationCache");
        }
    }
}
