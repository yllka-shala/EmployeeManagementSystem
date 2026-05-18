using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class DeductionRepository(AppDbContext db) : IGenericRepositoryInterface<Deduction>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var deduction = await db.Deductions.FindAsync(id);
            if(deduction is null)
                return NotFound();

            db.Deductions.Remove(deduction);
            await Commit();
            return Success();
        }

        public async Task<List<Deduction>> GetAll() => await db.Deductions
                                                          .AsNoTracking()
                                                          .Include(a => a.Employee)
                                                          .ToListAsync();

        public async Task<Deduction> GetById(int id) => await db.Deductions.FindAsync(id);

        public async Task<GeneralResponse> Insert(Deduction item)
        {
            var checkIfExist = await CheckEmployee(item.EmployeeId);
            if (checkIfExist)
                return new GeneralResponse(false, "Deduction Already Added!");

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            db.Deductions.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Deduction item)
        {
            var deduction = await db.Deductions.FindAsync(item.Id);
            if(deduction is null)
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            deduction.EmployeeId = item.EmployeeId;
            deduction.Amount = item.Amount;
            deduction.Reason = item.Reason;
            deduction.DeductionDate = item.DeductionDate;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Deduction Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        private async Task<bool> CheckEmployee(int? Id)
        {
            var deduction = await db.Deductions.AnyAsync(x => x.EmployeeId == Id && x.DeductionDate == DateTime.Today);
            return deduction;
        }
    }
}
