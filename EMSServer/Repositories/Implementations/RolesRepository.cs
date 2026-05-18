using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EMSServer.Repositories.Implementations
{
    public class RolesRepository(AppDbContext db) : IGenericRepositoryInterface<Roles>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var role = await db.Roles.FindAsync(id);
            if (role is null)
                return NotFound();

            db.Roles.Remove(role);
            await Commit();
            return Success();
        }

        public async Task<List<Roles>> GetAll() => await db.Roles
                                                          .AsNoTracking()
                                                          .ToListAsync();

        public async Task<Roles> GetById(int id) => await db.Roles.FindAsync(id);

        public async Task<GeneralResponse> Insert(Roles item)
        {
            var checkIfExists = await CheckName(item.Name);
            if (checkIfExists)
                return new GeneralResponse(false, "Role Already Added!");

            db.Roles.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Roles item)
        {
            var role = await db.Roles.FindAsync(item.Id);
            if (role is null)
                return NotFound();

            role.Name = item.Name;
            role.Description = item.Description;

            await Commit();
            return Success();
        }   

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Role Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        private async Task<bool> CheckName(string name)
        {
            var roles = await db.Roles.AnyAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return roles;
        }
    }
}
