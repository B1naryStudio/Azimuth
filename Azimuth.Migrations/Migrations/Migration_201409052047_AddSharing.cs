using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409052047)]
    public class Migration_201409052047_AddSharing : Migration
    {
        public override void Up()
        {
            Create.Table("SharedPlaylists")
                .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn("Guid").AsString().NotNullable()
                .WithColumn("PlaylistId").AsInt64().NotNullable().ForeignKey("Playlists", "PlaylistsId");
        }

        public override void Down()
        {
            Delete.Table("SharedPlaylists");
        }
    }
}