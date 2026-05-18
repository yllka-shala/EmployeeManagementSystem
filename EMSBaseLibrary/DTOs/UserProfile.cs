using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSBaseLibrary.DTOs
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureFileName { get; set; }
        public string? Base64ImageData { get; set; }

    }
}
