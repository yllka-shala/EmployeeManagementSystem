using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class PositionRepository(AppDbContext db) : IGenericRepositoryInterface<EmployeePosition>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var position = await db.Positions.FindAsync(id);
            if (position is null)
                return NotFound();

            db.Positions.Remove(position);
            await Commit();
            return Success();
        }

        public async Task<List<EmployeePosition>> GetAll() => await db.Positions
                                                            .AsNoTracking()
                                                            .Include(d => d.Department)
                                                            .ToListAsync(); 

        public async Task<EmployeePosition> GetById(int id) => await db.Positions.FindAsync(id);
                    
        public async Task<GeneralResponse> Insert(EmployeePosition item)
        {
            var checkIfExists = await CheckName(item.Title);
            if (checkIfExists)
                return new GeneralResponse(false,"Position already added!");

            db.Positions.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(EmployeePosition item)
        {
            var position = await db.Positions.FindAsync(item.Id);
            if (position == null) 
                return NotFound();

            position.Title = item.Title;
            position.Description = item.Description;
            position.DepartmentId = item.DepartmentId;
            position.MinSalary = item.MinSalary;
            position.MaxSalary = item.MaxSalary;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Position not Found!");
        private static GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        
        private async Task<bool> CheckName(string name)
        {
            var branch = await db.Positions.AnyAsync(x => x.Title!.ToLower().Equals(name.ToLower()));
            return branch;
        }
    }
}
