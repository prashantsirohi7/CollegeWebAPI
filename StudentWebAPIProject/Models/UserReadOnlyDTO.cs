using System.ComponentModel.DataAnnotations;

namespace StudentWebAPIProject.Models
{
    public class UserReadOnlyDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int UserTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}
