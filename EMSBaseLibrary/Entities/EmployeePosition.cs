using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class EmployeePosition : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
