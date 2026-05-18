using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext db) : IGenericRepositoryInterface<Department>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await db.Departments.FindAsync(id);
            if(dep is null)
                return NotFound();

            db.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await db.Departments
                                                                .AsNoTracking()
                                                                .ToListAsync();

        public async Task<Department> GetById(int id) => await db.Departments.FindAsync(id); 

        public async Task<GeneralResponse> Insert(Department item)
        {
            var checkIfExist = await CheckName(item.Name);
            if (!checkIfExist)
                return new GeneralResponse(false, "Department already added!");

            db.Departments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await db.Departments.FindAsync(item.Id);
            if (dep is null) 
                return NotFound();

            dep.Name = item.Name;
            dep.Description = item.Description;
            await Commit();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, Department Not Found!");
        private static GeneralResponse Success() => new(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();

        public async Task<bool> CheckName(string name)
        {
            var dep = await db.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return dep is null ? true : false;
        }
    }
}
