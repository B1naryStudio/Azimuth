using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azimuth.Hubs.Interfaces;
using Azimuth.Shared.Dto;
using Microsoft.AspNet.SignalR;

namespace Azimuth.Hubs.Concrete
{
    public class NotificationsHub : Hub, INotificationsHub
    {
        private static NotificationsHub _instance;
        private static readonly object SyncRoot = new Object();
        public static List<UserNotificationDto> ConnectedUsers { get; set; }

        public NotificationsHub() : base()
        {
            _instance = this;
        }

        static NotificationsHub()
        {
            ConnectedUsers = new List<UserNotificationDto>();
        }

        public static INotificationsHub Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new NotificationsHub();
                    }
                }
                return _instance;
            }
        }

        public void Connect(long id)
        {

            var user = ConnectedUsers.FirstOrDefault(s => s.UserId == id);
            //if (user != null)
            //{
            //    user.ConnectionId = Context.ConnectionId;
            //}
            //else
            //{
                var userDto = new UserNotificationDto
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = id
                };

                ConnectedUsers.Add(userDto);
            //}
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

        public void SendNotification(long id, NotificationDto notification, List<long> listReceivers )
        {
            var list = ConnectedUsers.Where(s => listReceivers.Contains(s.UserId)).Select(s => s.ConnectionId).ToList();
            if (list.Count > 0)
            {

                Clients.Clients(list).newNotification(notification); 
                //foreach (var socketId in list)
                //{
                //    Clients.Client(socketId).newNotification(notification);   
                //}
            }
        }
    }
}