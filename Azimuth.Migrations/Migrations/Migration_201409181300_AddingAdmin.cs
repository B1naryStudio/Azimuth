
using FluentMigrator;

namespace Azimuth.Migrations
{
    [Migration(201409181300)]
    public class Migration_201409181300_AddingAdmin: Migration
    {
        public override void Up()
        {
            Execute.Sql("SET IDENTITY_INSERT dbo.Users ON");
            Insert.IntoTable("Users")
                .Row(
                    new
                    {
                        Id = 0,
                        FirstName = "Azimuth",
                        LastName = "Azimuth",
                        ScreenName = "id268940215",
                        Gender = "Male",
                        Photo = "http://vk.com/images/camera_400.gif",
                        Timezone = "2",
                        Email = "azimuth.proj@gmail.com"
                    });
            Execute.Sql("SET IDENTITY_INSERT dbo.Users OFF");

            Execute.Sql("SET IDENTITY_INSERT dbo.UserSocialNetworks ON");
            Insert.IntoTable("UserSocialNetworks")
                .Row(
                    new
                    {
                        Id = 0,
                        UserId = 0,
                        SocialNetworkId = 1,
                        ThirdPartId = 268940215,
                        AccessToken =
                            "87ddf0d5a101cbef5c204aeb859cb248f48d4bd0ed3fa75d95f17fe3f1a3d844831fbf7da2ecbcc9250b2",
                        TokenExpires = "00:00:00",
                        UserName = "Azimuth Azimuth",
                        Photo = "http://vk.com/images/camera_400.gif"
                    });

            Execute.Sql("SET IDENTITY_INSERT dbo.UserSocialNetworks OFF");
        }

        public override void Down()
        {
            Execute.Sql("DELETE FROM Users WHERE Id=0;");
            Execute.Sql("DELETE FROM UserSocialNetworks WHERE Id=0;");
        }
    }
}