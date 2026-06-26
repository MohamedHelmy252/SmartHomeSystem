using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUnreadAsync(int userId);

        Task CreateAsync(int userId, string title, string message);

        Task<bool> MarkAsReadAsync(int userId, int notificationId);
    }
}
