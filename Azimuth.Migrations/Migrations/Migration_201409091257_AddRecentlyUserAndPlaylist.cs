using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409091257)]
    public class Migration_201409091257_AddRecentlyUserAndPlaylist : Migration
    {
        public override void Up()
        {
            Alter.Table("Notifications")
                .AddColumn("RecentlyUserId").AsInt64().Nullable().ForeignKey("Users", "Id")
                .AddColumn("RecentlyPlaylistId").AsInt64().Nullable().ForeignKey("Playlists", "PlaylistsId");
        }

        public override void Down()
        {
            Delete.Column("RecentlyUserId").FromTable("Notifications");
            Delete.Column("RecentlyPlaylistId").FromTable("Notifications");
        }
    }
}