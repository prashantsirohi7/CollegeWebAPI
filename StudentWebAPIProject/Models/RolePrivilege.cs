namespace StudentWebAPIProject.Models
{
    public class RolePrivilege
    {
        public int Id { get; set; }
        public string RolePriviliegeName { get; set; }
        public string Description { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Role Role { get; set; }
    }
}
