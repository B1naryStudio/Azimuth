
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409052000)]
    public class Migration_201409052000_DeleteLyricTrackUrlGenreAddVkTrackId : Migration
    {
        public override void Up()
        {
            Delete.Column("Lyrics").FromTable("Tracks");
            Delete.Column("Url").FromTable("Tracks");
            Delete.Column("Genre").FromTable("Tracks");
            Alter.Table("Tracks").AddColumn("ThirdPartId").AsString().NotNullable().WithDefaultValue("");
        }

        public override void Down()
        {
            Alter.Table("Tracks").AddColumn("Lyrics").AsString(10000).WithDefaultValue("");
            Alter.Table("Tracks").AddColumn("Url").AsString().WithDefaultValue("");
            Alter.Table("Tracks").AddColumn("Genre").AsString().WithDefaultValue("");
            Delete.Column("ThirdPartId").FromTable("Tracks");
        }
    }
}
