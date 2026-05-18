using EMSBaseLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class Asset : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? AssetTag { get; set; }
        public string? Description { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public decimal? Value { get; set; }

        public AssetStatus Status { get; set; }

        public ICollection<EmployeeAsset> EmployeeAssets { get; set; } = new List<EmployeeAsset>();

    }
}
