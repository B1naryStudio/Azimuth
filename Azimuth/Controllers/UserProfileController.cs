using System.Web.Mvc;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;

namespace Azimuth.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }
        //
        // GET: /UserProfile/
        public ActionResult Index(long? id)
        {
            var user = _userService.GetUserInfo(id ?? AzimuthIdentity.Current.UserCredential.Id);
            return View(user);
        }
    }
}
