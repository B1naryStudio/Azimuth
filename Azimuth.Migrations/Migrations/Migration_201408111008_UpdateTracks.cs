using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201408111008)]
    public class Migration_201408111008_UpdateTracks : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_Tracks_Genres").OnTable("Tracks");
            Delete.Column("GenreId").FromTable("Tracks");
            Delete.ForeignKey("FK_Tracks_Artists").OnTable("Tracks");
            Delete.Column("ArtistId").FromTable("Tracks");
            
            Alter.Table("Tracks").AddColumn("Genre").AsString().NotNullable();
        }

        public override void Down()
        {
            Alter.Table("Tracks").AddColumn("GenreId").AsInt32().NotNullable().ForeignKey("FK_Tracks_Genres","Genres","GenreId");
            Alter.Table("Tracks").AddColumn("ArtistId").AsInt64().NotNullable().ForeignKey("FK_Tracks_Artists", "Artists", "ArtistId"); ;
            Delete.Column("Genre").FromTable("Tracks");
        }
    }
}