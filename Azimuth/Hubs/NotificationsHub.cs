using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Microsoft.AspNet.SignalR;
using NHibernate.Criterion;

namespace Azimuth.Hubs
{
    public class NotificationsHub : Hub
    {
        public List<UserNotificationDto> ConnectedUsers { get; set; }

        public void Send(string message)
        {
            Clients.AllExcept(Context.ConnectionId).messageReceived(message);
        }

        public void Connect(long id)
        {
            var userDto = new UserNotificationDto
            {
                ConnectionId = Context.ConnectionId,
                UserId = id
            };

            ConnectedUsers.Add(userDto);
            Clients.AllExcept(Context.ConnectionId).messageReceived("hello " + id);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
            }

            return base.OnDisconnected(stopCalled);
        }

        public void SendMessage(long id, Notification notification)
        {
            var myFollowersIds = new List<long>();
            var socketsIds = ConnectedUsers.Where(x => x.UserId.IsIn(myFollowersIds)).Select(x => x.ConnectionId);
            foreach (var socketId in socketsIds)
            {
                Clients.Client(socketId).newNotification(notification);
            }
        }
    }

    public class UserNotificationDto
    {
        public string ConnectionId { get; set; }
        public long UserId { get; set; }
    }
}