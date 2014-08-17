using FluentMigrator;

namespace Azimuth.Migrations
//namespace Azimuth.Migrations.Migrations
{
    [Migration(201408141633)]
    public class Migration_201408141633_IncreaseLyricsField : Migration
    {
        public override void Up()
        {
            Alter.Table("Tracks").AlterColumn("Lyrics").AsString(10000).Nullable();
        }

        public override void Down()
        {
            Alter.Table("Tracks").AlterColumn("Lyrics").AsString(500).Nullable();
        }
    }
}
