using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class PerformanceReview : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int? ReviewerId { get; set; }
        public Employee? Reviewer { get; set; }
        public DateTime? ReviewDate { get; set; }
        public int Rating { get; set; }

        public string? Comments { get; set; }

    }
}
