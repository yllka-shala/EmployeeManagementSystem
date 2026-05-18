using EMSBaseLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int? LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Reason { get; set; }
        public LeaveStatus Status { get; set; }

        public int? ApprovedBy { get; set; }
        public Employee? Approver { get; set; }

    }
}
