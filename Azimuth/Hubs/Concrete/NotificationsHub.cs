using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Hubs.Interfaces;
using Azimuth.Shared.Dto;
using Microsoft.AspNet.SignalR;

namespace Azimuth.Hubs.Concrete
{
    public class NotificationsHub : Hub, INotificationsHub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserRepository _userRepository;
        private static NotificationsHub _instance;
        private static readonly object SyncRoot = new Object();
        public List<UserNotificationDto> ConnectedUsers { get; set; }

        public NotificationsHub() : base()
        {
            _instance = this;
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
            ConnectedUsers = new List<UserNotificationDto>();

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
            //var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            //if (item != null)
            //{
            //    ConnectedUsers.Remove(item);
            //}

            return base.OnDisconnected(stopCalled);
        }

        public void SendNotification(long id, NotificationDto notification)
        {
            //var myFollowings = _userRepository.GetOne(u => u.Id == id).Following;
            //if (myFollowings != null)
            //{
            //    var myFollowingsIds = myFollowings.Select(f => f.Id).ToList();
            //    var socketsIds = ConnectedUsers.Where(x => x.UserId.IsIn(myFollowingsIds)).Select(x => x.ConnectionId);
            //    foreach (var socketId in socketsIds)
            //    {
            //       Clients.Client(socketId).newNotification(notification);
            //    }    
            //}

            Clients.All.newNotification(notification);
        }
    }
}