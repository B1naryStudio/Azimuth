using FluentMigrator;

namespace Azimuth.Migrations
//namespace Azimuth.Migrations.Migrations
{
    [Migration(201409010308)]
    public class Migration_201409010308_AddFollowers : Migration
    {
        public override void Up()
        {
            Create.Table("UserToUser")
                .WithColumn("FollowerId")
                .AsInt64()
                .NotNullable()
                .ForeignKey("Users", "Id")
                .WithColumn("FollowingId")
                .AsInt64()
                .NotNullable()
                .ForeignKey("Users", "Id");
        }

        public override void Down()
        {
            Delete.Table("UserToUser");
        }
    }
}