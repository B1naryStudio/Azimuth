﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409081528)]
    public class Migration_201409081528_AlterLikesTable : Migration
    {
        public override void Up()
        {
            Alter.Table("Likes")
                .AddColumn("IsLiked").AsBoolean().NotNullable()
                .AddColumn("IsFavorite").AsBoolean().NotNullable();
            Create.UniqueConstraint("UC1")
                .OnTable("Likes")
                .Columns(new[] {"UserId", "PlaylistId"});
        }

        public override void Down()
        {
            Delete.Column("IsLiked").FromTable("Likes");
            Delete.Column("IsFavorite").FromTable("Likes");
            Delete.UniqueConstraint("UC1").FromTable("Likes");

        }
    }
}
