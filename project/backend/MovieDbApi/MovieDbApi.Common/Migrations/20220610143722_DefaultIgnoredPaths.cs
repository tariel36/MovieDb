using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class DefaultIgnoredPaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT IGNORE INTO `IgnoredPaths` (`Path`) SELECT 'Extras' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `IgnoredPaths` WHERE `Path` = 'Extras' LIMIT 1);");
            migrationBuilder.Sql(@"INSERT IGNORE INTO `IgnoredPaths` (`Path`) SELECT 'Extra' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `IgnoredPaths` WHERE `Path` = 'Extra' LIMIT 1);");
            migrationBuilder.Sql(@"INSERT IGNORE INTO `IgnoredPaths` (`Path`) SELECT 'Bonus' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `IgnoredPaths` WHERE `Path` = 'Bonus' LIMIT 1);");
            migrationBuilder.Sql(@"INSERT IGNORE INTO `IgnoredPaths` (`Path`) SELECT 'BD MENU' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `IgnoredPaths` WHERE `Path` = 'BD MENU' LIMIT 1);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
