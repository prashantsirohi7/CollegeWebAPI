using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Config
{
    public class UserTypeConfig : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).UseIdentityColumn();
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Description).IsRequired();

            builder.HasData(new List<UserType> {
                new UserType
                {
                    Id = 1,
                    Name = "Student",
                    Description = "For Students"
                },
                new UserType
                {
                    Id = 2,
                    Name = "Faculty",
                    Description = "For Faculty"
                },
                new UserType
                {
                    Id = 3,
                    Name = "Support Staff",
                    Description = "For Support Staff"
                },
                new UserType
                {
                    Id = 4,
                    Name = "Parents",
                    Description = "For Parents"
                }
            });
        }
    }
}
