using System;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.Infrastructure;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services;
using Azimuth.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Azimuth.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Go here sometimes
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        public ActionResult ConnectAccount(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl, AutoLogin = false }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl, bool autoLogin = true)
        {
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            
            if (result == null || result.Identity == null)
            {
                return RedirectToAction("Login");
            }

            var principal = ClaimsAuthenticationManager.Authenticate(String.Empty, new ClaimsPrincipal(result.Identity));
            var identity = principal.Identity as AzimuthIdentity;
            var loggedIdentity = AzimuthIdentity.Current;

            var provider = AccountProviderFactory.GetAccountProvider(identity.UserCredential);

            var userInfo = await provider.GetUserInfoAsync(identity.UserCredential.Email);
            var storeResult = _accountService.SaveOrUpdateUserData(userInfo, identity.UserCredential, loggedIdentity);

            if (storeResult && autoLogin)
            {
                SignIn(identity, userInfo);
            }
            return RedirectToLocal(returnUrl);
        }

        private void SignIn(AzimuthIdentity identity, User userInfo)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Name.FirstName + " " + userInfo.Name.LastName));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                identity.UserCredential.SocialNetworkName, Rights.PossessProperty));

            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = true}, identity);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private ClaimsAuthenticationManager ClaimsAuthenticationManager
        {
            get
            {
                return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // Go here sometimes
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}