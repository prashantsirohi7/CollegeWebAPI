using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentWebAPIProject.Models;
using System.Reflection.Emit;

namespace StudentWebAPIProject.DBSets.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(30);
            builder.Property(e => e.DOB).IsRequired();
            builder.Property(e => e.Email).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Address).IsRequired(false).HasMaxLength(200);

            builder.HasData(new List<Student> {
                new Student
                {
                    Id = 1,
                    Name = "John",
                    DOB = new DateTime(1990, 12, 12),
                    Email = "John@test.com",
                    Address = "USA"
                },
                new Student
                {
                    Id = 2,
                    Name = "Sam",
                    DOB = new DateTime(1990, 11, 15),
                    Email = "Sam@test.com",
                    Address = "USA"
                }
            });

            builder.HasOne(e => e.Department)
                .WithMany(e => e.Students)
                .HasForeignKey(e => e.DepartmentId)
                .HasConstraintName("FK_Students_Department");
        }
    }
}
