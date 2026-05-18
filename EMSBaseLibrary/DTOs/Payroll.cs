using EMSBaseLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.DTOs
{
    public class Payroll
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public decimal Salary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }

        public DateTime PayDate { get; set; }
    }
}
