using System.Net;
using System.Net.Http;
using System.Web.Http;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.Shared.Dto;

namespace Azimuth.ApiControllers
{
    public class UserController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public HttpResponseMessage Get(long id)
        {
            User user = null;
            using (_unitOfWork)
            {
                IRepository<User> userRepo = _unitOfWork.GetRepository<User>();

                user = userRepo.Get(id);

                _unitOfWork.Commit();
            }

            UserBrief dto = new UserBrief
            {
                Name = user.DisplayName,
                Email = user.Email
            };

            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }
    }
}