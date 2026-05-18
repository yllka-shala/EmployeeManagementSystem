using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using EMSServer.Data;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSServer.Repositories.Implementations
{
    public class EmployeeDocumentRepository(AppDbContext db, IFileRepository fileService) : IGenericRepositoryInterface<EmployeeDocument>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var document = await db.EmployeeDocuments.FirstOrDefaultAsync(d => d.Id == id);
            if(document is null)
                return NotFound();

            db.EmployeeDocuments.Remove(document);
            await Commit();
            return Success();

        }

        public async Task<List<EmployeeDocument>> GetAll() 
        { 
            var documents = await db.EmployeeDocuments    
                                    .AsNoTracking()
                                    .Include(ed => ed.Employee)
                                    .Include(ed => ed.Uploader)
                                    .ToListAsync();
            return documents;
        }
        public async Task<EmployeeDocument> GetById(int id) 
        { 
            var document = await db.EmployeeDocuments
                                    .AsNoTracking()
                                    .Include(ed => ed.Employee)
                                    .Include(ed => ed.Uploader)
                                    .FirstOrDefaultAsync(ed => ed.EmployeeId == id);
            return document;
        }

        public async Task<GeneralResponse> Insert(EmployeeDocument item)
        {
            if (!string.IsNullOrEmpty(item.Base64ImageData))
            {
                var fileName = await fileService.SaveFile(item.Base64ImageData, item.FileName);
                item.FileName = fileName;
            }

            var employeeDoc = new EmployeeDocument
            {
                EmployeeId = item.EmployeeId,
                FileName = item.FileName,
                FilePath = item.FilePath,
                Category = item.Category,
                UploadedAt = item.UploadedAt,
                ExpiryDate = item.ExpiryDate,
                UploadedBy = item.UploadedBy
            };

            db.EmployeeDocuments.Add(employeeDoc);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(EmployeeDocument item)
        {
            var document = await db.EmployeeDocuments.FirstOrDefaultAsync(d => d.Id == item.Id);
            if(document is null)
                return NotFound();

            if (!string.IsNullOrEmpty(item.Base64ImageData))
            {
                var fileName = await fileService.SaveFile(item.Base64ImageData, item.FileName);

                //Delete the old picture from folder
                if (!string.IsNullOrEmpty(document.FileName))
                {
                    fileService.DeleteFile(document.FileName);
                }

                document.FileName = fileName;
            }

            document.EmployeeId = item.EmployeeId;
            document.FilePath = item.FilePath;
            document.Category = item.Category;
            document.UploadedAt = item.UploadedAt;
            document.ExpiryDate = item.ExpiryDate;
            document.UploadedBy = item.UploadedBy;

            await Commit();
            return Success();

        }

        private GeneralResponse NotFound() => new GeneralResponse(false, "Not Found!");
        private GeneralResponse Success() => new GeneralResponse(true, "Process Completed!");
        private async Task Commit() => await db.SaveChangesAsync();
    }
}
