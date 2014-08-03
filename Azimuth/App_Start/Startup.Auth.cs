using System.Security.AccessControl;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;

namespace Azimuth
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            app.UseTwitterAuthentication(
               consumerKey: "LlXtJmJSLVQd8gzALZEctToQC",
               consumerSecret: "VrO3OXhWoCxAwQxzGvEoTVHeua1MoKTZv1zZ7dM47WnhKAOgAn");

            var fb = new FacebookAuthenticationOptions();
            fb.Scope.Add("email");
            fb.Scope.Add("user_birthday");
            fb.Scope.Add("user_location");
            fb.Scope.Add("user_photos");
            fb.AppId = "609844869113324";
            fb.AppSecret = "399f367e79f11226d1522c00c72a6c6d";
            fb.Provider = new FacebookAuthenticationProvider()
            {
                OnAuthenticated = async context =>
                {
                    // Get accesstoken from Facebook and store it in the database and
                    // user Facebook C# SDK to get more information about the user
                    context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                }
            };
            fb.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(fb);
            //app.UseFacebookAuthentication(
            //   appId: "609844869113324",
            //   appSecret: "399f367e79f11226d1522c00c72a6c6d");

            app.UseGoogleAuthentication(
                clientId: "847308079087-bl2m5iev3iibsp9pfoulodosek33rtrl.apps.googleusercontent.com",
                clientSecret: "oHy-Vd8TS48P4Ybz_Gsp_y2h");

            app.UseVkontakteAuthentication(
                appId: "4469725",
                appSecret: "1vUUwTGWEIp3bSLqDHuw",
                scope: "audio");
        }
    }
}