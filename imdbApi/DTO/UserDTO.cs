using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace imdbApi.DTO
{
    public class UserDTO
    {
       [Required]
        
        public string UserName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string isAdmin { get; set; } = null!;
    }
}
