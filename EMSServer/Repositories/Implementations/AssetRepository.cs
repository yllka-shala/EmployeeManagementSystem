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
    public class AssetRepository(AppDbContext db) : IGenericRepositoryInterface<Asset>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == id);
            if (asset is null)
                return NotFound();

            db.Assets.Remove(asset);
            await Commit();
            return Success();
        }

        public async Task<List<Asset>> GetAll()
        {
            var assets = await db.Assets.AsNoTracking().ToListAsync();
            return assets;
        }
        

        public async Task<Asset> GetById(int id) => await db.Assets.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<GeneralResponse> Insert(Asset item)
        {
            db.Assets.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Asset item)
        {
            var asset = await db.Assets.FirstOrDefaultAsync(a => a.Id == item.Id);
            if (asset is null)
                return NotFound();

            asset.Name = item.Name;
            asset.AssetTag = item.AssetTag;
            asset.Description = item.Description;
            asset.PurchaseDate = item.PurchaseDate;
            asset.Value = item.Value;
            asset.Status = item.Status;

            await Commit();
            return Success();
        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
    }
}
