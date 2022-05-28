using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Notifications;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Notifications
{
    public class CustomNotificationService : ICustomNotificationService
    {
        #region Fields

        private readonly IRepository<Notification> _notificationRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public CustomNotificationService(IRepository<Notification> notificationRepository,
            IEventPublisher eventPublisher)
        {
            _notificationRepository = notificationRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
        public Notification GetNotificationById(int id)
        {
            if (id == 0)
                return null;

            return _notificationRepository.ToCachedGetById(id);
        }

        public IList<Notification> GetAllNotification()
        {
            var query = _notificationRepository.Table;

            return query.ToList();
        }

        public Notification GetNotificationByEntityIdAndEntityName(int entityId,string entityName)
        {
            if (entityId < 0)
                return null;

            if (string.IsNullOrEmpty(entityName))
                return null;

            return _notificationRepository.Table.Where(x => x.EntityId == entityId && x.EntityName == entityName).FirstOrDefault();
        }

        public void InsertNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            _notificationRepository.Insert(notification);

            //event notification
            _eventPublisher.EntityUpdated(notification);
        }

        public void UpdateNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));

            _notificationRepository.Update(notification);

            //event notification
            _eventPublisher.EntityUpdated(notification);
        }

        #endregion
    }
}
