using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Azimuth.Services.Interfaces;

namespace Azimuth.ApiControllers
{
    [RoutePrefix("api/notifications")]
    public class NotificationController : ApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> GetRecentActivity(int id, int offset = 0)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _notificationService.GetRecentActivity(id, offset));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("followings/{id:int}/{offset:int?}")]
        public async Task<HttpResponseMessage> GetFollowingsActivity(int id, int offset = 0)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _notificationService.GetFollowingsActivity(id, offset));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}