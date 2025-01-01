using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.DBSets.Config;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets
{
    public class CollegeDBContext : DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options)
        {
            
        }
        public DbSet<Student> Students {  get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());

            /*modelBuilder.Entity<Student>().HasData(new List<Student> {
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

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(30);
                entity.Property(e => e.DOB).IsRequired();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            });*/
        }
    }
}
