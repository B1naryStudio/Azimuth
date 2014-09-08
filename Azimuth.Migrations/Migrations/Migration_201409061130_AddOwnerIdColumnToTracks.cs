
using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409061130)]
    public class Migration_201409061130_AddOwnerIdColumnToTracks: Migration
    {
        public override void Up()
        {
            Alter.Table("Tracks").AddColumn("OwnerId").AsString().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("OwnerId").FromTable("Tracks");
        }
    }
}
