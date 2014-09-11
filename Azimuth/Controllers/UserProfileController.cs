using System;
using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
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

        [HttpPost]
        public PartialViewResult _FollowInfo(long? userId, string isFollower)
        {
            var isFollow = Convert.ToBoolean(isFollower);
            User user = null;
            if (isFollow && userId != null)
            {
               user = _userService.UnfollowPerson((long)userId);
            }
            else if (userId != null)
            {
                user = _userService.FollowPerson((long)userId);
            }
            ViewBag.isFollower = !isFollow;
            return PartialView("_UserFollowInfo", user);
        }
    }
}
