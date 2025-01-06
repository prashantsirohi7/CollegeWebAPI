using System.ComponentModel.DataAnnotations;

namespace StudentWebAPIProject.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int UserTypeId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
