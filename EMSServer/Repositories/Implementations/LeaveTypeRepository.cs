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
    public class LeaveTypeRepository(AppDbContext db) : IGenericRepositoryInterface<LeaveType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var leaveType = await db.LeaveTypes.FirstOrDefaultAsync(e => e.Id == id);
            if (leaveType is null)
                return NotFound();

            db.LeaveTypes.Remove(leaveType);
            await Commit();
            return Success();
        }

        public async Task<List<LeaveType>> GetAll() => await db.LeaveTypes.AsNoTracking().ToListAsync();

        public async Task<LeaveType> GetById(int id) => await db.LeaveTypes.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<GeneralResponse> Insert(LeaveType item)
        {
            db.LeaveTypes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(LeaveType item)
        {
            var leaveType = await db.LeaveTypes.FirstOrDefaultAsync(e => e.Id == item.Id);
            if (leaveType is null)
                return NotFound();

            leaveType.Name = item.Name;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();

    }
}
