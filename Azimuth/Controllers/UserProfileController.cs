using System.Web.Mvc;
using Azimuth.DataAccess.Entities;
using Azimuth.Infrastructure.Concrete;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Dto;

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
        public ActionResult Index(int? id)
        {
            User data;
            if (id == null)
            {
                var email = AzimuthIdentity.Current.UserCredential.Email;
                data = _userService.GetUserInfo(email);
            }
            else
            {
                data = _userService.GetUserInfo((int)id);    
            }
            
            return View(data);
        }
    }
}
