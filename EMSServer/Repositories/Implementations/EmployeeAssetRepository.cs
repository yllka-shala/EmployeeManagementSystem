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
    public class EmployeeAssetRepository(AppDbContext db) : IGenericRepositoryInterface<EmployeeAsset>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var ea = await db.EmployeeAssets.FindAsync(id);
            if(ea is null)
                return NotFound();

            db.EmployeeAssets.Remove(ea);
            await Commit();
            return Success();
        }

        public async Task<List<EmployeeAsset>> GetAll() 
        {
            var ea = await db.EmployeeAssets
                             .AsNoTracking()
                             .Include(e => e.Employee)
                             .Include(a => a.Asset)
                             .ToListAsync(); 
            return ea;
        }

        public async Task<EmployeeAsset> GetById(int id) 
        { 
            var ea = await db.EmployeeAssets
                             .Include(e => e.Employee)
                             .Include(a => a.Asset)
                             .FirstOrDefaultAsync(ea => ea.Id == id); 
            return ea;
        }

        public async Task<GeneralResponse> Insert(EmployeeAsset item)
        {
            var checkIfExist = await CheckEmployee(item.EmployeeId);
            if (checkIfExist)
                return new GeneralResponse(false, "Employee Asset Already Added!");

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            db.EmployeeAssets.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(EmployeeAsset item)
        {
            var ea = await db.EmployeeAssets.FindAsync(item.Id);
            if (ea is null)
                return NotFound();

            if (item.EmployeeId == null)
                return new GeneralResponse(false, "Please provide a valid Employee!");

            ea.EmployeeId = item.EmployeeId;
            ea.AssetId = item.AssetId;
            ea.AssignedDate = item.AssignedDate;
            ea.ReturnedDate = item.ReturnedDate;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Sorry, Employee Asset Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
        private async Task<bool> CheckEmployee(int? id)
        {
            var employeeAssts = await db.EmployeeAssets.AnyAsync(x => x.EmployeeId == id && x.AssignedDate == DateTime.Today);
            return employeeAssts;
        }
    }
}
