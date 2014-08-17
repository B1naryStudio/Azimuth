
using FluentMigrator;

namespace Azimuth.Migrations
//namespace Azimuth.Migrations.Migrations
{
    [Migration(201408121200)]
    public class Migration_201408121200_ChangeUserSocialNetworksTable: Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK_UserSocialNetwork_1").FromTable("UserSocialNetworks");
            Alter.Table("UserSocialNetworks").AddColumn("Id").AsInt64().NotNullable().PrimaryKey("PK_UserSocialNetwork_2").Identity();
            Create.PrimaryKey("PK_UserSocialNetwork_2").OnTable("UserSocialNetworks").Column("Id");
            Alter.Table("UserSocialNetworks").AddColumn("UserName").AsString().NotNullable();
            Alter.Table("UserSocialNetworks").AddColumn("Photo").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Column("UserName").FromTable("UserSocialNetworks");
            Delete.Column("Photo").FromTable("UserSocialNetworks");
            Delete.PrimaryKey("PK_UserSocialNetwork_2").FromTable("UserSocialNetworks");
            Create.PrimaryKey("PK_UserSocialNetwork_1")
                .OnTable("UserSocialNetworks")
                .Columns(new string[] {"UserId", "SocialNetworkId"});
            Delete.Column("Id").FromTable("UserSocialNetworks");
        }
    }
}
