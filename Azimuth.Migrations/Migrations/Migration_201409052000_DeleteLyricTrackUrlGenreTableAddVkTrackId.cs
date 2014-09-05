
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
            Alter.Table("Tracks").AddColumn("ThirdPartId").AsString().NotNullable();
            Delete.Table("Genres");
        }

        public override void Down()
        {
            Alter.Table("Tracks").AddColumn("Lyrics").AsString(10000).WithDefaultValue("");
            Alter.Table("Tracks").AddColumn("Url").AsString();
            Delete.Column("ThirdPartId").FromTable("Tracks");
            Create.Table("Genres")
                .WithColumn("GenreId")
                .AsInt64()
                .NotNullable()
                .PrimaryKey("GenreId")
                .WithColumn("Name")
                .AsString()
                .NotNullable();
        }
    }
}
