How to use?

In AccountController add:

app.UseVkontakteAuthentication("{AppId}", "{AppSecret}", "{PERMISSIONS}");
{PERMISSIONS} - it is the comma-separated string. For example "email,audio" More info here http://vk.com/dev/permissions

Then you can get access_token and expires_in fields:

var identity = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
var email = result.Identity.FindFirst(ClaimTypes.Email).Value;
var token = result.Identity.FindFirst("AccessToken").Value;
var expired = result.Identity.FindFirst("Expired").Value;

or:

var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
var email = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
var token = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == "AccessToken").Value;
var expired = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == "Expired").Value;
