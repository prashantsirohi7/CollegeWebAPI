using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.Username).IsRequired();
            builder.Property(e => e.Password).IsRequired();
            builder.Property(e => e.PasswordSalt).IsRequired();
            builder.Property(e => e.UserTypeId).IsRequired();
            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.CreatedDate).IsRequired();

            builder.HasOne(e => e.UserType)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.UserTypeId)
                .HasConstraintName("FK_Users_UserTypes");
        }
    }
}
