﻿using FluentMigrator;

namespace Azimuth.Migrations
//namespace Azimuth.Migrations.Migrations
{
    [Migration(201408111122)]
    public class Migration_201408111122_UpdateTracks : Migration
    {
        public override void Up()
        {
            Alter.Table("Tracks").AddColumn("Url").AsString().NotNullable();//.WithDefaultValue("");
            Alter.Table("Tracks").AddColumn("Name").AsString().NotNullable();//.WithDefaultValue("");
        }

        public override void Down()
        {
            Delete.Column("Url").FromTable("Tracks");
            Delete.Column("Name").FromTable("Tracks");
        }
    }
}