using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSServer.Repositories.Implementations
{
    public class EmployeeNotificationRepository(AppDbContext db) : IGenericRepositoryInterface<EmployeeNotification>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var notification = await db.EmployeeNotifications.FirstOrDefaultAsync(n => n.EmployeeId == id);
            if(notification is null)
                return NotFound();

            db.EmployeeNotifications.Remove(notification);
            await Commit();
            return Success();
        }

        public async Task<List<EmployeeNotification>> GetAll()
        {
            var notifications = await db.EmployeeNotifications
                                        .AsNoTracking()
                                        .Include(en => en.Employee)
                                        .Include(en => en.Notification)
                                        .ToListAsync();
            return notifications;
        }

        public async Task<EmployeeNotification> GetById(int id)
        {
           var notification = await db.EmployeeNotifications.FirstOrDefaultAsync(en => en.EmployeeId == id);
           return notification;
        }
        public async Task<GeneralResponse> Insert(EmployeeNotification item)
        {
            db.EmployeeNotifications.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(EmployeeNotification item)
        {
            var notification = await db.EmployeeNotifications.FirstOrDefaultAsync(en => en.EmployeeId == item.EmployeeId);
            if (notification is null)
                return NotFound();

            notification.EmployeeId = item.EmployeeId;
            notification.NotificationId = item.NotificationId;
            notification.IsRead = item.IsRead;
            notification.ReadAt = item.ReadAt;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
    }
}
