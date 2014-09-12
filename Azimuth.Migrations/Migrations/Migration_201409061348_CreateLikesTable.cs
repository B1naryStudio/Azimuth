using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409061348)]
    public class Migration_201409061348_CreateLikesTable:Migration
    {
        public override void Up()
        {
            Create.Table("Likes")
                .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn("PlaylistId").AsInt64().NotNullable().ForeignKey("Playlists", "PlaylistsId")
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("Users", "Id");
        }

        public override void Down()
        {
            Delete.Table("Likes");
        }
    }
}
