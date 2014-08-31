using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201408291506)]
    public class Migration_201408291506_AddIdToAlbumListeners : Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_PlaylistListeners").FromTable("PlaylistListeners");
            Alter.Table("PlaylistListeners")
                .AddColumn("Id").AsInt64().NotNullable().Identity();
            Create.PrimaryKey("PK_PlaylistListeners_1")
                .OnTable("PlaylistListeners")
                .Column("Id");
        }

        public override void Down()
        {
            Delete.PrimaryKey("PK_PlaylistListeners_1").FromTable("PlaylistListeners");
            Delete.Column("Id").FromTable("PlaylistListeners");
            Create.PrimaryKey("PK_PlaylistListeners")
                .OnTable("PlaylistListeners")
                .Columns(new[] { "PlaylistId", "ListenerId" });
        }
    }
}

