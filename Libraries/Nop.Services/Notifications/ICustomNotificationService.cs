using System.Collections.Generic;
using Nop.Core.Domain.Notifications;

namespace Nop.Services.Notifications
{
    public interface ICustomNotificationService
    {
        void InsertNotification(Notification notification);

        void UpdateNotification(Notification notification);

        Notification GetNotificationById(int id);

        Notification GetNotificationByEntityIdAndEntityName(int entityId , string entityName);

        IList<Notification> GetAllNotification();
    }
}
