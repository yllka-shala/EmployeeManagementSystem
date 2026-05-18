

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EMSBaseLibrary.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
