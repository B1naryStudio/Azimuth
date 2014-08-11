using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.DataProviders.Concrete;
using Azimuth.DataProviders.Interfaces;
using Azimuth.Infrastructure;
using Azimuth.Services;
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
            var isAuthenticated = ClaimsPrincipal.Current.Identity.IsAuthenticated;
            var loggedUser = HttpContext.User.Identity as AzimuthIdentity;
            var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            
            if (result == null || result.Identity == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                Authenticate(result.Identity);
            }
            catch(AuthenticationException)
            {
                return RedirectToAction("Login");
            }

            var identity = ClaimsPrincipal.Current.Identity as AzimuthIdentity;

            IAccountProvider provider = AccountProviderFactory.GetAccountProvider(identity.UserCredential);

            var currentUser = await provider.GetUserInfoAsync(identity.UserCredential.Email);
            var storeResult = _accountService.SaveOrUpdateUserData(currentUser, identity.UserCredential, isAuthenticated);

            if (storeResult && autoLogin)
            {
                await SignInAsync(currentUser, identity.UserCredential.SocialNetworkName);
            }
            return RedirectToLocal(returnUrl);
        }

        private void Authenticate(IIdentity identity)
        {
            var authManger = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
            var principal = authManger.Authenticate(String.Empty, new ClaimsPrincipal(identity));

            Thread.CurrentPrincipal = principal;
            HttpContext.User = principal;
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User currentUser, string socialNetwork)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, currentUser.Name.FirstName + " " + currentUser.Name.LastName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString()));
            Claim claim = new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", socialNetwork, Rights.PossessProperty);
            claims.Add(claim);
            var id = new ClaimsIdentity(claims,
                                        DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(id);
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