
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409162000)]
    public class Migration_201409162000_AddingDateTimeToNotifications: Migration
    {
        public override void Up()
        {
            Alter.Table("Notifications").AddColumn("NotificationDate").AsDateTime().WithDefaultValue("");
        }

        public override void Down()
        {
            Delete.Column("NotificationDate").FromTable("Notifications");
        }
    }
}
