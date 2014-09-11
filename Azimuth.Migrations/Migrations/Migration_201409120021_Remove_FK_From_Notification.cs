using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409120021)]
    public class Migration_201409120021_Remove_FK_From_Notification : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_Notifications_RecentlyUserId_Users_Id").OnTable("Notifications");
            Delete.ForeignKey("FK_Notifications_RecentlyPlaylistId_Playlists_PlaylistsId").OnTable("Notifications");
        }

        public override void Down()
        {
            Alter.Table("Notifications")
                .AlterColumn("RecentlyUserId").AsInt64().Nullable().ForeignKey("Users", "Id")
                .AlterColumn("RecentlyPlaylistId").AsInt64().Nullable().ForeignKey("Playlists", "PlaylistsId");
        }
    }
}