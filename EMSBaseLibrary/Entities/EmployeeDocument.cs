using EMSBaseLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.Entities
{
    public class EmployeeDocument : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DocumentCategory Category { get; set; }
        public DateTime? UploadedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int? UploadedBy { get; set; }
        public Employee? Uploader { get; set; }

        [NotMapped]
        public string? Base64ImageData { get; set; }
    }
}
