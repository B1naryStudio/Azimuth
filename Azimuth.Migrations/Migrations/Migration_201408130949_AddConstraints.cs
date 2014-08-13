using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201408130949)]
    public class Migration_201408130949_AddConstraints : Migration
    {
        public override void Up()
        {
            Execute.Sql("Alter Table PLaylists Add Constraint ConstraintAccess CHECK(Accessibilty IN ('public', 'private'))");
        }

        public override void Down()
        {
            Delete.UniqueConstraint("ConstraintAccess").FromTable("Playlists");
        }
    }
}