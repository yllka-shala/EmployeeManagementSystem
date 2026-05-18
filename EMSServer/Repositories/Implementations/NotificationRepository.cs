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
    public class NotificationRepository(AppDbContext db) : IGenericRepositoryInterface<Notification>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var notification = await db.Notifications.FindAsync(id);
            if (notification is null)
                return NotFound();

            db.Notifications.Remove(notification);
            await Commit();
            return Success();
        }

        public async Task<List<Notification>> GetAll() => await db.Notifications.AsNoTracking().ToListAsync();

        public async Task<Notification> GetById(int id) => await db.Notifications.FindAsync(id);

        public async Task<GeneralResponse> Insert(Notification item)
        {
            var checkIfExists = await CheckName(item.Title);
            if (checkIfExists)
                return new GeneralResponse(false, "Notification Already Added!");

            db.Notifications.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Notification item)
        {
            var notification = await db.Notifications.FindAsync(item.Id);
            if (notification is null)
                return NotFound();

            notification.Title = item.Title;
            notification.Message = item.Message;
            notification.Type = item.Type;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Notification Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var notification = await db.Notifications.AnyAsync(x => x.Title!.ToLower().Equals(name.ToLower()));
            return notification;
        }
    }
}
