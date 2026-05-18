

using EMSBaseLibrary.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EMSBaseLibrary.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public DateTime? HireDate { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? RoleId { get; set; }
        public Roles? Role { get; set; }


        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();


        public decimal Salary { get; set; }
        public EmployeeStatus Status { get; set; }

        public int? PositionId { get; set; }
        public EmployeePosition? Position { get; set; }

        public ICollection<Bonus> Bonuses { get; set; } = new List<Bonus>();
        public ICollection<Deduction> Deductions { get; set; } = new List<Deduction>();
        public ICollection<EmployeeAsset> EmployeeAssets { get; set; } = new List<EmployeeAsset>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public ICollection<EmployeeNotification> EmployeeNotifications { get; set; } = new List<EmployeeNotification>();
    }
}
