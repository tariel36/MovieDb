using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class BaseScanPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT IGNORE INTO `ScannedPaths` (`Path`) SELECT '/app/ext_root' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `ScannedPaths` WHERE `Path` = '/app/ext_root' LIMIT 1);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
