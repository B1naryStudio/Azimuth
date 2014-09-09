using System.Threading.Tasks;
using Azimuth.DataAccess.Entities;
using Azimuth.DataAccess.Infrastructure;
using Azimuth.DataAccess.Repositories;
using Azimuth.Services.Interfaces;
using Azimuth.Shared.Enums;

namespace Azimuth.Services.Concrete
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationRepository _notificationRepository; 

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = _unitOfWork.GetRepository<Notification>() as NotificationRepository;
        }
        public Task CreateNotification(Notifications type, User user)
        {
            return Task.Run(() =>
            {
                using (_unitOfWork)
                {
                    var notification = new Notification
                    {
                        NotificationType = type,
                        User = user
                    };

                    _notificationRepository.AddItem(notification);

                    _unitOfWork.Commit();
                }
            });
        }
    }
}