using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201408092015)]
    public class Migration_201408092015_FillUpSocialNetworkTable : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("SocialNetworks").Row(new { Name = "Vkontakte" });
            Insert.IntoTable("SocialNetworks").Row(new { Name = "Google" });
            Insert.IntoTable("SocialNetworks").Row(new { Name = "Twitter" });
            Insert.IntoTable("SocialNetworks").Row(new { Name = "Facebook" });
        }

        public override void Down()
        {
            Delete.FromTable("SocialNetworks").AllRows();
        }
    }
}