using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class AttendanceRepository(AppDbContext db) : IGenericRepositoryInterface<Attendance>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var attendance = await db.Attendances.FindAsync(id);
            if(attendance is null)
                return NotFound();

            db.Attendances.Remove(attendance);
            await Commit();
            return Success();
        }

        public async Task<List<Attendance>> GetAll() => await db.Attendances
                                                          .AsNoTracking()
                                                          .Include(c => c.Employee)
                                                          .ToListAsync();

        public async Task<Attendance> GetById(int id) => await db.Attendances.FindAsync(id);

        public async Task<GeneralResponse> Insert(Attendance item)
        {
            var checkIfExist = await CheckEmployee(item.EmployeeId);
            if (checkIfExist)
                return new GeneralResponse(false, "Attendance Already Added!");

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            db.Attendances.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Attendance item)
        {
            var attendance = await db.Attendances.FindAsync(item.Id);
            if (attendance is null)
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            attendance.EmployeeId = item.EmployeeId;
            attendance.CheckIn = item.CheckIn;
            attendance.CheckOut = item.CheckOut;
            attendance.Status = item.Status;
            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Attendance Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
       
        private async Task<bool> CheckEmployee(int? Id)
        {
            var attendance = await db.Attendances.AnyAsync(x => x.EmployeeId == Id && x.CheckIn == DateTime.Today);
            return attendance;
        }
    }
}
