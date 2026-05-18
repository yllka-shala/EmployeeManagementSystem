using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class LeaveRequestRepository(AppDbContext db) : IGenericRepositoryInterface<LeaveRequest>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var leaveRequest = await db.LeaveRequests.FindAsync(id);
            if (leaveRequest is null)
                return NotFound();

            db.LeaveRequests.Remove(leaveRequest);
            await Commit();
            return Success();
        }

        public async Task<List<LeaveRequest>> GetAll()
        {
           var leaveRequest = await db.LeaveRequests
                                    .Include(e => e.Employee)
                                    .Include(l => l.LeaveType)
                                    .Include(a => a.Approver)
                                    .ToListAsync();
           return leaveRequest;
        }

        public async Task<LeaveRequest> GetById(int id)
        {
            var leaveRequest = await db.LeaveRequests
                                        .AsNoTracking()
                                        .Include(e => e.Employee)
                                        .Include(l => l.LeaveType)
                                        .Include(a => a.Approver)
                                        .FirstOrDefaultAsync(e => e.Id == id);
            return leaveRequest;
        }

        public async Task<GeneralResponse> Insert(LeaveRequest item)
        {
            var checkIfExist = await CheckEmployee(item.EmployeeId);
            if (checkIfExist)
                return new GeneralResponse(false, "Leave Request Already Added!");

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            if (item.LeaveTypeId == null)
                return new GeneralResponse(false, "Please provide a valid Leave Type!");

            db.LeaveRequests.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(LeaveRequest item)
        {
            var leaveRequest = await db.LeaveRequests.FindAsync(item.Id);
            if (leaveRequest is null) 
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            if (item.LeaveTypeId == null)
                return new GeneralResponse(false, "Please provide a valid Leave Type!");

            leaveRequest.EmployeeId = item.EmployeeId;
            leaveRequest.LeaveTypeId = item.LeaveTypeId;
            leaveRequest.StartDate = item.StartDate;
            leaveRequest.EndDate = item.EndDate;
            leaveRequest.Reason = item.Reason;
            leaveRequest.Status = item.Status;
            leaveRequest.ApprovedBy = item.ApprovedBy;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Leave Request not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();

        private async Task<bool> CheckEmployee(int? Id)
        {
            var leaveRequest = await db.LeaveRequests.AnyAsync(x => x.EmployeeId == Id && x.StartDate == DateTime.Today);
            return leaveRequest;
        }
    }
}
