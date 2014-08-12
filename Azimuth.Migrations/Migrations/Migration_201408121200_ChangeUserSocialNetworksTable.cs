
using FluentMigrator;

namespace Azimuth.Migrations.Migrations
{
    [Migration(201408121200)]
    public class Migration_201408121200_ChangeUserSocialNetworksTable: Migration
    {
        public override void Up()
        {
            Delete.PrimaryKey("PK__UserSoci__47534F08CD395008").FromTable("UserSocialNetworks");
            //Delete.Column("UserId").FromTable("UserSocialNetworks");
            Alter.Table("UserSocialNetworks").AddColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity();
            Create.PrimaryKey("PK_UserSoci_12345").OnTable("UserSocialNetworks").Column("Id");
            Alter.Table("UserSocialNetworks").AddColumn("UserName").AsString().NotNullable();
            Alter.Table("UserSocialNetworks").AddColumn("Photo").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Column("Id").FromTable("UserSocialNetworks");
            Delete.Column("UserName").FromTable("UserSocialNetworks");
            Delete.Column("Photo").FromTable("UserSocialNetworks");
            Delete.PrimaryKey("PK_UserSoci_12345").FromTable("UserSocialNetworks");
            Create.PrimaryKey("PK_UserSoci_1234")
                .OnTable("UserSocialNetworks")
                .Columns(new string[] {"UserId", "SocialNetworkId"});
        }
    }
}
