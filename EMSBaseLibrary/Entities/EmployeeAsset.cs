using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class EmployeeAsset : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public DateTime? AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }

    }
}
