using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDbApi.Common.Migrations
{
    public partial class Values : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT IGNORE INTO `TranslationCache` (`Language`,`Source`,`Target`) SELECT 'pl','Genre','Gatunek' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `TranslationCache` WHERE `Language` = 'pl' AND `Source` = 'Genre' AND `Target` = 'Gatunek' LIMIT 1);");
            migrationBuilder.Sql(@"INSERT IGNORE INTO `TranslationCache` (`Language`,`Source`,`Target`) SELECT 'pl','Staff','Obsada' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `TranslationCache` WHERE `Language` = 'pl' AND `Source` = 'Staff' AND `Target` = 'Obsada' LIMIT 1);");
            migrationBuilder.Sql(@"INSERT IGNORE INTO `TranslationCache` (`Language`,`Source`,`Target`) SELECT 'pl','Media type','Typ' FROM DUAL WHERE NOT EXISTS (SELECT * FROM `TranslationCache` WHERE `Language` = 'pl' AND `Source` = 'Media type' AND `Target` = 'Typ' LIMIT 1);");

            migrationBuilder.Sql(@"UPDATE `TranslationCache` SET `Target` = 'Gatunek' WHERE `TranslationCache`.`Language` = 'pl' AND `TranslationCache`.`Source` = 'Genre';");
            migrationBuilder.Sql(@"UPDATE `TranslationCache` SET `Target` = 'Obsada' WHERE `TranslationCache`.`Language` = 'pl' AND `TranslationCache`.`Source` = 'Staff';");
            migrationBuilder.Sql(@"UPDATE `TranslationCache` SET `Target` = 'Typ' WHERE `TranslationCache`.`Language` = 'pl' AND `TranslationCache`.`Source` = 'Media type';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
