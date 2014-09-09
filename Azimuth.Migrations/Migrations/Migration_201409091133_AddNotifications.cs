using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201409091133)]
    public class Migration_201409091133_AddNotifications : Migration
    {
        public override void Up()
        {
            Create.Table("Notifications")
                .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                .WithColumn("NotificationType").AsString(100).NotNullable()
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("Users", "Id");
        }

        public override void Down()
        {
            Delete.Table("Notifications");
        }
    }
}