using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class Deduction : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public decimal Amount { get; set; }
        public string? Reason { get; set; }

        public DateTime? DeductionDate { get; set; }
    }
}
