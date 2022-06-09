using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class MainImageChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "MediaItems");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "MediaItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MediaItems_ImageId",
                table: "MediaItems",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaItems_MediaItemImages_ImageId",
                table: "MediaItems",
                column: "ImageId",
                principalTable: "MediaItemImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaItems_MediaItemImages_ImageId",
                table: "MediaItems");

            migrationBuilder.DropIndex(
                name: "IX_MediaItems_ImageId",
                table: "MediaItems");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "MediaItems");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "MediaItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
