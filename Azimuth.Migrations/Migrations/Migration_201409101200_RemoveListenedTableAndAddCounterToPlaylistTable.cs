﻿
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409101200)]
    public class Migration_201409101200_RemoveListenedTableAndAddCounterToPlaylistTable: Migration
    {
        public override void Up()
        {
            if (Schema.Table("Listened").Exists())
            {
                Delete.Table("Listened");   
            }
            Alter.Table("Playlists").AddColumn("Listened").AsInt64().WithDefaultValue(0);
        }

        public override void Down()
        {
            Create.Table("Listened")
                .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn("PlaylistId").AsInt64().NotNullable().ForeignKey("Playlists", "PlaylistsId")
                .WithColumn("Amount").AsInt64().NotNullable();
            Delete.Column("Listened").FromTable("Playlists");
        }
    }
}
