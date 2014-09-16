using Azimuth.Shared.Dto;

namespace Azimuth.Hubs.Interfaces
{
    public interface INotificationsHub
    {
        void Connect(long id);
        void SendNotification(long id, NotificationDto notification);
    }
}