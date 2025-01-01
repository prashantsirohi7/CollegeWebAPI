using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Config
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.DepartmentName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Description).IsRequired(false).HasMaxLength(200);

            builder.HasData(new List<Department> {
                new Department
                {
                    Id = 1,
                    DepartmentName = "CSE",
                    Description = "Computer Science Department"
                },
                new Department
                {
                    Id = 2,
                    DepartmentName = "Math",
                    Description = "Mathematics Department"
                }
            });
        }
    }
}
