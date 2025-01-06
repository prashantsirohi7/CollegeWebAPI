using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Config
{
    public class RolePrivilegeConfig : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolePrivileges");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.RolePriviliegeName).IsRequired();
            builder.Property(e => e.Description);
            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.CreatedDate).IsRequired();

            builder.HasOne(e => e.Role)
                .WithMany(e => e.RolePrivileges)
                .HasForeignKey(e => e.RoleId)
                .HasConstraintName("FK_RolePrivileges_Roles");
        }
    }
}
