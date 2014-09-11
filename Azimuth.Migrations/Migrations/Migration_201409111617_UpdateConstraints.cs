using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409111617)]
    public class Migration_201409111617_UpdateConstraints : Migration
    {
        public override void Up()
        {
            Delete.UniqueConstraint("ConstraintAccess").FromTable("Playlists");
            Execute.Sql("Alter Table Playlists Add Constraint ConstraintAccess CHECK(Accessibilty IN ('public', 'private', 'shared'))");
        }

        public override void Down()
        {
            Delete.UniqueConstraint("ConstraintAccess").FromTable("Playlists");
            Execute.Sql("Alter Table Playlists Add Constraint ConstraintAccess CHECK(Accessibilty IN ('public', 'private'))");
        }
    }
}