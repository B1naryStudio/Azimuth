using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201408281315)]
    public class Migration_201408281315_AddTrackPositionColumn : Migration
    {
        public override void Up()
        {
            //Delete.PrimaryKey("PK_PlaylistTrack_1").FromTable("UserSocialNetworks");
            //Create.PrimaryKey("PK_PlaylistTrack_2")
            //    .OnTable("PlaylistTracks")
            //    .Columns(new string[] {"PlaylistId", "TrackId"});
            Alter.Table("PlaylistTracks").AddColumn("TrackPosition").AsInt32().NotNullable().WithDefaultValue(-1);
        }

        public override void Down()
        {
            //Delete.PrimaryKey("PK_PlaylistTrack_2").FromTable("PlaylistTracks");
            Delete.Column("TrackPosition").FromTable("PlaylistTracks");
        }
    }
}
