﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201408131104)]
    public class Migration_201408131104_CreateUnauthorizedListenersTable : Migration
    {
        public override void Up()
        {
            Create.Table("UnauthorizedListeners")
                .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn("PlaylistId").AsInt64().NotNullable().ForeignKey("Playlists", "PlaylistsId")
                .WithColumn("Amount").AsInt64().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("UnauthorizedListeners");
        }
    }
}
