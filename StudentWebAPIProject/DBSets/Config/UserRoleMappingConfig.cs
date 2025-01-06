using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Config
{
    public class UserRoleMappingConfig : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.ToTable("UserRoleMappings");
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.UserId, e.RoleId }, "UK_UserRoleMappings").IsUnique();

            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.RoleId).IsRequired();
            builder.Property(e => e.UserId).IsRequired();

            builder.HasOne(e => e.Role)
                .WithMany(e => e.UserRoleMappings)
                .HasForeignKey(e => e.RoleId)
                .HasConstraintName("FK_UserRoleMappings_Roles");

            builder.HasOne(e => e.User)
                .WithMany(e => e.UserRoleMappings)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_UserRoleMappings_Users");
        }
    }
}
