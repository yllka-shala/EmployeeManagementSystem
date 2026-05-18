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
    public class BonusRepository(AppDbContext db) : IGenericRepositoryInterface<Bonus>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var bonus = await db.Bonuses.FindAsync(id);
            if (bonus is null)
                return NotFound();

            db.Bonuses.Remove(bonus);
            await Commit();
            return Success();
        }

        public async Task<List<Bonus>> GetAll() 
        { 
            var bonuses = await db.Bonuses
                                  .AsNoTracking()
                                  .Include(b => b.Employee)
                                  .ToListAsync();
            return bonuses;
        }

        public async Task<Bonus> GetById(int id)
        {
            var bonus = await db.Bonuses
                                .Include(b => b.Employee)
                                .FirstOrDefaultAsync(b => b.Id == id);
            return bonus;
        }   

        public async Task<GeneralResponse> Insert(Bonus item)
        {
            var checkIfExist = await CheckEmployee(item.EmployeeId);
            if (checkIfExist)
                return new GeneralResponse(false, "Bonus Already Added!");  

            if(item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            db.Bonuses.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Bonus item)
        {
            var bonus = await db.Bonuses.FindAsync(item.Id);
            if (bonus is null)
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            bonus.EmployeeId = item.EmployeeId;
            bonus.Amount = item.Amount;
            bonus.Reason = item.Reason;
            bonus.AwardedDate = item.AwardedDate;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Bonus Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        private async Task<bool> CheckEmployee(int? Id)
        {
            var bonus = await db.Bonuses.AnyAsync(x => x.EmployeeId == Id && x.AwardedDate == DateTime.Today);
            return bonus;
        }
    }
}
