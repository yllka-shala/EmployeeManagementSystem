using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class EmployeeNotification : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int? NotificationId { get; set; }
        public Notification? Notification { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
