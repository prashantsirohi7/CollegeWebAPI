using System.ComponentModel.DataAnnotations;

namespace StudentWebAPIProject.Models
{
    public class RolePrivilegeDTO
    {
        public int Id { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string RolePriviliegeName { get; set; }
        public string Description { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
