using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
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
                //AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
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

            app.UseFacebookAuthentication(
               appId: "609844869113324",
               appSecret: "399f367e79f11226d1522c00c72a6c6d");

            app.UseGoogleAuthentication(
                clientId: "847308079087-bl2m5iev3iibsp9pfoulodosek33rtrl.apps.googleusercontent.com",
                clientSecret: "oHy-Vd8TS48P4Ybz_Gsp_y2h");
        }
    }
}