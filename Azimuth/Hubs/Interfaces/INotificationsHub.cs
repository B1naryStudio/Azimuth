using System.Collections.Generic;
using Azimuth.Shared.Dto;

namespace Azimuth.Hubs.Interfaces
{
    public interface INotificationsHub
    {
        void Connect(long id);
        void SendNotification(long id, NotificationDto notification, List<long> listReceivers );
    }
}