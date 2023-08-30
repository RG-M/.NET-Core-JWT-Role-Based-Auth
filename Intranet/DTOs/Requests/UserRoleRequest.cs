using System.ComponentModel.DataAnnotations;

namespace Intranet.DTOs.Requests
{
    public class UserRoleRequest
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public List<string> roles { get; set; }
    }
}
