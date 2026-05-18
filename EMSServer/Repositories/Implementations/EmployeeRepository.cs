using EMSBaseLibrary.Entities;
using EMSBaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using EMSServer.Data;
using EMSServer.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMSServer.Repositories.Interfaces;

namespace EMSServer.Repositories.Implementations
{
    public class EmployeeRepository(AppDbContext db) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await db.Employees.FindAsync(id);
            if(item is null)
                return NotFound();

            db.Employees.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var employee = await db.Employees
                                    .AsNoTracking()
                                    .Include(a => a.Department)
                                    .Include(a => a.Role)
                                    .Include(m => m.Manager)
                                    .Include(p => p.Position)
                                    .ToListAsync();

            return employee;
        }

        public async Task<Employee> GetById(int id)
        {
            var employee = await db.Employees
                                   .AsNoTracking()
                                   .Include(a => a.Department)
                                   .Include(a => a.Role)
                                   .Include(m => m.Manager)
                                   .Include(p => p.Position)
                                   .FirstOrDefaultAsync(e => e.Id == id);
            
            return employee;
        }
        

        public async Task<GeneralResponse> Insert(Employee item)
        {
            var checkIfExists = await CheckEmployee(item.FirstName, item.LastName);
            if (checkIfExists)
                return new GeneralResponse(false, "Employee Already Added!");

            db.Employees.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            var employee = await db.Employees.FindAsync(item.Id);
            if(employee is null)
                return NotFound();

            employee.FirstName = item.FirstName;
            employee.LastName = item.LastName;
            employee.Email = item.Email;
            employee.Phone = item.Phone;
            employee.DateOfBirth = item.DateOfBirth;
            employee.HireDate = item.HireDate;
            employee.DepartmentId = item.DepartmentId;
            employee.RoleId = item.RoleId;
            employee.ManagerId = item.ManagerId;
            employee.Salary = item.Salary;
            employee.Status = item.Status;
            employee.PositionId = item.PositionId;

            await Commit();
            return Success();
        }

        private async Task Commit() => await db.SaveChangesAsync();
        private static GeneralResponse NotFound() => new(false, "Sorry, Employee not found!");
        private static GeneralResponse Success() => new(true, "Process Completed!");
        private async Task<bool> CheckEmployee(string name, string surname)
        {
            var item = await db.Employees.AnyAsync(e => e.FirstName!.ToLower().Equals(name.ToLower()) 
                                                     && e.LastName!.ToLower().Equals(surname.ToLower()));
            return item;
        }
    }
}
